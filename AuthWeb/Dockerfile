# Используем образ ASP.NET для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8087


# Используем SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Устанавливаем git и клонируем репозиторий
RUN apt-get update && apt-get install -y git \
    && git clone https://<TOKEN>@github.com/trokhin87/AuthService.git myrepo

# Переходим в каталог с проектом
WORKDIR /src/myrepo/AuthWeb

# Восстанавливаем зависимости и собираем проект
RUN dotnet restore AuthWeb.csproj
RUN dotnet publish AuthWeb.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ с минимальным размером
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV dbProxy=UENV
ENV JwtKey=UENV
ENV JwtIssuer=UENV
ENV JwtAudience=UENV

# Запускаем приложение
ENTRYPOINT ["dotnet", "AuthWeb.dll"]
