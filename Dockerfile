FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GestBibliothequeDotnet8/GestBibliothequeDotnet8.csproj", "GestBibliothequeDotnet8/"]
RUN dotnet restore "GestBibliothequeDotnet8/GestBibliothequeDotnet8.csproj"
COPY . .
WORKDIR "/src/GestBibliothequeDotnet8"
RUN dotnet build "GestBibliothequeDotnet8.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestBibliothequeDotnet8.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestBibliothequeDotnet8.dll"]