FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# copy and publish app and libraries
COPY . .
RUN dotnet publish -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./iRLeagueManager.Web"]