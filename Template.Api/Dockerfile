FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER $APP_UID
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/Template.Api/Template.Api.csproj", "src/Template.Api/"]
COPY ["src/Template.Shared/Template.Shared.csproj", "src/Template.Shared/"]
COPY ["src/Modules/Template.Modules.Sample/Template.Modules.Sample.csproj", "src/Modules/Template.Modules.Sample/"]

RUN dotnet restore "src/Template.Api/Template.Api.csproj"

COPY . .

WORKDIR "/src/src/Template.Api"

RUN dotnet build "Template.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Template.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Template.Api.dll"]