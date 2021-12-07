FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /challenge
COPY . .
RUN dotnet restore
RUN dotnet publish -o /challenge/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /challenge
COPY --from=build /challenge/published-app /challenge
ENTRYPOINT [ "dotnet", "/challenge/API.dll" ]
