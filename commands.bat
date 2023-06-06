dotnet new sln --output HtmlRun

cd HtmlRun

dotnet new console --name HtmlRun.Terminal
dotnet sln add HtmlRun.Terminal

dotnet new classlib --name HtmlRun.Common
dotnet sln add HtmlRun.Common

dotnet new classlib --name HtmlRun.Interpreter
dotnet sln add HtmlRun.Interpreter
dotnet add HtmlRun.Interpreter reference HtmlRun.Common
dotnet add HtmlRun.Interpreter package AngleSharp --version 1.0.2

dotnet new classlib --name HtmlRun.Interpreter
dotnet sln add HtmlRun.Interpreter
dotnet add HtmlRun.Interpreter reference HtmlRun.Common
dotnet add HtmlRun.Interpreter package AngleSharp --version 1.0.2

dotnet new classlib --name HtmlRun.Runtime
dotnet sln add HtmlRun.Runtime
dotnet add HtmlRun.Runtime reference HtmlRun.Common
dotnet add HtmlRun.Runtime package Jurassic --version 3.2.6

dotnet add HtmlRun.Terminal reference HtmlRun.Interpreter
dotnet add HtmlRun.Terminal reference HtmlRun.Runtime