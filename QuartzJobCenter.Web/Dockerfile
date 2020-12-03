#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["QuartzJobCenter.Web/QuartzJobCenter.Web.csproj", "QuartzJobCenter.Web/"]
RUN dotnet restore "QuartzJobCenter.Web/QuartzJobCenter.Web.csproj"
COPY . .
WORKDIR "/src/QuartzJobCenter.Web"
RUN dotnet build "QuartzJobCenter.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuartzJobCenter.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuartzJobCenter.Web.dll"]