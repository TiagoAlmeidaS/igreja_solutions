# Arquitetura - Central de Acolhimento

## 1. Visao Geral

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENTES                                  │
│                                                                  │
│   ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│   │   React Web  │    │ React Native │    │  API Clients │      │
│   │   (Vite)     │    │   (Expo)     │    │  (Swagger)   │      │
│   └──────┬───────┘    └──────┬───────┘    └──────┬───────┘      │
│          │                   │                   │               │
└──────────┼───────────────────┼───────────────────┼───────────────┘
           │                   │                   │
           └───────────────────┼───────────────────┘
                               │
                         HTTPS / JSON
                               │
┌──────────────────────────────┼───────────────────────────────────┐
│                        BACKEND                                    │
│                              │                                    │
│   ┌──────────────────────────▼───────────────────────────────┐   │
│   │              .NET 9 Minimal API                           │   │
│   │                                                           │   │
│   │  ┌─────────┐  ┌──────────┐  ┌────────────┐  ┌────────┐ │   │
│   │  │Endpoints│  │Middleware │  │  Services   │  │  DTOs  │ │   │
│   │  │ (Routes)│  │(Auth,CORS│  │(Business    │  │        │ │   │
│   │  │         │  │ Logging) │  │  Logic)     │  │        │ │   │
│   │  └────┬────┘  └──────────┘  └──────┬─────┘  └────────┘ │   │
│   │       │                            │                     │   │
│   │  ┌────▼────────────────────────────▼─────────────────┐  │   │
│   │  │              Repositories (EF Core)                │  │   │
│   │  └────────────────────────┬───────────────────────────┘  │   │
│   └───────────────────────────┼───────────────────────────────┘   │
│                               │                                    │
│   ┌───────────────────────────▼───────────────────────────────┐   │
│   │                    PostgreSQL                              │   │
│   │  ┌─────────┐ ┌──────────┐ ┌───────────┐ ┌─────────────┐ │   │
│   │  │ Igrejas │ │Acolhidos │ │Cuidadores │ │Acompanham.  │ │   │
│   │  └─────────┘ └──────────┘ └───────────┘ └─────────────┘ │   │
│   └───────────────────────────────────────────────────────────┘   │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

## 2. Arquitetura do Backend (.NET 9)

### Estrutura de Camadas

```
backend/src/
├── CentralAcolhimento.Api/          # Projeto principal (Minimal API)
│   ├── Program.cs                   # Entry point e configuracao
│   ├── Endpoints/                   # Definicao de rotas
│   │   ├── AuthEndpoints.cs
│   │   ├── IgrejaEndpoints.cs
│   │   ├── AcolhidoEndpoints.cs
│   │   ├── CuidadorEndpoints.cs
│   │   └── AcompanhamentoEndpoints.cs
│   ├── Middleware/
│   │   ├── AuthMiddleware.cs
│   │   ├── TenantMiddleware.cs      # Filtro por igreja
│   │   └── ExceptionMiddleware.cs
│   ├── Filters/
│   │   └── TenantFilter.cs          # Query filter global por igreja
│   └── appsettings.json
│
├── CentralAcolhimento.Domain/       # Entidades e regras de dominio
│   ├── Entities/
│   │   ├── Igreja.cs
│   │   ├── Usuario.cs
│   │   ├── Acolhido.cs
│   │   ├── Cuidador.cs
│   │   ├── Acompanhamento.cs
│   │   └── HistoricoMudanca.cs
│   ├── Enums/
│   │   ├── StatusAcompanhamento.cs   # NovoContato, PrimeiraVisita, EmAcompanhamento, Desativada
│   │   ├── EstadoInteresse.cs        # Frio, Morno, Quente
│   │   ├── CrescimentoAlma.cs        # Novo, Crescendo, Firme
│   │   ├── TipoContato.cs            # WhatsApp, Visita, Ligacao, Presencial
│   │   ├── DisponibilidadeCuidador.cs # Sim, Nao
│   │   └── PerfilUsuario.cs          # Admin, Coordenador, Cuidador
│   └── Interfaces/
│       ├── IAcolhidoRepository.cs
│       ├── ICuidadorRepository.cs
│       └── IAcompanhamentoRepository.cs
│
├── CentralAcolhimento.Application/   # Servicos e logica de aplicacao
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── IgrejaService.cs
│   │   ├── AcolhidoService.cs
│   │   ├── CuidadorService.cs
│   │   ├── AcompanhamentoService.cs
│   │   └── CapacidadeService.cs
│   ├── DTOs/
│   │   ├── Requests/
│   │   └── Responses/
│   └── Validators/
│       ├── AcolhidoValidator.cs
│       └── CuidadorValidator.cs
│
├── CentralAcolhimento.Infrastructure/ # Acesso a dados e servicos externos
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/           # Fluent API configs
│   │   │   ├── IgrejaConfiguration.cs
│   │   │   ├── AcolhidoConfiguration.cs
│   │   │   ├── CuidadorConfiguration.cs
│   │   │   └── AcompanhamentoConfiguration.cs
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── AcolhidoRepository.cs
│   │   ├── CuidadorRepository.cs
│   │   └── AcompanhamentoRepository.cs
│   └── Security/
│       ├── JwtTokenGenerator.cs
│       └── PasswordHasher.cs
│
└── CentralAcolhimento.Tests/         # Testes
    ├── Unit/
    ├── Integration/
    └── Fixtures/
```

### Padrao de Design

- **Minimal API** com extensoes de endpoints agrupados
- **Repository Pattern** para acesso a dados
- **Service Layer** para logica de negocio
- **DTOs** para transferencia de dados (Request/Response separados)
- **FluentValidation** para validacao de entrada
- **Global Query Filters** para multi-tenancy por igreja

## 3. Arquitetura do Frontend Web (React)

```
web/
├── public/
├── src/
│   ├── api/                    # Clients HTTP (axios/fetch)
│   │   ├── client.ts           # Configuracao base (interceptors, auth)
│   │   ├── acolhidos.ts
│   │   ├── cuidadores.ts
│   │   └── acompanhamentos.ts
│   ├── components/             # Componentes reutilizaveis
│   │   ├── ui/                 # Botoes, inputs, cards, modals
│   │   ├── layout/             # Header, Sidebar, Footer
│   │   └── charts/             # Graficos do dashboard
│   ├── features/               # Modulos por funcionalidade
│   │   ├── auth/
│   │   ├── dashboard/
│   │   ├── acolhidos/
│   │   ├── cuidadores/
│   │   └── acompanhamentos/
│   ├── hooks/                  # Custom hooks
│   ├── contexts/               # React contexts (auth, tenant)
│   ├── types/                  # TypeScript types (importados de shared)
│   ├── utils/                  # Utilitarios
│   ├── routes/                 # Definicao de rotas
│   ├── App.tsx
│   └── main.tsx
├── package.json
├── vite.config.ts
├── tsconfig.json
└── tailwind.config.js
```

## 4. Arquitetura do Mobile (React Native + Expo)

```
mobile/
├── app/                        # Expo Router (file-based routing)
│   ├── (auth)/                 # Grupo de rotas de autenticacao
│   │   ├── login.tsx
│   │   └── _layout.tsx
│   ├── (app)/                  # Grupo de rotas autenticadas
│   │   ├── (tabs)/
│   │   │   ├── index.tsx       # Home / Meus Acolhidos
│   │   │   ├── registro.tsx    # Registro rapido
│   │   │   └── perfil.tsx      # Perfil do cuidador
│   │   ├── acolhido/
│   │   │   └── [id].tsx        # Detalhe do acolhido
│   │   └── _layout.tsx
│   └── _layout.tsx             # Root layout
├── components/
├── hooks/
├── api/
├── types/
├── constants/
├── app.json
├── package.json
└── tsconfig.json
```

## 5. Pacote Compartilhado

```
packages/shared/
├── src/
│   ├── types/
│   │   ├── acolhido.ts         # Interface IAcolhido, enums
│   │   ├── cuidador.ts         # Interface ICuidador
│   │   ├── acompanhamento.ts   # Interface IAcompanhamento
│   │   ├── igreja.ts           # Interface IIgreja
│   │   └── auth.ts             # Interface IUsuario, tokens
│   ├── enums/
│   │   ├── status.ts           # StatusAcompanhamento
│   │   ├── interesse.ts        # EstadoInteresse
│   │   ├── crescimento.ts      # CrescimentoAlma
│   │   └── contato.ts          # TipoContato
│   ├── validators/
│   │   ├── whatsapp.ts         # Validacao de formato WhatsApp
│   │   └── common.ts           # Validacoes comuns
│   └── index.ts
├── package.json
└── tsconfig.json
```

## 6. Multi-Tenancy

A estrategia de multi-tenancy e por **filtro no banco de dados**:

- Todas as entidades principais possuem `IgrejaId`.
- Um **Global Query Filter** no EF Core filtra automaticamente por `IgrejaId`.
- O `IgrejaId` e extraido do token JWT do usuario autenticado.
- O `TenantMiddleware` injeta o `IgrejaId` no contexto da requisicao.

```csharp
// Exemplo de Global Query Filter
modelBuilder.Entity<Acolhido>()
    .HasQueryFilter(a => a.IgrejaId == _tenantService.IgrejaId);
```

## 7. Seguranca

```
Fluxo de Autenticacao:
1. POST /api/auth/login (email + senha)
2. Backend valida credenciais
3. Retorna { accessToken (15min), refreshToken (7d) }
4. Cliente envia Authorization: Bearer {accessToken}
5. TenantMiddleware extrai IgrejaId do token
6. Queries automaticamente filtradas por IgrejaId
```

## 8. Infraestrutura (Docker)

```yaml
# Servicos planejados
- central-acolhimento-api    # .NET 9 API
- central-acolhimento-web    # React (nginx)
- central-acolhimento-db     # PostgreSQL
```
