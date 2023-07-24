echo Publishing HtmlRun.Terminal...
cd Runtimes\HtmlRun.Terminal
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../../dist/client/portable/win-x64
cd ..\..\dist\client\portable\win-x64
move /y HtmlRun.Terminal.exe htmlgo.exe
cd ..\..\..\..

echo Publishing HtmlRun.WebApi...
cd Runtimes\HtmlRun.WebApi
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../../dist/server/portable/win-x64
cd ..\..\dist\server\portable\win-x64
move /y HtmlRun.WebApi.exe htmlgo-server.exe
del /f /q web.config
cd ..\..\..\..

call .\Tools\publish-plugins.bat windows