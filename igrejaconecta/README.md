# IgrejaConecta

MicroSaaS para Automação de Avisos e Transmissões via WhatsApp

## Visão Geral

Solução completa para igrejas gerenciarem envio de avisos, transmissões ao vivo e agendamento de mensagens via WhatsApp oficial (Meta Cloud API).

## Estrutura do Projeto

```
igrejaconecta/
├── backend/          # FastAPI backend
├── frontend/         # React frontend
├── database/         # SQL schemas
└── docker-compose.yml
```

## Tecnologias

- **Backend:** Python 3.11, FastAPI, SQLAlchemy, PostgreSQL, Redis, Celery
- **Frontend:** React 18, Vite, Axios
- **Infraestrutura:** Docker, Docker Compose

## Pré-requisitos

- Docker e Docker Compose
- Python 3.11+ (para desenvolvimento local)
- Node.js 20+ (para desenvolvimento local)

## Executando com Docker Compose

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
- Backend API: http://localhost:8000
- Swagger: http://localhost:8000/docs
- PostgreSQL: localhost:5433
- Redis: localhost:6379

## Desenvolvimento Local

### Backend

```bash
cd backend
python -m venv venv
source venv/bin/activate  # Linux/Mac
pip install -r requirements.txt
cp .env.example .env
# Editar .env com suas configurações
uvicorn app.main:app --reload
```

### Frontend

```bash
cd frontend
npm install
cp .env.example .env
# Editar .env com suas configurações
npm run dev
```

## Documentação

Documentação completa disponível em: [docs/igrejaconecta.md](../docs/igrejaconecta.md)

## Status do Projeto

**Fase Atual:** Estrutura base criada

**Próximos Passos:**
1. Implementação do backend com FastAPI
2. Implementação do frontend com React
3. Integração com Meta WhatsApp Cloud API
4. Testes unitários e de integração
5. Deploy em produção

## Licença

Projeto privado - Uso interno

