# Roles e Multi-Tenancy - Central de Acolhimento

## 1. Visao Geral

O sistema opera com dois conceitos complementares que se cruzam em toda a aplicacao:

- **Roles (Papeis)**: definem **o que** o usuario pode fazer.
- **Multi-Tenancy**: define **sobre quais dados** o usuario pode agir.

```
┌──────────────────────────────────────────────────────────────┐
│                        ADMIN                                  │
│  Escopo: GLOBAL (todas as igrejas)                           │
│  Sem tenant fixo - pode alternar entre igrejas               │
│                                                               │
│  ┌──────────────────────────────────────────────────────┐    │
│  │              COORDENADOR                              │    │
│  │  Escopo: TENANT (sua igreja)                          │    │
│  │  Tenant fixo = igreja_id do cadastro                  │    │
│  │                                                        │    │
│  │  ┌──────────────────────────────────────────────┐     │    │
│  │  │            CUIDADOR                           │     │    │
│  │  │  Escopo: TENANT + OWNERSHIP (seus acolhidos)  │     │    │
│  │  │  Tenant fixo = igreja_id do cadastro           │     │    │
│  │  │  Dados restritos = acolhidos atribuidos a ele  │     │    │
│  │  └──────────────────────────────────────────────┘     │    │
│  └──────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────┘
```

---

## 2. Definicao de Roles

### 2.1 Admin

| Atributo         | Valor                                       |
|------------------|---------------------------------------------|
| Escopo de dados  | Global - todas as igrejas                   |
| `igreja_id`      | NULL no cadastro (acesso multi-igreja)      |
| Tenant ativo     | Pode selecionar qualquer igreja como contexto|
| Objetivo         | Gerenciar a plataforma como um todo          |

**Capacidades exclusivas:**
- Criar, editar e desativar igrejas
- Criar e gerenciar coordenadores
- Visualizar dashboard consolidado (cross-tenant)
- Acessar dados de qualquer igreja (alternando contexto)
- Gerenciar configuracoes globais do sistema

**Restricoes:**
- Nao registra acompanhamentos diretamente (nao e cuidador)
- Para operar dentro de uma igreja, deve selecionar o tenant ativo

### 2.2 Coordenador

| Atributo         | Valor                                            |
|------------------|--------------------------------------------------|
| Escopo de dados  | Sua igreja (tenant fixo)                         |
| `igreja_id`      | Preenchido no cadastro, imutavel                 |
| Tenant ativo     | Sempre a igreja vinculada, sem troca             |
| Objetivo         | Gerenciar acolhidos e cuidadores de sua igreja   |

**Capacidades:**
- Cadastrar, editar e desativar cuidadores (da sua igreja)
- Cadastrar, editar e desativar acolhidos (da sua igreja)
- Atribuir e reatribuir acolhidos a cuidadores
- Ajustar capacidade maxima dos cuidadores
- Registrar acompanhamentos de qualquer acolhido da igreja
- Visualizar dashboard da igreja
- Gerar relatorios da igreja
- Alterar status, interesse e crescimento de acolhidos

**Restricoes:**
- Nao pode ver ou acessar dados de outras igrejas
- Nao pode criar ou editar igrejas
- Nao pode criar ou editar coordenadores
- Nao pode alterar seu proprio `igreja_id`

### 2.3 Cuidador

| Atributo         | Valor                                                   |
|------------------|---------------------------------------------------------|
| Escopo de dados  | Seus acolhidos dentro da sua igreja                     |
| `igreja_id`      | Preenchido no cadastro, imutavel                        |
| Tenant ativo     | Sempre a igreja vinculada, sem troca                    |
| Ownership        | Filtro adicional: somente `acolhidos.cuidador_id = eu`  |
| Objetivo         | Acompanhar as pessoas atribuidas a ele                   |

**Capacidades:**
- Visualizar lista dos seus acolhidos atribuidos
- Visualizar perfil e historico dos seus acolhidos
- Registrar acompanhamentos dos seus acolhidos
- Alterar interesse, crescimento e status dos seus acolhidos
- Visualizar seu proprio perfil e capacidade

**Restricoes:**
- Nao pode ver acolhidos de outros cuidadores
- Nao pode cadastrar novos acolhidos
- Nao pode atribuir/reatribuir acolhidos
- Nao pode alterar sua propria capacidade maxima
- Nao pode acessar dashboard ou relatorios
- Nao pode gerenciar outros cuidadores

---

## 3. Modelo Multi-Tenant

### 3.1 Estrategia: Banco Compartilhado com Filtro por Coluna

Todas as igrejas compartilham o mesmo banco de dados. O isolamento e feito por uma coluna `igreja_id` presente em todas as tabelas de dominio.

```
┌─────────────────────────────────────────────────┐
│              PostgreSQL (unico)                  │
│                                                   │
│  ┌─────────────────────────────────────────────┐ │
│  │  Tabela: acolhidos                           │ │
│  │  ┌──────────┬──────────────────────────────┐ │ │
│  │  │igreja_id │ dados...                     │ │ │
│  │  ├──────────┼──────────────────────────────┤ │ │
│  │  │ igreja_A │ Joao, Quente, Crescendo...   │ │ │
│  │  │ igreja_A │ Maria, Frio, Novo...         │ │ │
│  │  │ igreja_B │ Pedro, Morno, Crescendo...   │ │ │
│  │  │ igreja_B │ Ana, Quente, Firme...        │ │ │
│  │  └──────────┴──────────────────────────────┘ │ │
│  └─────────────────────────────────────────────┘ │
│                                                   │
│  Coordenador da igreja_A ve SOMENTE:             │
│  → Joao, Maria                                    │
│                                                   │
│  Coordenador da igreja_B ve SOMENTE:             │
│  → Pedro, Ana                                     │
└─────────────────────────────────────────────────┘
```

### 3.2 Tabelas com Filtro de Tenant

| Tabela              | Coluna `igreja_id` | Filtro Global (EF Core) |
|---------------------|--------------------|-------------------------|
| igrejas             | N/A (e o tenant)   | Nao                     |
| usuarios            | Sim (FK)           | Sim                     |
| cuidadores          | Sim (FK)           | Sim                     |
| acolhidos           | Sim (FK)           | Sim                     |
| acompanhamentos     | Sim (FK)           | Sim                     |
| historico_mudancas  | Via acolhido_id    | Sim (join)              |

### 3.3 Resolucao do Tenant

O tenant e resolvido em cada requisicao HTTP atraves do JWT:

```
Requisicao HTTP
     │
     ▼
┌──────────────────────┐
│ AuthMiddleware        │  1. Valida o JWT
│ (Bearer Token)       │  2. Extrai claims do usuario
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ TenantMiddleware      │  3. Le claim "igreja_id" do token
│                       │  4. Le claim "perfil" do token
│                       │  5. Se Admin e sem header X-Tenant-Id:
│                       │     → tenant = null (acesso global)
│                       │  6. Se Admin e com header X-Tenant-Id:
│                       │     → tenant = header value
│                       │  7. Se Coordenador ou Cuidador:
│                       │     → tenant = igreja_id do token (fixo)
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ ITenantService        │  8. Armazena IgrejaId resolvido
│ (Scoped DI)          │     no contexto da requisicao
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ AppDbContext          │  9. Global Query Filters usam
│ (EF Core)            │     ITenantService.IgrejaId
└──────────────────────┘
```

### 3.4 JWT Claims

O token JWT contem as seguintes claims relevantes para roles e tenant:

```json
{
  "sub": "aaaa0002-0000-0000-0000-000000000002",
  "nome": "Pastor Carlos Silva",
  "email": "carlos@igrejabatista.com",
  "perfil": "Coordenador",
  "igreja_id": "11111111-1111-1111-1111-111111111111",
  "igreja_nome": "Igreja Batista Central",
  "iat": 1740000000,
  "exp": 1740000900
}
```

| Claim        | Tipo   | Descricao                                        |
|--------------|--------|--------------------------------------------------|
| `sub`        | UUID   | ID do usuario                                    |
| `nome`       | string | Nome do usuario                                  |
| `email`      | string | Email do usuario                                 |
| `perfil`     | string | Role: "Admin", "Coordenador" ou "Cuidador"       |
| `igreja_id`  | UUID?  | ID da igreja (null para Admin)                   |
| `igreja_nome`| string?| Nome da igreja (null para Admin)                 |
| `cuidador_id`| UUID?  | ID do cuidador (somente para perfil Cuidador)    |

---

## 4. Politicas de Autorizacao

### 4.1 Definicao das Policies (.NET)

```csharp
// Program.cs - Configuracao de politicas
builder.Services.AddAuthorization(options =>
{
    // Politica: somente Admin
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("perfil", "Admin"));

    // Politica: Admin ou Coordenador
    options.AddPolicy("AdminOrCoordenador", policy =>
        policy.RequireClaim("perfil", "Admin", "Coordenador"));

    // Politica: qualquer usuario autenticado (todos os perfis)
    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());

    // Politica: somente Cuidador (ou Coordenador/Admin)
    options.AddPolicy("PodeRegistrarAcompanhamento", policy =>
        policy.RequireClaim("perfil", "Admin", "Coordenador", "Cuidador"));
});
```

### 4.2 Mapeamento de Endpoints por Policy

| Endpoint                                    | Policy                      | Filtro adicional              |
|---------------------------------------------|-----------------------------|-------------------------------|
| `POST /api/v1/auth/login`                   | Anonimo                     | -                             |
| `POST /api/v1/auth/refresh`                 | Anonimo                     | -                             |
| **Igrejas**                                 |                             |                               |
| `GET /api/v1/igrejas`                       | AdminOnly                   | -                             |
| `POST /api/v1/igrejas`                      | AdminOnly                   | -                             |
| `PUT /api/v1/igrejas/{id}`                  | AdminOnly                   | -                             |
| `DELETE /api/v1/igrejas/{id}`               | AdminOnly                   | -                             |
| **Coordenadores**                           |                             |                               |
| `GET /api/v1/coordenadores`                 | AdminOnly                   | -                             |
| `POST /api/v1/coordenadores`               | AdminOnly                   | -                             |
| **Cuidadores**                              |                             |                               |
| `GET /api/v1/cuidadores`                    | AdminOrCoordenador          | Tenant filter                 |
| `POST /api/v1/cuidadores`                   | AdminOrCoordenador          | Tenant filter                 |
| `PUT /api/v1/cuidadores/{id}`               | AdminOrCoordenador          | Tenant filter                 |
| `PATCH /api/v1/cuidadores/{id}/*`           | AdminOrCoordenador          | Tenant filter                 |
| **Acolhidos**                               |                             |                               |
| `GET /api/v1/acolhidos`                     | AdminOrCoordenador          | Tenant filter                 |
| `GET /api/v1/acolhidos/{id}`                | Authenticated               | Tenant + Ownership filter     |
| `POST /api/v1/acolhidos`                    | AdminOrCoordenador          | Tenant filter                 |
| `PUT /api/v1/acolhidos/{id}`                | AdminOrCoordenador          | Tenant filter                 |
| `PATCH /api/v1/acolhidos/{id}/*`            | Authenticated               | Tenant + Ownership filter     |
| **Acompanhamentos**                         |                             |                               |
| `GET /api/v1/acolhidos/{id}/acompanhamentos`| Authenticated              | Tenant + Ownership filter     |
| `POST /api/v1/acolhidos/{id}/acompanhamentos`| PodeRegistrarAcompanhamento| Tenant + Ownership filter    |
| **Meus Acolhidos (Mobile)**                 |                             |                               |
| `GET /api/v1/meus-acolhidos`               | Cuidador*                   | Ownership filter (cuidador_id)|
| **Dashboard**                               |                             |                               |
| `GET /api/v1/dashboard`                     | AdminOrCoordenador          | Tenant filter                 |

*Coordenador e Admin tambem podem acessar, mas o endpoint retorna dados filtrados pelo cuidador_id do token.

### 4.3 Ownership Filter (Filtro de Propriedade)

Para o perfil **Cuidador**, alem do filtro de tenant, existe um filtro de ownership:

```csharp
// Exemplo de Ownership Filter no Service
public async Task<AcolhidoResponse?> GetAcolhidoAsync(Guid id, ClaimsPrincipal user)
{
    var query = _context.Acolhidos.Where(a => a.Id == id);

    // Tenant filter ja aplicado via Global Query Filter

    // Ownership filter para Cuidador
    if (user.GetPerfil() == PerfilUsuario.Cuidador)
    {
        var cuidadorId = user.GetCuidadorId();
        query = query.Where(a => a.CuidadorId == cuidadorId);
    }

    return await query.FirstOrDefaultAsync();
}
```

Fluxo de decisao:

```
Requisicao para GET /api/v1/acolhidos/{id}
     │
     ▼
┌──────────────────┐
│ E Admin?          │──Sim──► Retorna acolhido (qualquer igreja)
└────────┬─────────┘
         │ Nao
         ▼
┌──────────────────┐
│ E Coordenador?    │──Sim──► Retorna acolhido (somente da sua igreja)
└────────┬─────────┘         [Tenant filter]
         │ Nao
         ▼
┌──────────────────┐
│ E Cuidador?       │──Sim──► Retorna acolhido (somente atribuido a ele)
└────────┬─────────┘         [Tenant filter + Ownership filter]
         │ Nao
         ▼
       403 Forbidden
```

---

## 5. Implementacao Tecnica no Backend

### 5.1 ITenantService

```csharp
public interface ITenantService
{
    Guid? IgrejaId { get; }
    void SetTenant(Guid? igrejaId);
}

public class TenantService : ITenantService
{
    public Guid? IgrejaId { get; private set; }

    public void SetTenant(Guid? igrejaId)
    {
        IgrejaId = igrejaId;
    }
}

// Registro no DI (Program.cs)
builder.Services.AddScoped<ITenantService, TenantService>();
```

### 5.2 TenantMiddleware

```csharp
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var perfil = context.User.FindFirst("perfil")?.Value;
            var igrejaIdClaim = context.User.FindFirst("igreja_id")?.Value;

            if (perfil == "Admin")
            {
                // Admin: usa header X-Tenant-Id se fornecido
                if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader)
                    && Guid.TryParse(tenantHeader, out var tenantId))
                {
                    tenantService.SetTenant(tenantId);
                }
                // Senao, tenant = null (acesso global, sem filtro)
            }
            else if (Guid.TryParse(igrejaIdClaim, out var igrejaId))
            {
                // Coordenador e Cuidador: tenant fixo do token
                tenantService.SetTenant(igrejaId);
            }
        }

        await _next(context);
    }
}
```

### 5.3 Global Query Filters (EF Core)

```csharp
public class AppDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global filters: somente aplicados quando tenant esta definido
        modelBuilder.Entity<Acolhido>()
            .HasQueryFilter(a => _tenantService.IgrejaId == null
                || a.IgrejaId == _tenantService.IgrejaId);

        modelBuilder.Entity<Cuidador>()
            .HasQueryFilter(c => _tenantService.IgrejaId == null
                || c.IgrejaId == _tenantService.IgrejaId);

        modelBuilder.Entity<Acompanhamento>()
            .HasQueryFilter(ac => _tenantService.IgrejaId == null
                || ac.IgrejaId == _tenantService.IgrejaId);

        modelBuilder.Entity<Usuario>()
            .HasQueryFilter(u => _tenantService.IgrejaId == null
                || u.IgrejaId == _tenantService.IgrejaId);
    }
}
```

Comportamento:
- Quando `IgrejaId == null` (Admin sem header): filtro nao se aplica, retorna tudo.
- Quando `IgrejaId` tem valor: filtra automaticamente por igreja.

### 5.4 Authorization Handler Customizado (Ownership)

```csharp
public class OwnershipRequirement : IAuthorizationRequirement { }

public class OwnershipHandler : AuthorizationHandler<OwnershipRequirement, Acolhido>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnershipRequirement requirement,
        Acolhido acolhido)
    {
        var perfil = context.User.FindFirst("perfil")?.Value;

        // Admin e Coordenador: acesso total (dentro do tenant)
        if (perfil == "Admin" || perfil == "Coordenador")
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Cuidador: somente seus acolhidos
        if (perfil == "Cuidador")
        {
            var cuidadorIdClaim = context.User.FindFirst("cuidador_id")?.Value;
            if (Guid.TryParse(cuidadorIdClaim, out var cuidadorId)
                && acolhido.CuidadorId == cuidadorId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
```

---

## 6. Implementacao no Frontend (Web e Mobile)

### 6.1 Contexto de Autenticacao

```typescript
// contexts/AuthContext.ts
interface AuthState {
  usuario: {
    id: string;
    nome: string;
    email: string;
    perfil: 'Admin' | 'Coordenador' | 'Cuidador';
    igrejaId: string | null;
    igrejaNome: string | null;
    cuidadorId: string | null;
  } | null;
  accessToken: string | null;
  isAuthenticated: boolean;
}
```

### 6.2 Guards de Rota

```typescript
// guards/RoleGuard.tsx
interface RoleGuardProps {
  allowed: ('Admin' | 'Coordenador' | 'Cuidador')[];
  children: React.ReactNode;
  fallback?: React.ReactNode; // Exibe quando nao autorizado
}

function RoleGuard({ allowed, children, fallback }: RoleGuardProps) {
  const { usuario } = useAuth();

  if (!usuario || !allowed.includes(usuario.perfil)) {
    return fallback ?? <Navigate to="/unauthorized" />;
  }

  return <>{children}</>;
}

// Uso nas rotas:
<Route path="/igrejas" element={
  <RoleGuard allowed={['Admin']}>
    <IgrejasPage />
  </RoleGuard>
} />

<Route path="/dashboard" element={
  <RoleGuard allowed={['Admin', 'Coordenador']}>
    <DashboardPage />
  </RoleGuard>
} />

<Route path="/meus-acolhidos" element={
  <RoleGuard allowed={['Cuidador']}>
    <MeusAcolhidosPage />
  </RoleGuard>
} />
```

### 6.3 Navegacao Condicional por Role

```typescript
// Itens do menu lateral baseados no perfil
function getMenuItems(perfil: string): MenuItem[] {
  const menus: Record<string, MenuItem[]> = {
    Admin: [
      { label: 'Dashboard Global', path: '/dashboard', icon: 'chart' },
      { label: 'Igrejas', path: '/igrejas', icon: 'church' },
      { label: 'Coordenadores', path: '/coordenadores', icon: 'users' },
      { label: 'Cuidadores', path: '/cuidadores', icon: 'heart' },
      { label: 'Acolhidos', path: '/acolhidos', icon: 'people' },
    ],
    Coordenador: [
      { label: 'Dashboard', path: '/dashboard', icon: 'chart' },
      { label: 'Cuidadores', path: '/cuidadores', icon: 'heart' },
      { label: 'Acolhidos', path: '/acolhidos', icon: 'people' },
      { label: 'Atribuicoes', path: '/atribuicoes', icon: 'link' },
      { label: 'Relatorios', path: '/relatorios', icon: 'file' },
    ],
    Cuidador: [
      { label: 'Meus Acolhidos', path: '/meus-acolhidos', icon: 'people' },
      { label: 'Registrar Contato', path: '/registrar', icon: 'plus' },
      { label: 'Meu Perfil', path: '/perfil', icon: 'user' },
    ],
  };

  return menus[perfil] ?? [];
}
```

### 6.4 HTTP Client com Tenant Header (Admin)

```typescript
// api/client.ts
const apiClient = axios.create({ baseURL: '/api/v1' });

apiClient.interceptors.request.use((config) => {
  const { accessToken, usuario, tenantAtivo } = useAuthStore.getState();

  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }

  // Admin: envia o tenant selecionado via header
  if (usuario?.perfil === 'Admin' && tenantAtivo) {
    config.headers['X-Tenant-Id'] = tenantAtivo;
  }

  return config;
});
```

---

## 7. Fluxos por Role

### 7.1 Fluxo do Admin

```
Login
  │
  ▼
Dashboard Global (metricas de todas as igrejas)
  │
  ├──► Gerenciar Igrejas (CRUD)
  │       └──► Ao criar igreja, obrigatoriamente criar um Coordenador
  │
  ├──► Gerenciar Coordenadores
  │       └──► Vincular coordenador a uma igreja
  │
  ├──► Selecionar Igreja (troca de contexto tenant)
  │       └──► Ao selecionar, ve dados como se fosse Coordenador daquela igreja
  │
  └──► Relatorios Cross-Tenant
          └──► Comparativo entre igrejas
```

### 7.2 Fluxo do Coordenador

```
Login
  │
  ▼
Dashboard da Igreja (metricas da sua igreja)
  │
  ├──► Gerenciar Cuidadores
  │       ├──► Cadastrar novo cuidador (cria usuario + cuidador)
  │       ├──► Ajustar capacidade
  │       └──► Ativar/Desativar disponibilidade
  │
  ├──► Gerenciar Acolhidos
  │       ├──► Cadastrar novo acolhido
  │       ├──► Atribuir a cuidador
  │       ├──► Reatribuir cuidador
  │       ├──► Alterar status/interesse/crescimento
  │       └──► Desativar (com motivo)
  │
  ├──► Acompanhamentos
  │       └──► Visualizar todos os acompanhamentos da igreja
  │
  └──► Relatorios
          ├──► Por status / interesse / crescimento
          ├──► Capacidade dos cuidadores
          └──► Acolhidos sem contato recente
```

### 7.3 Fluxo do Cuidador

```
Login
  │
  ▼
Meus Acolhidos (lista dos acolhidos atribuidos)
  │
  ├──► Ver perfil do acolhido
  │       ├──► Timeline de historico
  │       ├──► Dados de cadastro (somente leitura)
  │       └──► Botao: abrir WhatsApp
  │
  ├──► Registrar Acompanhamento
  │       ├──► Tipo de contato
  │       ├──► Observacoes
  │       └──► Opcionalmente atualizar interesse/crescimento/status
  │
  └──► Meu Perfil
          └──► Ver capacidade e ocupacao
```

---

## 8. Cenarios Criticos de Seguranca

### 8.1 Tentativa de Acesso Cross-Tenant

```
Cenario: Coordenador da Igreja A tenta acessar acolhido da Igreja B
Requisicao: GET /api/v1/acolhidos/{id_da_igreja_B}

1. TenantMiddleware: define tenant = igreja_A (do token)
2. Global Query Filter: WHERE igreja_id = igreja_A
3. Acolhido da igreja_B nao aparece no resultado
4. Resposta: 404 Not Found
```

### 8.2 Tentativa de Escalacao de Privilegio

```
Cenario: Cuidador tenta acessar endpoint de Coordenador
Requisicao: POST /api/v1/acolhidos (cadastrar novo acolhido)

1. AuthMiddleware: valida JWT, extrai claims
2. Policy "AdminOrCoordenador": verifica claim "perfil"
3. Perfil = "Cuidador" → nao satisfaz a policy
4. Resposta: 403 Forbidden
```

### 8.3 Cuidador Tenta Acessar Acolhido de Outro Cuidador

```
Cenario: Cuidador A tenta ver acolhido atribuido ao Cuidador B (mesma igreja)
Requisicao: GET /api/v1/acolhidos/{id_do_acolhido_do_cuidador_B}

1. TenantMiddleware: define tenant = igreja do cuidador A
2. Global Query Filter: filtra pela igreja (acolhido existe na igreja)
3. Ownership Filter: WHERE cuidador_id = cuidador_A
4. Acolhido do cuidador B nao aparece
5. Resposta: 404 Not Found
```

### 8.4 Admin Manipulando Token

```
Cenario: Usuario tenta falsificar claim "perfil" = "Admin"
Protecao:
1. JWT assinado com chave secreta no servidor
2. Alteracao de qualquer claim invalida a assinatura
3. AuthMiddleware rejeita token invalido
4. Resposta: 401 Unauthorized
```

---

## 9. Matriz Completa de Permissoes

| Recurso / Acao                        | Admin | Coordenador | Cuidador |
|---------------------------------------|-------|-------------|----------|
| **Igrejas**                           |       |             |          |
| Listar todas as igrejas               | OK    | -           | -        |
| Criar igreja                          | OK    | -           | -        |
| Editar igreja                         | OK    | -           | -        |
| Desativar igreja                      | OK    | -           | -        |
| **Coordenadores**                     |       |             |          |
| Listar coordenadores                  | OK    | -           | -        |
| Criar coordenador                     | OK    | -           | -        |
| Editar coordenador                    | OK    | -           | -        |
| **Cuidadores**                        |       |             |          |
| Listar cuidadores (da igreja)         | OK    | OK          | -        |
| Criar cuidador                        | OK    | OK          | -        |
| Editar cuidador                       | OK    | OK          | -        |
| Alterar disponibilidade               | OK    | OK          | -        |
| Alterar capacidade maxima             | OK    | OK          | -        |
| Desativar cuidador                    | OK    | OK          | -        |
| **Acolhidos**                         |       |             |          |
| Listar todos da igreja                | OK    | OK          | -        |
| Listar meus acolhidos                 | -     | -           | OK       |
| Ver detalhes (da igreja)              | OK    | OK          | -        |
| Ver detalhes (meu acolhido)           | -     | -           | OK       |
| Cadastrar acolhido                    | OK    | OK          | -        |
| Editar acolhido                       | OK    | OK          | -        |
| Atribuir a cuidador                   | OK    | OK          | -        |
| Reatribuir cuidador                   | OK    | OK          | -        |
| Alterar status                        | OK    | OK          | OK*      |
| Alterar interesse                     | OK    | OK          | OK*      |
| Alterar crescimento                   | OK    | OK          | OK*      |
| Desativar acolhido                    | OK    | OK          | -        |
| **Acompanhamentos**                   |       |             |          |
| Ver acompanhamentos (da igreja)       | OK    | OK          | -        |
| Ver acompanhamentos (meu acolhido)    | -     | -           | OK       |
| Registrar acompanhamento (da igreja)  | -     | OK          | -        |
| Registrar acompanhamento (meu)        | -     | -           | OK       |
| **Dashboard**                         |       |             |          |
| Dashboard global (cross-tenant)       | OK    | -           | -        |
| Dashboard da igreja                   | OK**  | OK          | -        |
| **Relatorios**                        |       |             |          |
| Relatorios da igreja                  | OK**  | OK          | -        |
| Relatorio cross-tenant                | OK    | -           | -        |

`*` Somente para acolhidos atribuidos ao cuidador.
`**` Admin precisa selecionar uma igreja via header X-Tenant-Id.

---

## 10. Consideracoes de Implementacao

### 10.1 Nao Confiar no Frontend

Todas as validacoes de role e tenant devem ser feitas **no backend**. O frontend apenas esconde/exibe elementos para UX, mas a seguranca real e no servidor.

### 10.2 Auditoria de Acesso

Considerar registrar tentativas de acesso negado (403) para detectar uso indevido:

```csharp
// Middleware de auditoria (futuro)
if (context.Response.StatusCode == 403)
{
    _logger.LogWarning("Acesso negado: User={UserId}, Path={Path}, Perfil={Perfil}",
        userId, context.Request.Path, perfil);
}
```

### 10.3 Testes Obrigatorios

Para cada endpoint, testar os seguintes cenarios:
1. Acesso com perfil correto → 200/201
2. Acesso com perfil insuficiente → 403
3. Acesso cross-tenant → 404
4. Cuidador acessando acolhido de outro cuidador → 404
5. Token expirado → 401
6. Token invalido/adulterado → 401
