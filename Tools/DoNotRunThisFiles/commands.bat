echo DON NOT RUN THIS FILE
exit

dotnet new sln --output HtmlRun

cd HtmlRun

dotnet new webapi --name HtmlRun.WebApi
dotnet sln add HtmlRun.WebApi

@REM dotnet new worker --name HtmlRun.Server
@REM dotnet sln add HtmlRun.Server

dotnet new console --name HtmlRun.Terminal
dotnet sln add HtmlRun.Terminal

dotnet new classlib --name HtmlRun.Common
dotnet sln add HtmlRun.Common

dotnet new classlib --name HtmlRun.Common.Runtime
dotnet sln add HtmlRun.Common.Runtime
dotnet add HtmlRun.Common.Runtime reference HtmlRun.Common

dotnet new classlib --name HtmlRun.SQL.NHibernate
dotnet sln add HtmlRun.SQL.NHibernate
dotnet add HtmlRun.SQL.NHibernate reference HtmlRun.Common
dotnet add HtmlRun.SQL.NHibernate reference HtmlRun.Common.Runtime
dotnet add HtmlRun.SQL.NHibernate package FluentNHibernate --version 3.2.1
dotnet add HtmlRun.SQL.NHibernate package System.Data.SQLite

dotnet new classlib --name HtmlRun.Interpreter
dotnet sln add HtmlRun.Interpreter
dotnet add HtmlRun.Interpreter reference HtmlRun.Common
dotnet add HtmlRun.Interpreter package AngleSharp --version 1.0.2

dotnet new classlib --name HtmlRun.Runtime
dotnet sln add HtmlRun.Runtime
dotnet add HtmlRun.Runtime reference HtmlRun.Common
dotnet add HtmlRun.Runtime reference HtmlRun.Common.Runtime
dotnet add HtmlRun.Runtime package Jurassic --version 3.2.6

dotnet add HtmlRun.Terminal reference HtmlRun.Interpreter
dotnet add HtmlRun.Terminal reference HtmlRun.Runtime

@REM dotnet add HtmlRun.Server reference HtmlRun.Interpreter
@REM dotnet add HtmlRun.Server reference HtmlRun.Runtime

dotnet add HtmlRun.WebApi reference HtmlRun.Interpreter
dotnet add HtmlRun.WebApi reference HtmlRun.Runtime

dotnet new xunit --name HtmlRun.Tests
dotnet sln add HtmlRun.Tests
dotnet add HtmlRun.Tests reference HtmlRun.Common
dotnet add HtmlRun.Tests reference HtmlRun.Common.Runtime
dotnet add HtmlRun.Tests reference HtmlRun.Interpreter
dotnet add HtmlRun.Tests reference HtmlRun.Runtime

@REM DIFFERENT COMPILATION WAYS
@REM dotnet publish HtmlRun.sln -c Release -r win-x64  -p:PublishSingleFile=true --output ./bin
@REM dotnet publish HtmlRun.sln -c Release --self-contained -r win-x64 -p:PublishSingleFile=true --output ./bin/win-x64
@REM dotnet publish HtmlRun.sln /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../bin/win-x64
@REM dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true -c Release --self-contained -r win-x64 --output ../bin/win-x64
