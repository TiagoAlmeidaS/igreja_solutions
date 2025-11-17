# IgrejaConecta - Backend

API FastAPI seguindo Clean Architecture para automação de avisos e transmissões via WhatsApp.

## Arquitetura

O projeto segue **Clean Architecture** com as seguintes camadas:

- **Domain**: Entidades puras e value objects (sem dependências)
- **Application**: Casos de uso e DTOs
- **Infrastructure**: Repositórios, models SQLAlchemy, serviços externos
- **Presentation**: Endpoints da API e middleware

## Estrutura

```
backend/
├── app/
│   ├── domain/              # Camada de domínio
│   │   ├── entities/        # Entidades de negócio
│   │   └── value_objects/   # Value objects
│   ├── application/         # Camada de aplicação
│   │   ├── use_cases/       # Casos de uso
│   │   ├── dto/             # Data Transfer Objects
│   │   └── interfaces/      # Interfaces (contratos)
│   ├── infrastructure/      # Camada de infraestrutura
│   │   ├── database/        # Repositórios e models
│   │   ├── external/        # Serviços externos
│   │   └── tasks/           # Celery tasks
│   ├── presentation/        # Camada de apresentação
│   │   ├── api/v1/          # Endpoints da API
│   │   └── middleware/      # Middleware (auth, errors)
│   ├── core/                # Configurações centrais
│   ├── migrations/          # Alembic migrations
│   ├── tests/               # Testes
│   └── main.py              # Entry point
├── alembic.ini
├── pytest.ini
├── requirements.txt
└── Dockerfile
```

## Tecnologias

- **FastAPI**: Framework web
- **SQLAlchemy**: ORM
- **Alembic**: Migrations
- **Firebase Admin SDK**: Autenticação
- **Supabase**: Banco de dados PostgreSQL
- **Celery**: Task queue
- **Redis**: Broker para Celery
- **Pytest**: Testes unitários
- **Faker**: Dados fake para testes

## Instalação

```bash
# Criar ambiente virtual
python -m venv venv
source venv/bin/activate  # Linux/Mac
# ou
venv\Scripts\activate  # Windows

# Instalar dependências
pip install -r requirements.txt

# Configurar variáveis de ambiente
cp .env.example .env
# Editar .env com suas configurações
```

## Configuração

Variáveis de ambiente necessárias (`.env`):

```env
# Database (Supabase PostgreSQL)
DATABASE_URL=postgresql://user:pass@host:5432/dbname

# Supabase (opcional)
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-supabase-key

# Redis
REDIS_URL=redis://localhost:6379/0

# Firebase
FIREBASE_PROJECT_ID=your-project-id
FIREBASE_CREDENTIALS_PATH=/path/to/credentials.json
# ou
FIREBASE_CREDENTIALS_JSON=base64_encoded_json

# WhatsApp
WHATSAPP_API_VERSION=v20.0

# Environment
ENVIRONMENT=development
DEBUG=True
```

## Migrations

```bash
# Criar migration
alembic revision --autogenerate -m "Initial migration"

# Aplicar migrations
alembic upgrade head

# Reverter migration
alembic downgrade -1
```

## Executar

```bash
# Desenvolvimento
uvicorn app.main:app --reload

# Produção
uvicorn app.main:app --host 0.0.0.0 --port 8000 --workers 4
```

## Testes

```bash
# Executar todos os testes
pytest

# Com cobertura
pytest --cov=app --cov-report=html

# Apenas testes unitários
pytest app/tests/unit
```

## Docker

```bash
# Build
docker build -t igrejaconecta-backend .

# Run
docker run -p 8000:8000 --env-file .env igrejaconecta-backend
```

## Documentação

- Swagger UI: http://localhost:8000/docs
- ReDoc: http://localhost:8000/redoc

## Autenticação

A API utiliza Firebase Authentication. Todas as rotas (exceto `/health`) requerem um token Firebase no header:

```
Authorization: Bearer <firebase_token>
```

## Endpoints Principais

- `POST /api/v1/church` - Criar igreja
- `GET /api/v1/church/me` - Obter igreja atual
- `POST /api/v1/church/whatsapp/config` - Configurar WhatsApp
- `POST /api/v1/contacts` - Criar contato
- `POST /api/v1/contacts/upload` - Importar contatos CSV
- `POST /api/v1/broadcasts` - Criar transmissão
- `POST /api/v1/broadcasts/{id}/send` - Enviar transmissão
- `GET /api/v1/broadcasts/statistics` - Estatísticas
- `POST /api/v1/templates` - Criar template
- `GET /api/v1/templates` - Listar templates

## Próximos Passos

- [ ] Completar todos os 23 casos de uso
- [ ] Adicionar mais testes unitários (alcançar 80% cobertura)
- [ ] Implementar testes de integração
- [ ] Configurar CI/CD
- [ ] Adicionar logging estruturado
- [ ] Implementar rate limiting
- [ ] Adicionar documentação OpenAPI completa
