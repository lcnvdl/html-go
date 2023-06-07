cd HtmlRun.Terminal
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../bin/win-x64
cd ..\bin\win-x64
move /y HtmlRun.Terminal.exe htmlgo.exe
cd ..\..
