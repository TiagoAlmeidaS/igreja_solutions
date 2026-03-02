# Central de Acolhimento

Sistema de gestão de acolhimento para igrejas, permitindo o acompanhamento de pessoas acolhidas e seus cuidadores, com controle por igreja e coordenação centralizada.

## Estrutura do Monorepo

```
central_acolhimento/
├── backend/              # API .NET 9 Minimal API
│   ├── src/              # Código-fonte da API
│   └── tests/            # Testes automatizados
├── web/                  # Frontend React + Vite
├── mobile/               # App React Native + Expo
├── packages/
│   └── shared/           # Tipos e utilitários compartilhados (TS)
├── database/             # Migrations e seeds SQL
├── docs/                 # Documentação completa do projeto
│   ├── requirements.md   # Requisitos funcionais e não-funcionais
│   ├── use-cases.md      # Casos de uso detalhados
│   ├── features.md       # Features e roadmap
│   ├── fixtures.md       # Dados iniciais e seeds
│   ├── architecture.md   # Arquitetura do sistema
│   ├── data-model.md     # Modelo de dados (ERD)
│   ├── api-contracts.md          # Contratos da API REST
│   ├── business-rules.md        # Regras de negócio
│   ├── roles-multitenancy.md    # Roles, permissões e multi-tenancy
│   ├── ui-dashboard.md          # UI: Dashboard Torre de Controle
│   ├── ui-relatorios.md         # UI: Relatórios de Gestão
│   └── ui-cuidadores.md         # UI: Gestão de Cuidadores
└── README.md                    # Este arquivo
```

## Stack Tecnológico

| Camada   | Tecnologia                        |
|----------|-----------------------------------|
| Backend  | .NET 9 Minimal API + EF Core      |
| Banco    | PostgreSQL                         |
| Web      | React + Vite + TypeScript          |
| Mobile   | React Native + Expo + TypeScript   |
| Shared   | TypeScript (tipos compartilhados)  |
| Infra    | Docker + Docker Compose            |

## Conceitos-Chave

- **Pessoa Acolhida**: Indivíduo cadastrado para receber acompanhamento espiritual
- **Cuidador**: Membro da igreja responsável pelo acompanhamento
- **Coordenador**: Líder que supervisiona cuidadores e acolhidos de uma igreja
- **Igreja**: Unidade organizacional que agrupa coordenadores, cuidadores e acolhidos
- **Admin**: Administrador global da plataforma, com acesso multi-igreja

## Roles e Multi-Tenancy

O sistema opera com isolamento de dados por igreja (multi-tenant por filtro):

| Role         | Escopo de Dados                   | Tenant              |
|--------------|-----------------------------------|---------------------|
| Admin        | Global (todas as igrejas)         | Selecionavel        |
| Coordenador  | Sua igreja                        | Fixo (do cadastro)  |
| Cuidador     | Seus acolhidos na sua igreja      | Fixo (do cadastro)  |

Documentação completa: [roles-multitenancy.md](./docs/roles-multitenancy.md)

## Documentação

| Documento                                                | Descrição                            |
|----------------------------------------------------------|--------------------------------------|
| [requirements.md](./docs/requirements.md)                | Requisitos funcionais e não-funcionais|
| [use-cases.md](./docs/use-cases.md)                      | Casos de uso detalhados              |
| [features.md](./docs/features.md)                        | Features e roadmap por fase          |
| [architecture.md](./docs/architecture.md)                | Arquitetura do sistema               |
| [data-model.md](./docs/data-model.md)                    | Modelo de dados (ERD)                |
| [api-contracts.md](./docs/api-contracts.md)              | Contratos da API REST                |
| [business-rules.md](./docs/business-rules.md)            | Regras de negócio                    |
| [fixtures.md](./docs/fixtures.md)                        | Dados iniciais e cenários de teste   |
| [roles-multitenancy.md](./docs/roles-multitenancy.md)    | Roles, permissões e multi-tenancy    |
| [ui-dashboard.md](./docs/ui-dashboard.md)                | UI: Dashboard Torre de Controle      |
| [ui-relatorios.md](./docs/ui-relatorios.md)              | UI: Relatórios de Gestão             |
| [ui-cuidadores.md](./docs/ui-cuidadores.md)              | UI: Gestão de Cuidadores             |

## Status

Em planejamento - Fase de documentação e arquitetura.
