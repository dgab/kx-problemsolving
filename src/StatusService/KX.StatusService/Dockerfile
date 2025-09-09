#TODO: Some container hardening, e.g. Create User

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/StatusService/KX.StatusService/KX.StatusService.csproj", "src/StatusService/KX.StatusService/"]
RUN dotnet restore "src/StatusService/KX.StatusService/KX.StatusService.csproj"
COPY . .
WORKDIR "/src/src/StatusService/KX.StatusService"
RUN dotnet build "KX.StatusService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KX.StatusService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get upgrade -y && apt-get install curl -y

# After a little search I did not find solution to use docker socket without root permissions.
# Since an app like this in production would use some kind of orchestrator (like kubernetes)
# there it would be better to use the kube api server for status queries, which has proper RBAC

#RUN groupadd -r work && useradd -m -r -g work work
#RUN chown -v -H -R root:work /app
#
#RUN chown work: /usr/bin/curl
#RUN chmod u+rx /usr/bin/curl
#
#USER work


ENTRYPOINT ["dotnet", "KX.StatusService.dll"]
