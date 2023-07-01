# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

ARG APP_DIRECTORY=/Examples/WebApi
ARG APP_FILE=/01-hello_world.html

WORKDIR /publish
COPY ${APP_DIRECTORY}/* .
RUN mv -f ${APP_FILE} Index.html

WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "HtmlRun.WebApi.dll", "Index.html"]