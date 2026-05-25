FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia todo o conteúdo do repositório
COPY . .

# Compila apontando direto para o projeto da Sprint3 (evita o aviso de solução)
RUN dotnet publish Sprint3/Sprint3.csproj -c Release -o /app/publish

# Gera a imagem de execução
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 🔥 COMANDOS CRUCIAIS PARA EVITAR O ERRO 139 NO .NET 10 PREVIEW:
# Eles desativam a compilação em camadas experimental que faz o container crashar com SQLs grandes
ENV DOTNET_TieredCompilation=0
ENV DOTNET_ReadyToRun=0

ENTRYPOINT ["dotnet", "Sprint3.dll"]