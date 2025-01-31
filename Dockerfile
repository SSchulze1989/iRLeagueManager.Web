FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /source
COPY . .
RUN dotnet restore src/iRLeagueManager.Web -r linux-x64
RUN dotnet publish src/iRLeagueManager.Web -r linux-x64 -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./iRLeagueManager.Web"]
