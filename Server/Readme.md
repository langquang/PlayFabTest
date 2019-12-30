Setup

Enviroment

> .Net Core SDK 2.2.8
>
> download: https://dotnet.microsoft.com/download/dotnet-core/2.2

# Publish

```sh
Vào folder chứa file `sln`
dotnet publish -c Release -r rhel.7-x64
```



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