@echo off

if "%1"=="windows" goto directory
if "%1"=="linux" goto directory

echo Usage: publish-plugins.bat [windows or linux]
goto end

:directory
echo Publishing HtmlRun.SQL.NHibernate...
cd ..\Plugins\HtmlRun.SQL.NHibernate

if "%1"=="windows" goto windows
if "%1"=="linux" goto linux

:windows
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --no-self-contained -r win-x64 --output ../../dist/plugins/HtmlRun.SQL.NHibernate/win-x64
goto restore

:linux
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --no-self-contained -r linux-x64 --output ../../dist/plugins/HtmlRun.SQL.NHibernate/linux-x64
goto restore

:restore
cd ..
cd ..

:end

