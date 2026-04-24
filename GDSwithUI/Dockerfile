# ─────────────────────────────────────────────
# Stage 1: build
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GDSwithUI/GDSwithUI.csproj GDSwithUI/
RUN dotnet restore GDSwithUI/GDSwithUI.csproj

COPY . .

# Show everything at /src level to find the rogue Program.cs
RUN echo "=== /src root ===" && ls -la /src/ \
 && echo "=== /src/GDSwithUI ===" && ls -la /src/GDSwithUI/

RUN dotnet publish GDSwithUI/GDSwithUI.csproj \
    -c Release \
    -o /app/publish

# ─────────────────────────────────────────────
# Stage 2: runtime
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8082
EXPOSE 8083

ENTRYPOINT ["dotnet", "GDSwithUI.dll"]
