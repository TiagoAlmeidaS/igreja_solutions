# Hinos API

API REST desenvolvida em .NET 9 com Minimal API para gerenciamento de hinários.

## Estrutura do Projeto

Este projeto possui uma solution file `hinos_api.sln` localizada nesta pasta.

### Projetos na Solution

- **hinos_api**: Projeto principal da API
- **hinos_api.Tests**: Projeto de testes unitários

## Como Usar a Solution

### Abrir a Solution

```bash
# No Visual Studio / Rider / VS Code
# Abra o arquivo hinos_api.sln dentro desta pasta
```

### Compilar a Solution

```bash
cd hinos_api
dotnet build hinos_api.sln
```

### Executar Testes

```bash
cd hinos_api
dotnet test hinos_api.sln
```

### Executar Apenas o Projeto Principal

```bash
cd hinos_api
dotnet run
```

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

## Estrutura de Pastas

```
hinos_api/
├── Data/              # Contextos e Seeders do Entity Framework
├── DTOs/              # Data Transfer Objects
├── Models/            # Entidades do domínio
├── Services/          # Serviços de negócio
├── Tests/             # Testes unitários
├── Properties/        # Configurações de launch
└── Program.cs         # Ponto de entrada da aplicação
```

## Configuração

Veja `appsettings.json` e `appsettings.Development.json` para configurações de conexão e autenticação.

## Documentação

A documentação completa da API está disponível em:
- Swagger UI: `http://localhost:5000/swagger` (quando a aplicação estiver rodando)
- Documentação: `docs/hinos_api.md`
