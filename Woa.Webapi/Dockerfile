FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Woa.Webapi/Woa.Webapi.csproj", "Woa.Webapi/"]
COPY ["Woa.Sdk/Woa.Sdk.csproj", "Woa.Sdk/"]
COPY ["Woa.Common/Woa.Common.csproj", "Woa.Common/"]
RUN dotnet restore "Woa.Webapi/Woa.Webapi.csproj"
COPY . .
WORKDIR "/src/Woa.Webapi"
RUN dotnet build "Woa.Webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Woa.Webapi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Woa.Webapi.dll"]
