#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Server/QuickSack.Net.Server.csproj", "Server/"]
COPY ["Client/QuickSack.Net.Client.csproj", "Client/"]
COPY ["Shared/QuickSack.Net.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/QuickSack.Net.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "QuickSack.Net.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuickSack.Net.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickSack.Net.Server.dll"]