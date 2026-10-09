FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ClyvoVetChallenge.sln", "./"]
COPY ["ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj", "ClyvoVetChallenge.Api/"]
COPY ["ClyvoVetChallenge.Application/ClyvoVetChallenge.Application.csproj", "ClyvoVetChallenge.Application/"]
COPY ["ClyvoVetChallenge.Domain/ClyvoVetChallenge.Domain.csproj", "ClyvoVetChallenge.Domain/"]
COPY ["ClyvoVetChallenge.Infrastructure/ClyvoVetChallenge.Infrastructure.csproj", "ClyvoVetChallenge.Infrastructure/"]
COPY ["ClyvoVetChallenge.Tests.Unit/ClyvoVetChallenge.Tests.Unit.csproj", "ClyvoVetChallenge.Tests.Unit/"]
COPY ["ClyvoVetChallenge.Tests.Integration/ClyvoVetChallenge.Tests.Integration.csproj", "ClyvoVetChallenge.Tests.Integration/"]

RUN dotnet restore "ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj"

COPY . .
WORKDIR "/src/ClyvoVetChallenge.Api"
RUN dotnet publish "ClyvoVetChallenge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

RUN mkdir -p /app/logs && chown -R app:app /app

USER app

COPY --chown=app:app --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClyvoVetChallenge.Api.dll"]
