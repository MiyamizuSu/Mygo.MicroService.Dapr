#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Contrib/MaskedTestList.Api/MaskedTestList.Api.csproj", "Contrib/MaskedTestList.Api/"]
RUN dotnet restore "./Contrib/MaskedTestList.Api/MaskedTestList.Api.csproj"
COPY . .
WORKDIR "/src/Contrib/MaskedTestList.Api"
RUN dotnet build "./MaskedTestList.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MaskedTestList.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecAll.Contrib.MaskedTestList.Api.dll"]