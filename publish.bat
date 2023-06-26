echo Publishing HtmlRun.Terminal...
cd HtmlRun.Terminal
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../dist/client/portable/win-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r linux-x64 --output ../dist/client/portable/linux-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --self-contained -r win-x64 --output ../dist/client/standalone/win-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --self-contained -r linux-x64 --output ../dist/client/standalone/linux-x64
cd ..\dist\client\portable\win-x64
move /y HtmlRun.Terminal.exe htmlgo.exe
cd ..\linux-x64
move /y HtmlRun.Terminal htmlgo
cd ..\..\standalone\win-x64
move /y HtmlRun.Terminal.exe htmlgo.exe
cd ..\..\standalone\linux-x64
move /y HtmlRun.Terminal htmlgo
cd ..\..\..\..

echo Publishing HtmlRun.WebApi...
cd HtmlRun.WebApi
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../dist/server/portable/win-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r linux-x64 --output ../dist/server/portable/linux-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --self-contained -r win-x64 --output ../dist/server/standalone/win-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --self-contained -r linux-x64 --output ../dist/server/standalone/linux-x64
cd ..\dist\server\portable\win-x64
move /y HtmlRun.WebApi.exe htmlgo-server.exe
cd ..\linux-x64
move /y HtmlRun.WebApi htmlgo-server
cd ..\..\standalone\win-x64
move /y HtmlRun.WebApi.exe htmlgo-server.exe
cd ..\..\standalone\linux-x64
move /y HtmlRun.WebApi htmlgo-server
cd ..\..\..\..

call publish-plugins.bat