# ── Stage 1: BUILD ───────────────────────────────────────────────────────
# Use the full .NET 9 SDK image — this has the compiler (dotnet build/publish)
# We only use this stage to compile the app, not to run it
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory inside the container
WORKDIR /src

# Copy ONLY the .csproj file first and restore NuGet packages.
# Docker caches each step — if .csproj hasn't changed, it reuses
# the cached restore layer instead of downloading packages every build.
# This makes rebuilds much faster when you only change .cs files.
COPY MedNex_Backend.API/MedNex_Backend.API.csproj MedNex_Backend.API/
RUN dotnet restore MedNex_Backend.API/MedNex_Backend.API.csproj

# Now copy the rest of your source code
COPY MedNex_Backend.API/ MedNex_Backend.API/

# Move into the project folder
WORKDIR /src/MedNex_Backend.API

# Publish in Release mode — compiles, optimizes, and outputs to /app/publish
# --no-restore: skip restore since we already did it above
# -o /app/publish: output folder
RUN dotnet publish MedNex_Backend.API.csproj \
    -c Release \
    --no-restore \
    -o /app/publish


# ── Stage 2: RUNTIME ─────────────────────────────────────────────────────
# Use the lightweight ASP.NET runtime image — no compiler, just the runtime
# This is the image that actually gets deployed and run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Copy ONLY the compiled output from Stage 1 — not the source code
# The source code never ends up in the final image
COPY --from=build /app/publish .

# Tell Docker that the app listens on port 5129 (matches your launchSettings)
# This is documentation — you still need to map ports in docker-compose
EXPOSE 5129

# The command that runs when the container starts
# Runs the compiled DLL directly
ENTRYPOINT ["dotnet", "MedNex_Backend.API.dll"]