FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Woa.Chatbot/Woa.Chatbot.csproj", "Woa.Chatbot/"]
COPY ["Woa.Sdk/Woa.Sdk.csproj", "Woa.Sdk/"]
COPY ["Woa.Common/Woa.Common.csproj", "Woa.Common/"]
RUN dotnet restore "Woa.Chatbot/Woa.Chatbot.csproj"
COPY . .
WORKDIR "/src/Woa.Chatbot"
RUN dotnet build "Woa.Chatbot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Woa.Chatbot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Woa.Chatbot.dll"]
