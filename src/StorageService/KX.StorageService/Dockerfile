#TODO: Some container hardening, e.g. Create User

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/StorageService/KX.StorageService/KX.StorageService.csproj", "src/StorageService/KX.StorageService/"]
RUN dotnet restore "src/StorageService/KX.StorageService/KX.StorageService.csproj"
COPY . .
WORKDIR "/src/src/StorageService/KX.StorageService"
RUN dotnet build "KX.StorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KX.StorageService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get upgrade -y && apt-get install curl -y

RUN groupadd -r work && useradd -m -r -g work work
RUN chown -v -H -R root:work /app

RUN chown work: /usr/bin/curl
RUN chmod u+rx /usr/bin/curl

USER work

ENTRYPOINT ["dotnet", "KX.StorageService.dll"]
