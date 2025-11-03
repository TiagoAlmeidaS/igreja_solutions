# Servidor MCP - hinos_mcp

## Visão Geral

Servidor MCP (Model Context Protocol) desenvolvido em TypeScript que expõe todas as rotas da API `hinos_api` como tools para integração com LLMs. Este servidor atua como uma camada intermediária que permite que modelos de linguagem interajam com a API através do protocolo MCP.

## Tecnologias

- **TypeScript** - Linguagem principal
- **@modelcontextprotocol/sdk** - SDK oficial do Model Context Protocol
- **axios** - Cliente HTTP para comunicação com hinos_api
- **dotenv** - Gerenciamento de variáveis de ambiente
- **jest** - Framework de testes
- **@faker-js/faker** - Geração de dados fake para testes
- **tsx** - Execução TypeScript

## Estrutura do Projeto

```
hinos_mcp/
├── src/
│   ├── index.ts              # Entry point do servidor MCP
│   ├── server.ts             # Configuração do servidor MCP
│   ├── tools/
│   │   ├── auth.ts           # Tool de autenticação
│   │   ├── hymns.ts          # Tools de hinos (listar, buscar, criar, atualizar, deletar)
│   │   └── health.ts         # Tool de health check
│   ├── client/
│   │   └── apiClient.ts      # Cliente HTTP para comunicação com hinos_api
│   ├── auth/
│   │   └── jwtManager.ts     # Gerenciador de tokens JWT
│   ├── types/
│   │   ├── hymn.ts           # Tipos relacionados a hinos
│   │   └── api.ts            # Tipos da API
│   └── utils/
│       └── validation.ts     # Utilitários de validação
├── tests/
│   ├── tools/
│   │   ├── auth.test.ts
│   │   ├── hymns.test.ts
│   │   └── health.test.ts
│   ├── client/
│   │   └── apiClient.test.ts
│   ├── helpers/
│   │   └── faker.ts          # Dados fake para testes
│   └── setup.ts              # Configuração dos testes
├── package.json
├── tsconfig.json
├── .env.example
└── README.md
```

## Model Context Protocol (MCP)

O MCP é um protocolo desenvolvido pela Anthropic para permitir que LLMs interajam com fontes de dados externas e ferramentas através de um protocolo padronizado. Este servidor implementa um servidor MCP que expõe as rotas da API como "tools" que podem ser chamadas pela LLM.

## Tools Disponíveis

### auth_login

Tool para autenticação na API usando email e senha.

**Endpoint mapeado:** `POST /api/auth/login`

**Parâmetros:**
- `email` (string, obrigatório): Email do usuário
- `password` (string, obrigatório): Senha do usuário

**Resposta:**
- Armazena o token JWT automaticamente
- Retorna dados do usuário autenticado

**Exemplo de uso:**
```json
{
  "name": "auth_login",
  "arguments": {
    "email": "admin@hinario.com",
    "password": "admin123"
  }
}
```

### health_check

Tool para verificar o status de saúde da API.

**Endpoint mapeado:** `GET /health`

**Parâmetros:** Nenhum

**Resposta:**
- Status da API (ok/error)

### hymns_list

Tool para listar todos os hinos cadastrados.

**Endpoint mapeado:** `GET /api/hymns`

**Parâmetros:**
- `category` (string, opcional): Filtrar por categoria (hinario, canticos, suplementar, novos)
- `search` (string, opcional): Termo de busca

**Resposta:**
- Lista de hinos encontrados

**Exemplo de uso:**
```json
{
  "name": "hymns_list",
  "arguments": {
    "category": "hinario",
    "search": "graça"
  }
}
```

### hymns_get

Tool para buscar um hino específico por ID.

**Endpoint mapeado:** `GET /api/hymns/{id}`

**Parâmetros:**
- `id` (number, obrigatório): ID do hino

**Resposta:**
- Detalhes completos do hino

### hymns_search

Tool para buscar hinos por termo.

**Endpoint mapeado:** `GET /api/hymns/search`

**Parâmetros:**
- `term` (string, obrigatório): Termo de busca

**Resposta:**
- Lista de hinos encontrados

### hymns_create

Tool para criar um novo hino.

**Endpoint mapeado:** `POST /api/hymns`

**Parâmetros:**
- `number` (string, obrigatório): Número do hino (deve ser único)
- `title` (string, obrigatório): Título do hino
- `category` (string, obrigatório): Categoria (hinario, canticos, suplementar, novos)
- `hymnBook` (string, obrigatório): Nome do hinário
- `key` (string, opcional): Tom musical
- `bpm` (number, opcional): Batidas por minuto
- `verses` (array, opcional): Lista de versos

**Resposta:**
- Hino criado

### hymns_update

Tool para atualizar um hino existente.

**Endpoint mapeado:** `PUT /api/hymns/{id}`

**Parâmetros:**
- `id` (number, obrigatório): ID do hino
- `number` (string, obrigatório): Número do hino
- `title` (string, obrigatório): Título do hino
- `category` (string, obrigatório): Categoria
- `hymnBook` (string, obrigatório): Nome do hinário
- `key` (string, opcional): Tom musical
- `bpm` (number, opcional): Batidas por minuto
- `verses` (array, opcional): Lista de versos

**Resposta:**
- Hino atualizado

### hymns_delete

Tool para remover um hino.

**Endpoint mapeado:** `DELETE /api/hymns/{id}`

**Parâmetros:**
- `id` (number, obrigatório): ID do hino

**Resposta:**
- Confirmação de remoção

## Autenticação JWT

O servidor MCP gerencia automaticamente a autenticação JWT:

1. Ao chamar `auth_login`, o token JWT é armazenado automaticamente
2. Todas as requisições subsequentes incluem o token no header `Authorization: Bearer {token}`
3. Se uma requisição retornar 401 (Unauthorized), o token é limpo automaticamente

## Configuração

### Variáveis de Ambiente

Criar arquivo `.env`:

```env
# API Configuration
API_BASE_URL=http://localhost:5000

# MCP Server Configuration
MCP_SERVER_NAME=hinos_mcp
MCP_SERVER_VERSION=1.0.0
```

### Instalação

```bash
npm install
```

### Build

```bash
npm run build
```

### Execução

```bash
# Modo desenvolvimento (watch)
npm run dev

# Modo produção
npm start
```

## Executando como Servidor MCP

O servidor MCP comunica-se via stdio (standard input/output). Para usar com uma LLM:

1. Configure o servidor MCP no cliente LLM
2. O servidor estará disponível em `node dist/index.js` (após build)
3. Todas as tools estarão disponíveis para a LLM

## Testes

### Executar Testes

```bash
# Todos os testes
npm test

# Modo watch
npm run test:watch

# Com cobertura
npm run test:coverage
```

### Estrutura de Testes

- Testes unitários para cada tool
- Testes do API client
- Uso de Faker para dados fake
- Mock das dependências externas

## Integração com LLMs

Este servidor pode ser integrado com LLMs que suportam o protocolo MCP, como:

- Claude Desktop (Anthropic)
- Outros clientes MCP compatíveis

### Configuração no Claude Desktop

Adicionar ao arquivo de configuração do Claude:

```json
{
  "mcpServers": {
    "hinos_mcp": {
      "command": "node",
      "args": ["/caminho/para/hinos_mcp/dist/index.js"],
      "env": {
        "API_BASE_URL": "http://localhost:5000"
      }
    }
  }
}
```

## Tratamento de Erros

Todas as tools retornam respostas padronizadas:

```json
{
  "success": true|false,
  "data": {...},
  "error": "mensagem de erro",
  "statusCode": 200|400|401|404|409|500
}
```

## Segurança

- Tokens JWT são armazenados em memória
- Tokens são limpos automaticamente em caso de erro 401
- Validação de entrada em todas as tools
- Comunicação com API usa HTTPS (recomendado em produção)

## Melhorias Futuras

- Suporte a múltiplas instâncias da API
- Cache de respostas
- Rate limiting
- Logging estruturado
- Métricas e observabilidade
- Suporte a HTTP transport (além de stdio)

## Licença

Este projeto é privado e de uso interno.

