FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

WORKDIR /src
COPY sausage-rolls.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 

WORKDIR /app
COPY --from=build /out/ /app

ENTRYPOINT ["dotnet", "/app/sausage-roll.dll"]