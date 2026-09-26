# Build context must be the repo root (C:\code\FLUY), because this service
# references ../../../shared/Fluy.SharedKernel, which lives outside this folder.
# Build from the repo root with:
#   docker build -f fluy-service/Dockerfile -t resyerf/fluy-service:v1 .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY fluy-service/src/Fluy.Api/Fluy.Api.csproj fluy-service/src/Fluy.Api/
COPY fluy-service/src/Fluy.Application/Fluy.Application.csproj fluy-service/src/Fluy.Application/
COPY fluy-service/src/Fluy.Domain/Fluy.Domain.csproj fluy-service/src/Fluy.Domain/
COPY fluy-service/src/Fluy.Infrastructure/Fluy.Infrastructure.csproj fluy-service/src/Fluy.Infrastructure/
COPY shared/Fluy.SharedKernel/Fluy.SharedKernel.csproj shared/Fluy.SharedKernel/

RUN dotnet restore fluy-service/src/Fluy.Api/Fluy.Api.csproj

COPY fluy-service/src/ fluy-service/src/
COPY shared/ shared/

RUN dotnet publish fluy-service/src/Fluy.Api/Fluy.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Fluy.Api.dll"]
