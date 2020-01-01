# Setup

## Các thư viện sử dụng

> + LiteNetLib: thư viện netword (udp)
> + MessagePack: Encode - Decode các packet giữ client - server: `request, response, message, model`
> + Newtonsoft.Json: để Encode - Decode `model` lưu vào PlayFab. *Căn nhắc đổi sang utf8json đồng bộ với MessagePack (vì cùng 1 người viêt) và được đánh giá là nhanh hơn*
> + PlayFab SDK C#

## Enviroment

> .Net Core SDK 2.2.8
>
> download: https://dotnet.microsoft.com/download/dotnet-core/2.2

# Publish

```sh
Vào folder chứa file `sln`
dotnet publish -c Release -r rhel.7-x64
```

> + Vào folder chứa file `sln`
>
> + `dotnet publish -c Release -r rhel.7-x64`

## Config Service

>sudo nano /etc/systemd/system/IAHNetCoreServer.service

Nhập nội dung:

```sh

[Unit]
Description=IAHNetCoreServer

[Service]
ExecStart=/usr/bin/dotnet /home/tinbq/dotnet_test/rhel.7-x64/publish/IAHNetCoreServer.dll
WorkingDirectory=/home/tinbq/dotnet_test/rhel.7-x64/publish/
Restart=on-failure
SyslogIdentifier=IAHNetCoreServer
PrivateTemp=true
KillSignal=true
KillSignal=SIGTERM

[Install]  
WantedBy=multi-user.target
```

> sudo systemctl daemon-reload
>
> sudo systemctl start IAHNetCoreServer
>
> sudo systemctl status IAHNetCoreServer
>
> sudo systemctl stop IAHNetCoreServer

# Cấu Trúc Project Server

+ Các phần code chung giữa server - client bao gồm: Các class base, define, model, request, response, message sẽ được định nghĩa ở server, sau đó gọi file batch để copy sang client.
+ Client muốn custom phần code chung (như là thêm function) kế thừa các class này. Về phía client thì phần code chung này xem như là auto gen.

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

