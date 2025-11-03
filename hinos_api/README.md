# Hinos API

API REST desenvolvida em .NET 9 com Minimal API para gerenciamento de hinários.

## Execução Rápida

```bash
dotnet restore
dotnet run
```

Acesse: http://localhost:5000/swagger

## Docker

```bash
docker build -t hinos_api .
docker run -p 5000:8080 hinos_api
```

Veja a [documentação completa](../docs/hinos_api.md) para mais detalhes.
