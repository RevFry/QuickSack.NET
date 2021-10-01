#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Server/QuickSack.Server.csproj", "Server/"]
COPY ["Client/QuickSack.Client.csproj", "Client/"]
COPY ["Shared/QuickSack.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/QuickSack.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "QuickSack.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuickSack.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickSack.Server.dll"]