# Setup

## Các thư viện sử dụng

> + ## LiteNetLib: thư viện netword (udp)
> + MessagePack: Encode - Decode các packet giữ client - server: `request, response, message, model`
> + Newtonsoft.Json: để Encode - Decode `model` lưu vào PlayFab. *Căn nhắc đổi sang utf8json đồng bộ với MessagePack (vì cùng 1 người viêt) và được đánh giá là nhanh hơn*
> + PlayFab SDK C#

## Enviroment

> .Net Core SDK 2.2.8
>
> download: https://dotnet.microsoft.com/download/dotnet-core/2.2

## Setup Server

> + Register Microsoft key and feed:
>
>    `sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm`
>
> + Install the .NET Core SDK:
>
>   `sudo yum install dotnet-sdk-3.1`
>
> + Install the .NET Core runtime
>   `sudo yum install dotnet-runtime-3.1`
>
> + Install htop để thay thế top
>
>   `sudo yum install htop`

# Publish

```sh
Vào folder chứa file `sln`
dotnet publish -c Release -r rhel.7-x64
```

> + Vào folder chứa file `sln`
>+ `dotnet publish -c Release -r rhel.7-x64 --framework netcoreapp2.2`

## Config Service

>+ Install **nano** đê edit file text
>
>  ```shell
>	sudo yum install nano
>  ```
>
>+ Tạo file server
>
>```shell
>	sudo nano /etc/systemd/system/IAHServerAPI.service
>```
>

Nhập nội dung sau vào file **IAHServerAPI.service**:

```sh

[Unit]
Description=IAHServerAPI

[Service]
ExecStart=/usr/bin/dotnet /home/tinbq/API_Server/publish/IAHNetCoreServer.dll
WorkingDirectory=/home/tinbq/API_Server/publish/
Restart=on-failure
SyslogIdentifier=IAHServerAPI
PrivateTmp=true
KillSignal=SIGTERM

[Install]  
WantedBy=multi-user.target
```

> sudo systemctl daemon-reload
>
> sudo systemctl start IAHServerAPI
>
> sudo systemctl status IAHServerAPI
>
> sudo systemctl stop IAHServerAPI



## Ý Tưởng Chính

> Trù tượng thành các class sau:
>
> + NetServer (Client): xử lý kết nối mạng
> + NetPlayer: Mô tả 1 thực thể của user ở client và server. 
> + NetData: 1 một tin được gửi qua netword
> + Handler: Xử lý logic khi có NetData đến

# Cấu Trúc Project Server

+ Các phần code chung giữa server - client bao gồm: Các class base, define, model, request, response, message sẽ được định nghĩa ở server, sau đó gọi file batch để copy sang client.
+ Client muốn custom phần code chung (như là thêm function) thì kế thừa các class này. Về phía client thì phần code chung này xem như là auto gen.

https://drive.google.com/file/d/1EQGaFBaRPDT_IC5Mu9oZUqWKh5AW6DKM/view?usp=sharing

![](https://drive.google.com/file/d/1EQGaFBaRPDT_IC5Mu9oZUqWKh5AW6DKM/view?usp=sharing)

### Ví dụ: Tạo server và đăng ký các sử lý (client tương tự)

```c#
var netServer = new NetServer<DataPlayer>("Server", new EntryHandler());
netServer.Start(9000, "secret_key");
```

> + Tạo 1 server có name là "Server", tên dùng cho debug khi có nhiều `server` như **api-server** (kết nối giữ user - server), **battle-server** (kết nối giữ server battle - server)
> + Định nghĩ 1 player là một instance của class DataPlayer, class này kế thừa từ class `NetPlayer`
> + Handler để xử lý logic là `EntryHandler` : handler này dùng để đăng ký map các request vào các hàm tương ứng.
> + Start server với port 9000 và `secret_key` để kết nối

```c#
public EntryHandler()
{
    _router = new NetRouter<DataPlayer>("API-Server-Router" ,new TimeOutChecker(this));
    // Register headers
    _router.RegisterHeader<RequestHeader>(() => new RequestHeader());
    _router.RegisterHeader<ResponseHeader>(() => new ResponseHeader());

    // Register các hàm để xử lý các request tương ứng
    _router.Subscribe<TestRequest>(NetAPICommand.TEST_REQUEST, TestHandler.Perform);
    _router.Subscribe<CreateMasterAccountRequest>(NetAPICommand.CREATE_MASTER_ACCOUNT, CreateMasterAccountHandler.Perform);
    _router.Subscribe<CheckCreateNodeAccountRequest>(NetAPICommand.CHECK_CREATE_NODE_ACCOUNT, CheckCreateNodeAccountHandler.Perform);
    _router.Subscribe<CreateNodeAccountRequest>(NetAPICommand.CREATE_NODE_ACCOUNT, CreateNodeAccountHandler.Perform);
}
```

> Ví dụ xử lý TestRequest

```c#
public static async Task<INetData> Perform(TestRequest request, DataPlayer player)
        {
            Logger.Debug($"Server receive a Test Command with content={request.msg}");

            if (!request.IsValid()) // check require params
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.WRONG_REQUEST);
            }

            //----------------------------------------------------------------------------------------------------------
            //---------------- EDIT DATA -------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            // increase currency
            player.IncreaseGem(500); // auto save to PlayFab and auto sync to client
            // decrease Gold
            player.DecreaseGold(100); // auto save to PlayFab and auto sync to client
            // change statistic
            player.Level++; // auto save to PlayFab and auto sync to client
            // grant item
            IncreaseInventoryItemState state = player.IncreaseInventoryItem("HealthPotion", 5); // grant new if not exist or not stackable
            switch (state)
            {
                case IncreaseInventoryItemState.FAIL:
                    Logger.Debug("Wrong logic here, increase negative value");
                    break;
                case IncreaseInventoryItemState.UPDATE_CACHES_IN_LATER:
                    Logger.Debug("Increase successfull but need to wait for response from PlayFab <instanceId> to update cache Player");
                    break;
                case IncreaseInventoryItemState.UPDATE_CACHES_IN_IMMEDIATE:
                    Logger.Debug("Increase successfull");
                    break;
            }

            // revoke item
            var itemInstance = player.FindFirstInventoryItemByItemId("HealthPotion");
            if (itemInstance != null)
            {
                player.DecreaseInventoryItem(itemInstance, 2);

                // edit instance custom data
                if (itemInstance.CustomData == null)
                    itemInstance.CustomData = new Dictionary<string, string>();
                if (itemInstance.CustomData.TryGetValue("star", out var star))
                {
                    itemInstance.CustomData["star"] = (int.Parse(star) + 1).ToString();
                }
                else
                {
                    itemInstance.CustomData["star"] = "1";
                }

                player.UpdateInventoryItemCustomData(itemInstance); // call this function to save new CustomData to PlayFab
            }

            // edit data
            player.KeyReward.OnlineReward.nextAt = DateTime.UtcNow;                       // online reward is a property inside KeyReward
            player.AddSyncEntities(SyncEntityName.ONLINE_REWARD);             // mask sync entity OnlineReward to client
            player.AddChangedDataFlag(PFPlayerDataFlag.REWARD | PFPlayerDataFlag.REWARD); // mask save key REWARD to PlayFab

            //----------------------------------------------------------------------------------------------------------
            //---------------- COMMIT EDITED DATA ----------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            // sync to client
            SyncHelper.SyncDataToClient(player); // send a SyncData message to client
            // save to PlayFab
            await PFDriver.CommitChangedToPlayFab(player); // asynchronous

            // Response for current request
            var response = new TestResponse(request) {msg = "A response of test command from server"};
            player.Send(response);
            return response;
        }
```



## Cấu trúc 1 gói tin (Net Packet)

> Gồm 2 phần:
>
> + Header: là 1 class định nghĩa các thông tin cơ bản của gói tin, các class này phải được đăng ký vào Router
> + Data: Chứ data của riêng gói tin này

> Tất cả các data trong gói tin đều xử dụng MessagePack để đóng gói để tăng khả năng mở rộng sau này và để đồng bộ

## Tự động save user data vào PlayFab

Ý tưởng là ẩn đi phần save thay đổi data của user vào PlayFab. Thực hiện bằng các tạo một class là gọi là Receipt chứa tất các các thông tin của user. Data của user được thay đổi thông qua các Get/Set, trong các Get/Set sẽ `fill` và Receipt này. Khi sử lý xong request thì gọi hàm `commitChange` để sent Receipt này sang CloudScript, CloudScript sẽ kiểm tra xem phần này thay đổi và lưu xuống PlayFab

## Tự động đồng bộ user data xuống Client

Tương tự như thực hiện với PlayFab. Data nào thay đổi sẽ được gửi xuống client thông qua message `SyncData`. Client khi nhận message này sẽ update data. 

Message `SyncData` nên được gửi xuống client trước khi kết quả của request trả về.

##  Chia nhỏ data thành các Entity

+ Chia nhỏ để tăng hiệu suất (giảm băng thông, giảm thời gian encode-decode packet)

+ Do PlayFab hạn chế chỉ có thể update cùng lúc 10 key nên việc chi nhỏ này chỉ thực hiện được ở chức năng tự động đồng bộ user data xuống client.

  Ví dụ: Trong PlayFab có 1 key là **Rewards**, trong key này sẽ chứa các entity là **OnlineReward, DailyReward, WeeklyReward...** thì lúc đó lúc update data vào PlayFab vẫn phải update cả key **Rewards** nhưng lúc sync xuống client chỉ cần update những entity nhỏ trong đó.

## Tạo 1 class GiftPack để chứa thông tin gói quà, phần thưởng...

Thống nhất sử dụng 1 class duy nhất để tiện config, xử lý hiển thị, cập nhật UI ở client



## Giả quyết vấn đề InstanceId khi grant 1 item mới vào Inventory trong PlayFab

> Server sẽ gửi GiftPack về trước cho client hiển thị, trong thời gian đó nếu PlayFab grant item xong và trả về cho server thì server sẽ gọi message `SyncData` xuống client để update lần nữa. Lần gọi `SyncData` trước sẽ không có thông tin về item mới được grant này.

## Class Diagram

https://drive.google.com/file/d/1eEzPaLSJmd-K5JIEfJz6JDtFpVMMdXf2/view?usp=sharing

![](https://drive.google.com/file/d/1eEzPaLSJmd-K5JIEfJz6JDtFpVMMdXf2/view?usp=sharing)

