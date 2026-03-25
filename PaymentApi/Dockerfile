# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
COPY PaymentApi/PaymentApi.csproj PaymentApi/
RUN dotnet restore PaymentApi/PaymentApi.csproj

COPY SharedLibrary/ SharedLibrary/
COPY PaymentApi/ PaymentApi/
WORKDIR /src/PaymentApi
RUN dotnet publish PaymentApi.csproj -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentApi.dll"]
