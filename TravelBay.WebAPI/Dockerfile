FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY TravelBay.sln ./
COPY TravelBay.Model/TravelBay.Model.csproj TravelBay.Model/
COPY TravelBay.Common.Services/TravelBay.Common.Services.csproj TravelBay.Common.Services/
COPY TravelBay.Services/TravelBay.Services.csproj TravelBay.Services/
COPY TravelBay.WebAPI/TravelBay.WebAPI.csproj TravelBay.WebAPI/
RUN dotnet restore TravelBay.WebAPI/TravelBay.WebAPI.csproj

COPY TravelBay.Model/ TravelBay.Model/
COPY TravelBay.Common.Services/ TravelBay.Common.Services/
COPY TravelBay.Services/ TravelBay.Services/
COPY TravelBay.WebAPI/ TravelBay.WebAPI/
RUN dotnet publish TravelBay.WebAPI/TravelBay.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TravelBay.WebAPI.dll"]
