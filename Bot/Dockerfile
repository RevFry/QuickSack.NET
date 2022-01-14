#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bot/QuickSack.Net.Bot.csproj", "Bot/"]
COPY ["Shared/QuickSack.Net.Shared.csproj", "Shared/"]
RUN dotnet restore "Bot/QuickSack.Net.Bot.csproj"
COPY . .
WORKDIR "/src/Bot"
RUN dotnet build "QuickSack.Net.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuickSack.Net.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickSack.Net.Bot.dll"]