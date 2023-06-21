echo Publishing HtmlRun.SQL.NHibernate...
cd HtmlRun.SQL.NHibernate
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --self-contained -r win-x64 --output ../dist/plugins/HtmlRun.SQL.NHibernate/win-x64
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=false -c Release --self-contained -r linux-x64 --output ../dist/plugins/HtmlRun.SQL.NHibernate/linux-x64
cd ..
