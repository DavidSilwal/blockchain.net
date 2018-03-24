FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY *.sln ./
COPY Blockchain.NET.Node/Blockchain.NET.Node.csproj Blockchain.NET.Node/
COPY Blockchain.NET.Blockchain/Blockchain.NET.Blockchain.csproj Blockchain.NET.Blockchain/
COPY Blockchain.NET.Core/Blockchain.NET.Core.csproj Blockchain.NET.Core/
RUN dotnet restore
COPY . .
WORKDIR /src/Blockchain.NET.Node
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Blockchain.NET.Node.dll"]
