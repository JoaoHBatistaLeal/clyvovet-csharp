FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj", "ClyvoVetChallenge.Api/"]
COPY ["ClyvoVetChallenge.Application/ClyvoVetChallenge.Application.csproj", "ClyvoVetChallenge.Application/"]
COPY ["ClyvoVetChallenge.Domain/ClyvoVetChallenge.Domain.csproj", "ClyvoVetChallenge.Domain/"]
COPY ["ClyvoVetChallenge.Infrastructure/ClyvoVetChallenge.Infrastructure.csproj", "ClyvoVetChallenge.Infrastructure/"]

RUN dotnet restore "ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj"

COPY . .
WORKDIR "/src/ClyvoVetChallenge.Api"
RUN dotnet publish "ClyvoVetChallenge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "ClyvoVetChallenge.Api.dll"]