echo Publishing HtmlRun.SQL.NHibernate...
cd Plugins\HtmlRun.SQL.NHibernate

if "%1"=="windows" goto windows
if "%1"=="linux" goto linux
goto end

:windows
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release -r win-x64 --output ../../dist/plugins/HtmlRun.SQL.NHibernate/win-x64
goto end

:linux
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release -r linux-x64 --output ../../dist/plugins/HtmlRun.SQL.NHibernate/linux-x64
goto end

:end
cd ..
cd ..
