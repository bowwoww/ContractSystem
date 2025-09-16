FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update && \
    apt-get install -y locales && \
    sed -i '/zh_TW.UTF-8/s/^# //g' /etc/locale.gen && \
    locale-gen zh_TW.UTF-8

ENV LANG=zh_TW.UTF-8
ENV LC_ALL=zh_TW.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "GymSystem.dll"]