# ─────────────────────────────────────────────
# Stage 1: build
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GDSwithREST/GDSwithREST.csproj                               GDSwithREST/
COPY GDSwithREST.Domain/GDSwithREST.Domain.csproj                 GDSwithREST.Domain/
COPY GDSwithREST.Infrastructure/GDSwithREST.Infrastructure.csproj GDSwithREST.Infrastructure/

RUN dotnet restore GDSwithREST/GDSwithREST.csproj

COPY . .

RUN dotnet publish GDSwithREST/GDSwithREST.csproj \
    -c Release \
    -o /app/publish

# ─────────────────────────────────────────────
# Stage 2: runtime
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Bake config and cert into the image so the entrypoint
# can copy them into the volume-mounted paths at startup
COPY --from=build /src/GDSwithREST/GdsConfig ./GdsConfig
COPY --from=build /src/GDSwithREST/HTTPSCert ./HTTPSCert

COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

EXPOSE 58810
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["/docker-entrypoint.sh"]