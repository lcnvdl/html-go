echo Publishing HtmlRun.Terminal...
cd HtmlRun.Terminal
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../dist/client/portable/win-x64
cd ..\dist\client\portable\win-x64
move /y HtmlRun.Terminal.exe htmlgo.exe
cd ..\..\..\..

echo Publishing HtmlRun.WebApi...
cd HtmlRun.WebApi
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../dist/server/portable/win-x64
cd ..\dist\server\portable\win-x64
move /y HtmlRun.WebApi.exe htmlgo-server.exe
cd ..\..\..\..

call publish-plugins.bat