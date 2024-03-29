FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LCT.Api/LCT.Api.csproj", "LCT.Api/"]
COPY ["LCT.Application/LCT.Application.csproj", "LCT.Application/"]
COPY ["LCT.Domain/LCT.Domain.csproj", "LCT.Domain/"]
COPY ["LCT.Core/LCT.Core.csproj", "LCT.Core/"]
COPY ["LCT.Infrastructure/LCT.Infrastructure.csproj", "LCT.Infrastructure/"]
RUN dotnet restore "LCT.Api/LCT.Api.csproj"
COPY . .
WORKDIR "/src/LCT.Api"
RUN dotnet build "LCT.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LCT.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LCT.Api.dll"]