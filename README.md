# Welp

Project Uses BlazorServer Template and couple extras

## Install Dotnet

1. [Download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
1. If it's installed correctly, you can see info from terminal with `dotnet --info`

    ```bash
    @:~/EN.605.601/Welp$ dotnet --info
    .NET SDK (reflecting any global.json):
    Version:   6.0.122
    Commit:    dc5a76ad5c

    Runtime Environment:
    OS Name:     ubuntu
    OS Version:  22.04
    OS Platform: Linux
    RID:         ubuntu.22.04-x64
    Base Path:   /usr/lib/dotnet/sdk/6.0.122/

    global.json file:
    Not found

    Host:
    Version:      6.0.22
    Architecture: x64
    Commit:       4bb6dc195c

    .NET SDKs installed:
    6.0.122 [/usr/lib/dotnet/sdk]

    .NET runtimes installed:
    Microsoft.AspNetCore.App 6.0.22 [/usr/lib/dotnet/shared/Microsoft.AspNetCore.App]
    Microsoft.NETCore.App 6.0.22 [/usr/lib/dotnet/shared/Microsoft.NETCore.App]

    Download .NET:
    https://aka.ms/dotnet-download

    Learn about .NET Runtimes and SDKs:
    https://aka.ms/dotnet/runtimes-sdk-info

```

1. Run the app via cli with `dotnet watch run` then open browser to https://localhost:

1. Open vscode at repository root. This should be the folder containing `Welp.csproj`.

1. Install c# extension(s) in vscode. You should be prompted to install all the items from `./.vscode/extensions.json`