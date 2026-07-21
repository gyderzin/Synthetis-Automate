<<<<<<< HEAD
# ===========================
# Etapa 1 - Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia somente o csproj primeiro (melhora cache)
COPY *.csproj ./

RUN dotnet restore

# Copia o restante do projeto
COPY . .

RUN dotnet publish -c Release -o /app/publish

# ===========================
# Etapa 2 - Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Instala bibliotecas necessárias
RUN apt-get update && \
    apt-get install -y \
    libc6 \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

# Cria a pasta onde os relatórios serão gravados
RUN mkdir -p /relatorios

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

=======
# ===========================
# Etapa 1 - Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia somente o csproj primeiro (melhora cache)
COPY *.csproj ./

RUN dotnet restore

# Copia o restante do projeto
COPY . .

RUN dotnet publish -c Release -o /app/publish

# ===========================
# Etapa 2 - Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Instala bibliotecas necessárias
RUN apt-get update && \
    apt-get install -y \
    libc6 \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

# Cria a pasta onde os relatórios serão gravados
RUN mkdir -p /relatorios

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
ENTRYPOINT ["dotnet","BackEnd_Synthetis.dll"]