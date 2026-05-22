FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj"
RUN dotnet publish "ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClyvoVetChallenge.Api.dll"]