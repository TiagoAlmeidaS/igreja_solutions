# igreja_solutions

Projetos agrupados para igreja para soluções específicas e mínimas

## Estrutura do Projeto

```
igreja_solutions/
├── hinos_api/          # API .NET 9 para gerenciamento de hinários
├── hinos_web/          # Frontend React para visualização de hinários
├── hinos_mcp/          # Servidor MCP para integração com LLMs
├── igrejaconecta/      # MicroSaaS para automação de avisos via WhatsApp
│   ├── backend/        # FastAPI backend
│   ├── frontend/       # React frontend
│   └── database/       # SQL schemas
├── docs/               # Documentação dos projetos
└── docker-compose.yml  # Orquestração Docker dos serviços
```

## Projetos

### hinos_api
API REST desenvolvida em .NET 9 com Minimal API para CRUD de hinos.
- [Documentação completa](./docs/hinos_api.md)

### hinos_web
Frontend React com Vite para visualização e busca de hinários.
- [Documentação completa](./docs/hinos_web.md)

### hinos_mcp
Servidor MCP (Model Context Protocol) em TypeScript para integração da API com LLMs.
- [Documentação completa](./docs/hinos_mcp.md)

**Nota:** O serviço MCP está configurado no Docker Compose e se comunica com a API via rede interna Docker. Clientes MCP podem se conectar ao container para usar o servidor.

### igrejaconecta
MicroSaaS para automação de avisos e transmissões via WhatsApp (Meta Cloud API).
- [Documentação completa](./docs/igrejaconecta.md)
- Backend: FastAPI + PostgreSQL + Redis + Celery
- Frontend: React + Vite
- Status: Estrutura base criada - Em desenvolvimento

## Executando com Docker Compose

A forma mais simples de executar todo o sistema é usando Docker Compose.

### Pré-requisitos
- Docker
- Docker Compose

### Executar

```bash
# Subir todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

### Acessar
- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- MCP Server: hinos_mcp (comunicação via stdio)

## Executando Localmente (Desenvolvimento)

### API (hinos_api)

```bash
cd hinos_api
dotnet restore
dotnet run
```

API estará disponível em http://localhost:5000

### Frontend (hinos_web)

```bash
cd hinos_web
npm install

# Criar arquivo .env
echo "VITE_API_URL=http://localhost:5000" > .env

npm run dev
```

Frontend estará disponível em http://localhost:3000

## Variáveis de Ambiente

### Frontend
Criar arquivo `hinos_web/.env`:
```
VITE_API_URL=http://localhost:5000
```

### API
A API utiliza `appsettings.json` para configuração. A connection string padrão é:
```
Data Source=data/hymns.db
```

## Banco de Dados

A API utiliza SQLite por padrão. O banco de dados é criado automaticamente na primeira execução e populado com dados iniciais.

**Localização:** `hinos_api/data/hymns.db`

## Documentação

- [API - hinos_api](./docs/hinos_api.md)
- [Frontend - hinos_web](./docs/hinos_web.md)
- [Servidor MCP - hinos_mcp](./docs/hinos_mcp.md)
- [IgrejaConecta - igrejaconecta](./docs/igrejaconecta.md)

## Desenvolvimento

### Estrutura de Commits
Seguir padrão de commits semânticos:
- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Documentação
- `refactor:` Refatoração
- `test:` Testes

### Contribuindo
1. Criar branch a partir de `main`
2. Desenvolver funcionalidade
3. Criar pull request
4. Aguardar revisão

## Licença

Este projeto é privado e de uso interno.