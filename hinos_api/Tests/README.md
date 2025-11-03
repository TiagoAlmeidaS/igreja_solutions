# Testes Unitários - hinos_api

## Visão Geral

Este projeto contém testes unitários completos para a API de Hinários, cobrindo todas as camadas: Services (lógica de negócio), Data (infraestrutura/DbContext) e Endpoints (casos de uso).

## Estrutura

```
Tests/
├── Helpers/
│   ├── HymnFaker.cs          # Gerador de dados fake usando Bogus
│   └── DbContextHelper.cs    # Helper para criar DbContext in-memory
├── Services/
│   └── HymnServiceTests.cs   # Testes da camada de serviços
├── Data/
│   └── HymnsDbContextTests.cs # Testes da camada de infraestrutura
└── Endpoints/
    └── HymnsEndpointTests.cs  # Testes dos casos de uso (endpoints)
```

## Tecnologias

- **xUnit** - Framework de testes
- **FluentAssertions** - Asserções legíveis
- **Bogus** - Geração de dados fake
- **Entity Framework Core InMemory** - Banco de dados em memória para testes

## Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --verbosity normal

# Executar com cobertura (se configurado)
dotnet test /p:CollectCoverage=true
```

## Cobertura de Testes

### Camada de Serviços (HymnService)
- ✅ Mapeamento de Hymn para DTO (MapToDto)
- ✅ Mapeamento de CreateDto para Hymn (MapFromCreateDto)
- ✅ Atualização de Hymn a partir de UpdateDto (UpdateFromDto)
- ✅ Tratamento de campos opcionais (Key, Bpm)
- ✅ Tratamento de versos vazios

**Total: 10 testes**

### Camada de Infraestrutura (HymnsDbContext)
- ✅ Criação de banco de dados
- ✅ Persistência de hinos
- ✅ Persistência de versos relacionados
- ✅ Cascade delete (remoção de versos ao remover hino)
- ✅ Validação de campos obrigatórios
- ✅ Índices (Number, Category, Title)
- ✅ Armazenamento de versos como JSON
- ✅ Atualização de entidades
- ✅ Validação de MaxLength

**Total: 10 testes**

### Casos de Uso (Endpoints)
- ✅ GET /api/hymns - Listagem sem filtros
- ✅ GET /api/hymns?category= - Filtro por categoria
- ✅ GET /api/hymns?search= - Busca por termo
- ✅ GET /api/hymns/{id} - Buscar por ID (sucesso e não encontrado)
- ✅ GET /api/hymns/search - Busca avançada
- ✅ POST /api/hymns - Criação de hino
- ✅ POST /api/hymns - Validações (número, título, categoria vazios)
- ✅ POST /api/hymns - Conflito de número duplicado
- ✅ PUT /api/hymns/{id} - Atualização de hino
- ✅ PUT /api/hymns/{id} - Validações
- ✅ PUT /api/hymns/{id} - Conflito de número usado por outro hino
- ✅ DELETE /api/hymns/{id} - Remoção de hino
- ✅ DELETE /api/hymns/{id} - Cascade delete de versos
- ✅ Busca em versos (conteúdo das letras)

**Total: 18 testes**

### Total: 38 testes

## Dados Fake (Bogus)

O projeto utiliza **Bogus** para gerar dados fake realistas:

- Hinos com números, títulos, categorias válidas
- Versos com tipos e linhas
- Campos opcionais (Key, Bpm)
- Dados em português (locale: pt_BR)

## Executar Testes Específicos

```bash
# Executar apenas testes de serviços
dotnet test --filter "FullyQualifiedName~HymnServiceTests"

# Executar apenas testes de infraestrutura
dotnet test --filter "FullyQualifiedName~HymnsDbContextTests"

# Executar apenas testes de endpoints
dotnet test --filter "FullyQualifiedName~HymnsEndpointTests"
```

## Notas Importantes

1. **EF Core InMemory**: O provider InMemory não aplica todas as restrições de banco de dados (como MaxLength). Para testes mais próximos da realidade, considere usar SQLite em memória.

2. **Isolamento**: Cada teste utiliza um banco de dados in-memory isolado (nome único por Guid) para garantir que não haja interferência entre testes.

3. **Fake Data**: Todos os dados de teste são gerados usando Bogus, garantindo variedade e realismo nos testes.

## Próximos Passos

- Adicionar cobertura de código (Coverlet)
- Adicionar testes de performance
- Adicionar testes de integração HTTP completos
- Configurar pipeline CI/CD com testes
