# API de Hinários - hinos_api

## Visão Geral

API REST desenvolvida em .NET 9 com Minimal API para gerenciamento de hinários. Fornece endpoints para operações CRUD completas sobre hinos e versos.

## Tecnologias

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQLite** - Banco de dados (configurável para SQL Server/PostgreSQL)
- **Swagger/OpenAPI** - Documentação interativa da API
- **JWT** - Autenticação via tokens JWT
- **Minimal API** - Abordagem simplificada de construção de APIs

## Estrutura do Projeto

```
hinos_api/
├── Models/          # Entidades do domínio (Hymn, Verse)
├── Data/            # DbContext e seed de dados
├── Services/        # Lógica de negócio e mapeamento
│   └── HymnFormatService.cs  # Serviço de formatação de hinos (texto plano e Holyrics)
├── DTOs/            # Data Transfer Objects (Create, Update, Response)
├── Endpoints/       # Endpoints da API organizados por funcionalidade
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

### POST /api/auth/login
Realiza autenticação na API usando email e senha.

**Body:** LoginRequestDto
```json
{
  "email": "admin@hinario.com",
  "password": "admin123"
}
```

**Resposta:**

- `200 OK` - Login realizado com sucesso
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "1",
    "name": "Administrador",
    "email": "admin@hinario.com"
  }
}
```

- `400 Bad Request` - Email ou senha não fornecidos
- `401 Unauthorized` - Credenciais inválidas
```json
{
  "message": "Credenciais inválidas"
}
```

**Nota:** O token JWT retornado deve ser incluído no header `Authorization: Bearer {token}` em requisições protegidas.

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

### GET /api/hymns/{id}/download/plain
Download do hino em formato texto plano.

**Parâmetros:**
- `id` (int): ID do hino

**Resposta:**
- `200 OK` - Arquivo .txt com o hino em formato texto simples
  - Content-Type: `text/plain;charset=utf-8`
  - Content-Disposition: `attachment; filename="hino-{número}-{título-slug}.txt"`
- `404 Not Found` - Hino não encontrado

**Formato do arquivo:**
O arquivo contém apenas o título do hino seguido dos versos, sem marcadores de tipo. Ideal para copiar e colar em WhatsApp ou outros aplicativos.

**Exemplo:**
```
Graça Maravilhosa

Linha 1 do verso 1
Linha 2 do verso 1

Linha 1 do verso 2
Linha 2 do verso 2
```

### GET /api/hymns/{id}/download/holyrics
Download do hino em formato Holyrics.

**Parâmetros:**
- `id` (int): ID do hino

**Resposta:**
- `200 OK` - Arquivo .txt formatado para importação no Holyrics
  - Content-Type: `text/plain;charset=utf-8`
  - Content-Disposition: `attachment; filename="hino-{número}-{título-slug}.txt"`
- `404 Not Found` - Hino não encontrado

**Formato do arquivo:**
O arquivo está formatado para importação direta no Holyrics, OpenLP e outros softwares de projeção. Inclui marcadores de tipo de verso `[V1]`, `[V2]`, `[R]`, etc., e metadados como Tom e BPM quando disponíveis.

**Estrutura do formato:**
```
#{número} - {título}
{hinário}

[V1]
{linha1}
{linha2}

[V2]
{linha1}
{linha2}

---
Tom: {key} | BPM: {bpm}
```

**Exemplo:**
```
#101 - Graça Maravilhosa
Hinário Adventista do Sétimo Dia

[V1]
Maravilhosa graça
Do meu Salvador

[V2]
Jesus morreu por mim
Naquela cruz

---
Tom: G | BPM: 72
```

**Nota:** O arquivo pode ser importado diretamente no Holyrics através da opção de importação de arquivos de texto. O formato é compatível com OpenLP e outros softwares de projeção que suportam marcadores de tipo de verso.

### GET /health
Health check da API.

**Resposta:** `200 OK` - API está funcionando

## Configuração

### Variáveis de Ambiente

- `ASPNETCORE_ENVIRONMENT`: Ambiente de execução (Development, Production)
- `ConnectionStrings__DefaultConnection`: String de conexão do banco de dados
- `Auth__Email`: Email do usuário administrador (override do appsettings.json)
- `Auth__Password`: Senha do usuário administrador (override do appsettings.json)
- `Auth__JwtSecret`: Chave secreta para assinatura JWT (override do appsettings.json)
- `Auth__JwtExpirationHours`: Horas de expiração do token JWT (override do appsettings.json)

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=data/hymns.db"
  },
  "Auth": {
    "Email": "admin@hinario.com",
    "Password": "admin123",
    "JwtSecret": "minha-chave-secreta-super-segura-com-pelo-menos-32-caracteres-para-jwt",
    "JwtExpirationHours": 24
  }
}
```

**Importante:** 
- Em produção, configure as credenciais via variáveis de ambiente ao invés de deixar no appsettings.json
- O `JwtSecret` deve ter pelo menos 32 caracteres para garantir segurança
- A senha é armazenada em texto plano (conforme configuração solicitada)

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

## Autenticação

A API utiliza autenticação baseada em JWT (JSON Web Tokens). 

### Configuração de Credenciais

As credenciais são configuradas via `appsettings.json` ou variáveis de ambiente:
- Email e senha do administrador são validados no login
- Um token JWT é gerado com expiração configurável (padrão: 24 horas)
- O token contém claims: email, id e nome do usuário

### Uso do Token

Após realizar login, inclua o token no header das requisições:
```
Authorization: Bearer {token}
```

### Endpoint Público

O endpoint `/api/auth/login` é público (não requer autenticação). Todos os outros endpoints podem ser protegidos no futuro.

## Segurança

- CORS configurado para origens específicas
- Validação de entrada nos endpoints
- Sanitização de dados
- Autenticação JWT implementada
- Credenciais configuráveis via variáveis de ambiente

**Recomendações:**
- Em produção, usar variáveis de ambiente para credenciais sensíveis
- Considerar hash de senhas para maior segurança
- Configurar JWT Secret forte (mínimo 32 caracteres)
- Revisar políticas de expiração de tokens

## Melhorias Futuras

- Rate limiting
- Cache de respostas
- Paginação nos endpoints de listagem
- Suporte a múltiplos bancos de dados (SQL Server, PostgreSQL)
- Logging estruturado (Serilog)
- Métricas e observabilidade (OpenTelemetry)
