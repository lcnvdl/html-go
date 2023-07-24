echo Publishing HtmlRun.Terminal...
cd HtmlRun.Terminal
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r linux-x64 --output ../../dist/client/portable/linux-x64
cd ..\..\dist\client\portable\
cd linux-x64
move /y HtmlRun.Terminal htmlgo
cd ..\..\..\..

echo Publishing HtmlRun.WebApi...
cd HtmlRun.WebApi
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r linux-x64 --output ../../dist/server/portable/linux-x64
cd ..\..\dist\server\portable\
cd linux-x64
move /y HtmlRun.WebApi htmlgo-server
cd ..\..\..\..

call .\Tools\publish-plugins.bat linux