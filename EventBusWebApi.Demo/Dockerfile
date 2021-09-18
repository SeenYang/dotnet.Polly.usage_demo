FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EventBusWebApi.Demo/EventBusWebApi.Demo.csproj", "EventBusWebApi.Demo/"]
RUN dotnet restore "EventBusWebApi.Demo/EventBusWebApi.Demo.csproj"
COPY . .
WORKDIR "/src/EventBusWebApi.Demo"
RUN dotnet build "EventBusWebApi.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventBusWebApi.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventBusWebApi.Demo.dll"]
