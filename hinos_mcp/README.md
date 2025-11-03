# hinos_mcp

Servidor MCP (Model Context Protocol) para integração da API `hinos_api` com LLMs.

## Visão Geral

Este projeto implementa uma camada intermediária que expõe todas as rotas da API `hinos_api` como tools do Model Context Protocol, permitindo que LLMs interajam com a API através do protocolo MCP.

## Tecnologias

- **TypeScript** - Linguagem principal
- **@modelcontextprotocol/sdk** - SDK oficial do MCP
- **axios** - Cliente HTTP para comunicação com a API
- **dotenv** - Gerenciamento de variáveis de ambiente
- **jest** - Framework de testes
- **@faker-js/faker** - Geração de dados fake para testes

## Estrutura do Projeto

```
hinos_mcp/
├── src/
│   ├── index.ts              # Entry point do servidor MCP
│   ├── server.ts             # Configuração do servidor MCP
│   ├── tools/
│   │   ├── auth.ts           # Tool de autenticação
│   │   ├── hymns.ts          # Tools de hinos
│   │   └── health.ts         # Tool de health check
│   ├── client/
│   │   └── apiClient.ts      # Cliente HTTP
│   ├── auth/
│   │   └── jwtManager.ts     # Gerenciador de tokens JWT
│   ├── types/
│   │   ├── hymn.ts           # Tipos relacionados a hinos
│   │   └── api.ts            # Tipos da API
│   └── utils/
│       └── validation.ts     # Utilitários de validação
├── tests/                    # Testes unitários
└── package.json
```

## Instalação

```bash
npm install
```

## Configuração

Copie `.env.example` para `.env` e configure:

```env
API_BASE_URL=http://localhost:5000
MCP_SERVER_NAME=hinos_mcp
MCP_SERVER_VERSION=1.0.0
```

## Scripts

```bash
# Desenvolvimento
npm run dev

# Build
npm run build

# Executar
npm start

# Testes
npm test

# Testes com watch
npm run test:watch

# Cobertura de testes
npm run test:coverage
```

## Tools Disponíveis

### auth_login
Realiza autenticação na API usando email e senha.

### health_check
Verifica o status de saúde da API.

### hymns_list
Lista todos os hinos com filtros opcionais (categoria, busca).

### hymns_get
Busca um hino específico por ID.

### hymns_search
Busca hinos por termo.

### hymns_create
Cria um novo hino.

### hymns_update
Atualiza um hino existente.

### hymns_delete
Remove um hino.

## Documentação Completa

Veja a [documentação completa](./docs/hinos_mcp.md) para mais detalhes.

