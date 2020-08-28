#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update \ 
	&& apt-get install -y imagemagick \
	&& rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["imgresizer.csproj", ""]
RUN dotnet restore "./imgresizer.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "imgresizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "imgresizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "imgresizer.dll"]