# API de Hinários - hinos_api

## Visão Geral

API REST desenvolvida em .NET 9 com Minimal API para gerenciamento de hinários. Fornece endpoints para operações CRUD completas sobre hinos e versos.

## Tecnologias

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQLite** - Banco de dados (configurável para SQL Server/PostgreSQL)
- **Swagger/OpenAPI** - Documentação interativa da API
- **Minimal API** - Abordagem simplificada de construção de APIs

## Estrutura do Projeto

```
hinos_api/
├── Models/          # Entidades do domínio (Hymn, Verse)
├── Data/            # DbContext e seed de dados
├── Services/        # Lógica de negócio e mapeamento
├── DTOs/            # Data Transfer Objects (Create, Update, Response)
├── Program.cs       # Configuração e endpoints da API
└── appsettings.json # Configurações da aplicação
```

## Modelo de Dados

### Hymn (Hino)
- `Id` (int): Identificador único
- `Number` (string): Número do hino (ex: "101", "S12")
- `Title` (string): Título do hino
- `Category` (string): Categoria (hinario, canticos, suplementar, novos)
- `HymnBook` (string): Nome do hinário
- `Key` (string?): Tom musical (opcional)
- `Bpm` (int?): Batidas por minuto (opcional)
- `Verses` (List<Verse>): Lista de versos do hino

### Verse (Verso)
- `Id` (int): Identificador único
- `Type` (string): Tipo do verso (V1, V2, V3, V4, R, Ponte, C)
- `Lines` (List<string>): Linhas do verso (armazenadas como JSON)
- `HymnId` (int): ID do hino pai

## Endpoints da API

### GET /api/hymns
Lista todos os hinos com filtros opcionais.

**Query Parameters:**
- `category` (opcional): Filtrar por categoria (hinario, canticos, suplementar, novos)
- `search` (opcional): Buscar por termo (busca em número, título, hinário e letras)

**Resposta:** `200 OK` - Lista de hinos

### GET /api/hymns/{id}
Busca um hino específico por ID.

**Parâmetros:**
- `id` (int): ID do hino

**Resposta:** 
- `200 OK` - Dados do hino
- `404 Not Found` - Hino não encontrado

### GET /api/hymns/search
Busca hinos por termo de busca.

**Query Parameters:**
- `term` (obrigatório): Termo de busca

**Resposta:** `200 OK` - Lista de hinos encontrados

### POST /api/hymns
Cria um novo hino.

**Body:** CreateHymnDto
```json
{
  "number": "101",
  "title": "Graça Maravilhosa",
  "category": "hinario",
  "hymnBook": "Hinário Adventista do Sétimo Dia",
  "key": "G",
  "bpm": 72,
  "verses": [
    {
      "type": "V1",
      "lines": ["Linha 1", "Linha 2"]
    }
  ]
}
```

**Resposta:**
- `201 Created` - Hino criado
- `400 Bad Request` - Dados inválidos
- `409 Conflict` - Número de hino já existe

### PUT /api/hymns/{id}
Atualiza um hino existente.

**Parâmetros:**
- `id` (int): ID do hino

**Body:** UpdateHymnDto (mesma estrutura do CreateHymnDto)

**Resposta:**
- `200 OK` - Hino atualizado
- `400 Bad Request` - Dados inválidos
- `404 Not Found` - Hino não encontrado
- `409 Conflict` - Número de hino já existe em outro hino

### DELETE /api/hymns/{id}
Remove um hino.

**Parâmetros:**
- `id` (int): ID do hino

**Resposta:**
- `204 No Content` - Hino removido
- `404 Not Found` - Hino não encontrado

### GET /health
Health check da API.

**Resposta:** `200 OK` - API está funcionando

## Configuração

### Variáveis de Ambiente

- `ASPNETCORE_ENVIRONMENT`: Ambiente de execução (Development, Production)
- `ConnectionStrings__DefaultConnection`: String de conexão do banco de dados

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=data/hymns.db"
  }
}
```

## Executando Localmente

### Pré-requisitos
- .NET 9 SDK
- SQLite (incluído no .NET)

### Passos

1. Navegar até a pasta do projeto:
```bash
cd hinos_api
```

2. Restaurar dependências:
```bash
dotnet restore
```

3. Executar a aplicação:
```bash
dotnet run
```

4. Acessar:
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

## Docker

### Build da imagem:
```bash
docker build -t hinos_api .
```

### Executar container:
```bash
docker run -p 5000:8080 -v $(pwd)/data:/app/data hinos_api
```

## Banco de Dados

A API utiliza SQLite por padrão, com o banco de dados localizado em `data/hymns.db`.

O banco é criado automaticamente na primeira execução e populado com dados iniciais através do `DbSeeder`.

### Migrations

Migrations são aplicadas automaticamente via `EnsureCreated()`. Para ambientes de produção, considere usar migrations explícitas.

## CORS

CORS está configurado para permitir requisições de:
- http://localhost:3000
- http://localhost:5173
- http://localhost:4173

Para produção, ajuste as origens permitidas no `Program.cs`.

## Logging

Logging configurado via ASP.NET Core logging padrão. Níveis:
- Development: Information
- Production: Warning

## Segurança

- CORS configurado para origens específicas
- Validação de entrada nos endpoints
- Sanitização de dados

## Melhorias Futuras

- Autenticação e autorização (JWT)
- Rate limiting
- Cache de respostas
- Paginação nos endpoints de listagem
- Suporte a múltiplos bancos de dados (SQL Server, PostgreSQL)
- Logging estruturado (Serilog)
- Métricas e observabilidade (OpenTelemetry)
