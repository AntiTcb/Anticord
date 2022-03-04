#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/OrgBot/OrgBot.csproj", "src/OrgBot/"]
RUN dotnet restore "src/OrgBot/OrgBot.csproj"
COPY . .
WORKDIR "/src/src/OrgBot"
RUN dotnet build "OrgBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrgBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrgBot.dll"]