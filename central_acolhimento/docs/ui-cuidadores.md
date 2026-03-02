# UI - Gestao de Cuidadores

> Tela de gerenciamento de cuidadores. Visao de cards com capacidade, atividades e perfil lateral.

## 1. Layout Geral

```
┌───────────────────────────────────────────────────────────────────────────┐
│ ┌──────────────┐                                                          │
│ │  SIDEBAR     │   AREA DE CONTEUDO                             DRAWER   │
│ │              │                                                          │
│ │ □ Dashboard  │   Gestao de Cuidadores                    ┌───────────┐ │
│ │ ■ Cuidadores │   Gerencie a disponibilidade e a carga    │ Perfil do │ │
│ │ □ Visitas    │   dos seus X lideres cuidadores           │ Cuidador  │ │
│ │ □ Relatorios │                                           │           │ │
│ │ □ Config     │   [Pesquisar cuidador...] [+Adicionar]    │ Joao Silva│ │
│ │              │                                           │ ● ATIVO   │ │
│ │              │   [Todos] [Disponiveis] [Em Alerta]       │           │ │
│ │              │   LISTA ▼  OCUPACAO ▼                     │ email     │ │
│ │              │                                           │ tel       │ │
│ │              │   ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐   │           │ │
│ │              │   │Card 1│ │Card 2│ │Card 3│ │Card 4│   │ ESCALA DE │ │
│ │              │   │Joao  │ │Maria │ │Anton.│ │Franc.│   │ ATIVIDADES│ │
│ │              │   │Silva │ │Oliv. │ │Santos│ │Costa │   │           │ │
│ │              │   └──────┘ └──────┘ └──────┘ └──────┘   │ Visitas:24│ │
│ │              │                                           │           │ │
│ │              │   ┌──────┐  ┌ ─ ─ ─ ─ ─ ─ ┐             │ HISTORICO │ │
│ │              │   │Card 5│  │  + Mover     │             │ RECENTES  │ │
│ │              │   │Lucas │  │  Cuidador    │             │           │ │
│ │              │   │Ferraz│  └ ─ ─ ─ ─ ─ ─ ┘             │[Editar]   │ │
│ │              │   └──────┘                                └───────────┘ │
│ │              │                                                          │
│ │ Mapa de     │                                                          │
│ │ Membros     │                                                          │
│ └──────────────┘                                                          │
└───────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Componentes Detalhados

### 2.1 Sidebar

| Elemento                | Tipo         | Comportamento                             |
|-------------------------|--------------|-------------------------------------------|
| Item "Dashboard"        | NavLink      | Navega para `/dashboard`                  |
| Item "Cuidadores" (ativo)| NavLink     | Destaque azul, pagina atual               |
| Item "Visitas"          | NavLink      | Navega para `/acompanhamentos`            |
| Item "Relatorios"       | NavLink      | Navega para `/relatorios`                 |
| Item "Configuracoes"    | NavLink      | Navega para `/configuracoes`              |
| "Mapa de Membros"       | Link/Badge   | Link para visualizacao geografica (futuro)|

### 2.2 Cabecalho da Pagina

| Elemento                        | Tipo           | Dados                                  |
|---------------------------------|----------------|----------------------------------------|
| Titulo "Gestao de Cuidadores"   | H1             | Fixo                                   |
| Subtitulo                       | Texto cinza    | "Gerencie a disponibilidade e a carga dos seus X lideres cuidadores" |
| Campo "Pesquisar cuidador..."   | SearchInput    | Filtra cards por nome                  |
| Botao "+ Adicionar Novo Cuidador"| Button primary| Abre modal de cadastro                 |

### 2.3 Filtros e Ordenacao (Tabs + Selects)

#### Tabs de Filtro Rapido

| Tab           | Filtro                                      | Descricao                    |
|---------------|---------------------------------------------|------------------------------|
| Todos         | Sem filtro                                  | Exibe todos os cuidadores    |
| Disponiveis   | `disponibilidade = true` AND `ocupacao < 100%` | Prontos para receber acolhidos |
| Em Alerta     | `ocupacao >= 80%` OR `disponibilidade = false` | Cuidadores que precisam de atencao |

#### Selects de Ordenacao

| Select       | Opcoes                                              |
|--------------|-----------------------------------------------------|
| LISTA        | Todos, Por bairro, Por grupo                        |
| OCUPACAO     | Maior ocupacao primeiro, Menor ocupacao primeiro     |

---

### 2.4 Grid de Cards de Cuidadores

Layout em grid responsivo (4 colunas em desktop, 2 em tablet, 1 em mobile).

#### Card de Cuidador

```
┌──────────────────────────────────┐
│  👤 Joao Silva                   │
│     Membro Ativo                 │
│                                  │
│  Visitas: Ativas    06           │
│  Ultima Atividade: Ha 2h         │
│                                  │
│  ████████████████░░░░  SAUDAVEL  │
│                                  │
│  [DISPONIVEIS PARA ATENDIMENTO]  │
└──────────────────────────────────┘
```

| Elemento                  | Tipo            | Dados                                      |
|---------------------------|-----------------|---------------------------------------------|
| Avatar                    | Imagem/Iniciais | Foto do cuidador ou iniciais coloridas      |
| Nome                      | Texto bold       | `cuidador.nome`                             |
| Status                    | Badge            | "Membro Ativo" / "Inativo"                 |
| Visitas Ativas            | Label + Numero   | Total de acolhidos ativos atribuidos        |
| Ultima Atividade          | Texto cinza      | Tempo desde ultimo acompanhamento registrado|
| Barra de ocupacao         | ProgressBar      | Proporcional (acolhidosAtivos / capacidadeMax) |
| Label de estado           | Badge colorido   | Estado baseado na ocupacao                  |

**Labels de estado na barra:**

| Percentual ocupacao | Label                         | Cor da barra | Cor do badge  |
|---------------------|-------------------------------|-------------- |---------------|
| 0-59%               | DISPONIVEIS PARA ATENDIMENTO  | Verde         | Verde         |
| 60-79%              | ATENCAO MODERADA              | Amarelo       | Amarelo       |
| 80-99%              | CAPACIDADE EM ALERTA          | Laranja       | Laranja       |
| 100%                | CAPACIDADE MAXIMA             | Vermelho      | Vermelho      |

**Interacao do card:**
- Clique no card: abre Drawer lateral com perfil completo.
- Card selecionado: borda azul de destaque.

---

### 2.5 Card "+ Mover Cuidador" (Placeholder)

```
┌ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ┐
│                                    │
│            +                       │
│       Mover Cuidador               │
│                                    │
└ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ┘
```

| Propriedade | Descricao                                                          |
|-------------|--------------------------------------------------------------------|
| Tipo        | Card com borda tracejada (dashed)                                  |
| Acao        | Abre modal para transferir cuidador de outra equipe/grupo          |
| Visivel     | Sempre, no final da grid                                           |

---

### 2.6 Drawer: Perfil do Cuidador (Painel Lateral Direito)

Abre ao clicar em um card de cuidador. Slide-in da direita.

```
┌─────────────────────────────────────┐
│                                  X  │
│  👤 Joao Silva                      │
│  ● ATIVO                           │
│                                     │
│  📧 joao.silva@email.com            │
│  📱 (11) 99885-7777                 │
│                                     │
│  ──────────────────────────────     │
│  ESCALA DE ATIVIDADES               │
│                                     │
│  Visitas este mes         24        │
│                                     │
│  ──────────────────────────────     │
│  HISTORICO RECENTES                 │
│                                     │
│  ✓ Visita Finalizada                │
│    15 Fevereiro                     │
│                                     │
│  📄 Relatorio de Visita Enviado     │
│    14 Fevereiro - Debates as 16:35  │
│                                     │
│  ┌───────────────────────────────┐  │
│  │        Editar Perfil          │  │
│  └───────────────────────────────┘  │
│                                     │
└─────────────────────────────────────┘
```

#### Secao: Informacoes Basicas

| Campo          | Tipo          | Dados                                      |
|----------------|---------------|--------------------------------------------|
| Avatar grande  | Imagem        | Foto ou iniciais                           |
| Nome           | H2            | `cuidador.nome`                            |
| Status badge   | Badge         | "● ATIVO" verde ou "● INATIVO" cinza      |
| Email          | Link mailto   | `usuario.email`                            |
| Telefone       | Link tel      | `cuidador.whatsapp` (clicavel)             |

#### Secao: Escala de Atividades

| Metrica               | Tipo           | Dados                                    |
|-----------------------|----------------|------------------------------------------|
| Visitas este mes      | Label + Numero | COUNT acompanhamentos WHERE tipo IN (Visita, Presencial) AND mes_atual |

#### Secao: Historico Recentes

Lista dos ultimos 5 acompanhamentos registrados pelo cuidador.

| Campo          | Tipo          | Dados                                      |
|----------------|---------------|--------------------------------------------|
| Icone          | Icon          | ✓ para visita finalizada, 📄 para relatorio|
| Titulo         | Texto bold    | Tipo de atividade                          |
| Data           | Texto cinza   | Data do acompanhamento                     |
| Detalhe        | Texto cinza   | Observacoes (truncado)                     |

#### Botao "Editar Perfil"

Abre modal de edicao com campos:
- Nome Completo
- Email
- WhatsApp
- Cidade
- Disponibilidade (toggle)
- Capacidade maxima (number input)

---

## 3. Modal: Adicionar Novo Cuidador

```
┌───────────────────────────────────────┐
│ Adicionar Novo Cuidador            X  │
├───────────────────────────────────────┤
│                                       │
│ Nome Completo*      [               ] │
│ Email*              [               ] │
│ WhatsApp*           [(  )       -   ] │
│ Cidade*             [               ] │
│                                       │
│ Disponibilidade     [● Sim  ○ Nao  ] │
│ Capacidade maxima   [  5  ] (1-20)    │
│                                       │
│ Senha inicial*      [               ] │
│ Confirmar senha*    [               ] │
│                                       │
│ ℹ️  Sera criado um usuario com       │
│    perfil Cuidador automaticamente.   │
│                                       │
│          [Cancelar]   [Cadastrar]     │
└───────────────────────────────────────┘
```

**Validacoes:**
- Nome: minimo 3 caracteres.
- Email: formato valido, unico no sistema.
- WhatsApp: formato (XX) XXXXX-XXXX, unico na igreja.
- Capacidade: entre 1 e 20.
- Senha: minimo 8 caracteres, 1 maiuscula, 1 numero.

**Ao salvar:**
1. Cria usuario com perfil Cuidador.
2. Cria registro de cuidador vinculado ao usuario e a igreja.
3. Card aparece na grid automaticamente.
4. Toast de sucesso: "Cuidador cadastrado com sucesso!"

---

## 4. Interacoes e Acoes

### 4.1 Pesquisa de Cuidador

- Debounce de 300ms.
- Filtra cards na grid em tempo real (client-side).
- Se muitos cuidadores (> 20), faz busca no servidor.

### 4.2 Drag & Drop entre Cards (Futuro)

- Arrastar acolhido de um card de cuidador para outro.
- Validacao de capacidade ao soltar.
- Confirmacao antes de efetivar transferencia.

### 4.3 Clique no Card

- Abre Drawer lateral com perfil completo.
- Card fica com borda azul de destaque.
- Clique fora do Drawer ou no X fecha o painel.

### 4.4 Acoes no Drawer

| Acao                  | Descricao                                          |
|-----------------------|----------------------------------------------------|
| Editar Perfil         | Abre modal de edicao                               |
| Ver Acolhidos         | Navega para `/acolhidos?cuidadorId={id}`           |
| Alterar Disponibilidade| Toggle rapido no drawer                           |
| Desativar Cuidador    | Confirmacao + reatribuicao dos acolhidos           |

---

## 5. Estados Visuais

### 5.1 Card States

| Estado            | Visual                                                |
|-------------------|-------------------------------------------------------|
| Normal            | Fundo branco, sombra leve                             |
| Hover             | Sombra aumenta, cursor pointer                        |
| Selecionado       | Borda azul 2px, sombra azul leve                      |
| Em alerta         | Borda laranja sutil, badge "CAPACIDADE EM ALERTA"     |
| Critico           | Borda vermelha sutil, badge "CAPACIDADE MAXIMA"       |
| Inativo           | Opacidade reduzida (0.6), badge "INATIVO" cinza       |

### 5.2 Loading States

| Componente   | Loading                                                   |
|--------------|-----------------------------------------------------------|
| Grid         | Skeleton cards (4 placeholders)                           |
| Drawer       | Skeleton para info + spinner para historico                |
| Pesquisa     | Spinner no icone de busca                                 |

### 5.3 Empty States

| Cenario              | Mensagem                                              |
|----------------------|-------------------------------------------------------|
| Nenhum cuidador      | "Nenhum cuidador cadastrado. Adicione o primeiro!"    |
| Pesquisa sem resultado| "Nenhum cuidador encontrado para '{termo}'"          |
| Filtro sem resultado | "Nenhum cuidador com o filtro selecionado"            |
| Drawer sem historico | "Nenhuma atividade recente registrada"                |

---

## 6. Responsividade

| Breakpoint  | Grid                  | Drawer                          |
|-------------|-----------------------|---------------------------------|
| Desktop     | 4 colunas             | Lateral (30% da tela)           |
| Tablet      | 2 colunas             | Lateral (50% da tela)           |
| Mobile      | 1 coluna              | Full screen (overlay)           |

---

## 7. Endpoints Necessarios

| Endpoint                                | Descricao                                   | Novo? |
|-----------------------------------------|---------------------------------------------|-------|
| `GET /api/v1/cuidadores`               | Lista com filtros (existente)               | Nao   |
| `GET /api/v1/cuidadores/{id}`          | Detalhes do cuidador (existente)            | Nao   |
| `GET /api/v1/cuidadores/{id}/atividades`| Historico de acompanhamentos do cuidador   | Sim   |
| `GET /api/v1/cuidadores/{id}/metricas` | Metricas: visitas no mes, etc              | Sim   |
| `POST /api/v1/cuidadores`              | Cadastro (existente)                        | Nao   |
| `PUT /api/v1/cuidadores/{id}`          | Edicao (existente)                          | Nao   |
| `PATCH /api/v1/cuidadores/{id}/disponibilidade` | Toggle disponibilidade (existente) | Nao   |

---

## 8. Mapeamento de Dados: Card → API

```typescript
// Tipo do card de cuidador na UI
interface CuidadorCard {
  id: string;
  nome: string;
  avatar?: string;
  email: string;
  whatsapp: string;
  status: 'ativo' | 'inativo';          // derivado de cuidador.ativo
  visitasAtivas: number;                 // cuidador.acolhidosAtivos
  ultimaAtividade: string;               // tempo relativo do ultimo acompanhamento
  ocupacaoPercentual: number;            // (acolhidosAtivos / capacidadeMax) * 100
  capacidadeMax: number;
  estadoOcupacao: 'saudavel' | 'atencao' | 'alerta' | 'critico';
  labelOcupacao: string;                 // texto do badge
}

// Mapeamento de estadoOcupacao
function getEstadoOcupacao(percentual: number): string {
  if (percentual >= 100) return 'critico';
  if (percentual >= 80) return 'alerta';
  if (percentual >= 60) return 'atencao';
  return 'saudavel';
}
```
