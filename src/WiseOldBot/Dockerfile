#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WiseOldBot/WiseOldBot.csproj", "src/WiseOldBot/"]
RUN dotnet restore "src/WiseOldBot/WiseOldBot.csproj"
COPY . .
WORKDIR "/src/src/WiseOldBot"
RUN dotnet build "WiseOldBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WiseOldBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WiseOldBot.dll"]