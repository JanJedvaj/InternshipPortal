# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + csproj files first (better cache)
COPY InternshipPortal.API.sln ./
COPY InternshipPortal.BL/InternshipPortal.BL.csproj InternshipPortal.BL/
COPY InternshipPortal.API/InternshipPortal.API.csproj InternshipPortal.API/

# Restore only the API project (pulls required deps)
RUN dotnet restore InternshipPortal.API/InternshipPortal.API.csproj

# Copy the rest
COPY InternshipPortal.BL/ InternshipPortal.BL/
COPY InternshipPortal.API/ InternshipPortal.API/

# Publish only the API project
RUN dotnet publish InternshipPortal.API/InternshipPortal.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InternshipPortal.API.dll"]
