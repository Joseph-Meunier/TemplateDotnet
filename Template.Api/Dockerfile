FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER root

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID

WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

COPY ["Template.Api/Template.Api.csproj", "Template.Api/"]
COPY ["Template.Shared/Template.Shared.csproj", "Template.Shared/"]
COPY ["Template.Modules.Users/Template.Modules.Users.csproj", "Template.Modules.Users/"]
COPY ["Template.Modules.Blog/Template.Modules.Blog.csproj", "Template.Modules.Blog/"]

RUN dotnet restore "Template.Api/Template.Api.csproj"

COPY . .

WORKDIR "/src/Template.Api"

RUN dotnet build "Template.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore


FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Template.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Template.Api.dll"]