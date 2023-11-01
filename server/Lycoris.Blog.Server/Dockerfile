FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime

RUN apt-get update && apt-get install -y git

VOLUME /app/AppData/
WORKDIR /app

COPY . /app/
ENTRYPOINT ["dotnet", "Lycoris.Blog.Server.dll"]

