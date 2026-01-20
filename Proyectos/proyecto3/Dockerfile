FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Proyecto3.csproj", "./"]
RUN dotnet restore "Proyecto3.csproj"
COPY . .
RUN dotnet build "Proyecto3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Proyecto3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Proyecto3.dll"]
