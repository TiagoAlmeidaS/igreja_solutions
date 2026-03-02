# UI - Dashboard: Torre de Controle (Coordenacao)

> Tela principal do Coordenador. Visao consolidada do estado do acolhimento na igreja.

## 1. Layout Geral

```
┌─────────────────────────────────────────────────────────────────────────┐
│  HEADER                                                                  │
│  ┌────────────┐  Dashboard  Relatorios  Cuidadores  Config  │ Q Buscar │
│  │ Torre de   │                                              │ 🔔  👤  │
│  │ Controle   │                                              │          │
│  └────────────┘                                              │          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐      │
│  │ KPI Card 1       │  │ KPI Card 2       │  │ KPI Card 3       │      │
│  │ Taxa Retencao    │  │ Casas Ativas     │  │ Alerta Inativid. │      │
│  └──────────────────┘  └──────────────────┘  └──────────────────┘      │
│                                                                          │
│  ┌─────────────────────────────────────────┐  ┌──────────────────────┐ │
│  │ Jornada Espiritual (Kanban)             │  │ Gestao de Capacidade │ │
│  │                        [+Novo Convidado]│  │                      │ │
│  │ ┌─────────┐ ┌──────────┐ ┌───────────┐ │  │ Lista de cuidadores  │ │
│  │ │ Novo    │ │ Primeira │ │ Em Acomp.  │ │  │ com barras de ocup.  │ │
│  │ │ Contato │ │ Visita   │ │            │ │  │                      │ │
│  │ │  (4)    │ │   (2)    │ │    (8)     │ │  │ Capacidade Total: %  │ │
│  │ │         │ │          │ │            │ │  │ [Balancear Cargas]   │ │
│  │ └─────────┘ └──────────┘ └───────────┘ │  └──────────────────────┘ │
│  └─────────────────────────────────────────┘                           │
│                                                                          │
├─────────────────────────────────────────────────────────────────────────┤
│  FOOTER                                                                  │
│  ● Servidor Online  ○ Ultima sincronizacao: Agora   Manual  Termos  Sup│
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Componentes Detalhados

### 2.1 Header / Navegacao Principal

| Elemento               | Tipo          | Comportamento                                         |
|------------------------|---------------|-------------------------------------------------------|
| Logo "Torre de Controle" | Link        | Redireciona para Dashboard                            |
| Subtitulo "SISTEMA DE ACOLHIMENTO" | Texto | Apenas visual                               |
| Tab "Dashboard"        | NavLink ativo | Destaque azul quando na pagina atual                  |
| Tab "Relatorios"       | NavLink       | Navega para `/relatorios`                             |
| Tab "Cuidadores"       | NavLink       | Navega para `/cuidadores`                             |
| Tab "Configuracoes"    | NavLink       | Navega para `/configuracoes`                          |
| Campo "Buscar convidado..." | SearchInput | Busca global por nome/whatsapp de acolhidos       |
| Icone Notificacao (sino) | IconButton  | Abre painel de notificacoes com badge de contagem     |
| Avatar do usuario      | AvatarButton  | Abre dropdown com perfil, trocar senha, sair          |

**Visibilidade por role:**
- Coordenador: todas as tabs visiveis
- Admin: todas + seletor de igreja no header

### 2.2 KPI Cards (Linha Superior)

Tres cards de metricas em destaque horizontal.

#### Card 1 - Taxa de Retencao TCI

| Propriedade    | Valor                                                      |
|----------------|------------------------------------------------------------|
| Titulo         | "Taxa de Retencao TCI"                                    |
| Valor principal| Percentual (ex: `87.4%`)                                  |
| Indicador      | Seta verde para cima `+2.4%` ou vermelha para baixo       |
| Barra progresso| Barra azul proporcional ao percentual                     |
| Icone          | Icone de grafico (canto superior direito)                  |
| Tooltip        | "Percentual de acolhidos que permaneceram ativos no periodo"|

**Calculo:**
```
Taxa Retencao = (acolhidos ativos hoje / total de acolhidos cadastrados no periodo) * 100
Variacao = taxa_atual - taxa_periodo_anterior
```

**Endpoint:** `GET /api/v1/dashboard/kpis`

#### Card 2 - Casas Ativas

| Propriedade    | Valor                                                      |
|----------------|------------------------------------------------------------|
| Titulo         | "Casas Ativas"                                             |
| Valor principal| Percentual (ex: `92%`)                                    |
| Meta           | Texto "Meta: 95%" em cinza ao lado                        |
| Barra progresso| Barra azul proporcional                                   |
| Icone          | Icone de casa (canto direito)                              |
| Tooltip        | "Percentual de acolhidos com pelo menos 1 visita presencial"|

**Calculo:**
```
Casas Ativas = (acolhidos com >= 1 visita no periodo / total acolhidos ativos) * 100
```

#### Card 3 - Alerta de Inatividade

| Propriedade    | Valor                                                      |
|----------------|------------------------------------------------------------|
| Titulo         | "Alerta de Inatividade"                                   |
| Valor principal| Numero inteiro (ex: `14`)                                 |
| Subtitulo      | "Casos urgentes"                                           |
| Cor destaque   | Vermelho/laranja (alerta)                                  |
| Icone          | Triangulo de alerta (canto direito)                        |
| Descricao      | "Exige intervencao nas ultimas 48h"                        |
| Clicavel       | Sim - abre lista filtrada de acolhidos inativos            |

**Calculo:**
```
Casos urgentes = acolhidos ativos sem nenhum contato nos ultimos 14 dias
```

---

### 2.3 Jornada Espiritual (Kanban Board)

Board estilo Kanban com colunas representando o status de acompanhamento.

#### Estrutura

```
┌────────────────┐  ┌────────────────┐  ┌────────────────────┐
│● NOVO CONTATO  │  │● PRIMEIRA      │  │● EM ACOMPANHAMENTO │
│  (4)           │  │  VISITA (2)    │  │  (8)               │
│                │  │                │  │                    │
│ ┌────────────┐ │  │ ┌────────────┐ │  │ ┌────────────────┐ │
│ │ Ana Souza  │ │  │ │Marcos Lima │ │  │ │ Julia Silva    │ │
│ │  3 dias    │ │  │ │  5 dias    │ │  │ │  2 dias        │ │
│ │ Pendente   │ │  │ │ Aguardando │ │  │ │ Consolidacao   │ │
│ │ retorno    │ │  │ │ feedback   │ │  │ │ ativa          │ │
│ │ [wa][📞]   │ │  │ │ [wa]       │ │  │ │ [📞]           │ │
│ └────────────┘ │  │ └────────────┘ │  │ └────────────────┘ │
│                │  │                │  │                    │
│ ┌────────────┐ │  │                │  │                    │
│ │Carlos A.   │ │  │                │  │                    │
│ │  Hoje      │ │  │                │  │                    │
│ │ Enviou     │ │  │                │  │                    │
│ │ interesse  │ │  │                │  │                    │
│ │ [wa]       │ │  │                │  │                    │
│ └────────────┘ │  │                │  │                    │
└────────────────┘  └────────────────┘  └────────────────────┘
```

#### Cabecalho do Kanban

| Elemento                   | Tipo         | Comportamento                          |
|----------------------------|--------------|----------------------------------------|
| Titulo "Jornada Espiritual"| H2 com icone | Icone de chama/espirito               |
| Botao "+ Novo Convidado"   | Button primary| Abre modal de cadastro de acolhido    |

#### Colunas

| Coluna              | Cor do indicador | Filtro de status      | Contagem          |
|---------------------|------------------|-----------------------|-------------------|
| Novo Contato        | Cinza (●)        | `status = 0`          | Total entre ()    |
| Primeira Visita     | Amarelo (●)      | `status = 1`          | Total entre ()    |
| Em Acompanhamento   | Verde (●)        | `status = 2`          | Total entre ()    |

**Nota:** A coluna "Desativada" nao aparece no Kanban. Acolhidos desativados sao acessiveis via filtro na listagem completa.

#### Card de Acolhido (dentro da coluna)

| Elemento              | Tipo            | Dados                                       |
|-----------------------|-----------------|---------------------------------------------|
| Nome                  | Texto bold      | `acolhido.nomeCompleto`                     |
| Badge de dias         | Badge colorido  | Dias desde ultimo contato                   |
| Observacao curta      | Texto cinza     | Ultima observacao do acompanhamento (truncada)|
| Icones de acao rapida | IconButtons     | WhatsApp (abre link), Telefone (abre link)  |
| Menu "..."            | DropdownMenu    | Ver perfil, Reatribuir, Registrar contato   |

**Cores do badge de dias:**
| Dias sem contato | Cor do badge  | Texto          |
|------------------|---------------|----------------|
| 0-3 dias         | Verde         | `X dias`       |
| 4-7 dias         | Amarelo       | `X dias`       |
| 8-14 dias        | Laranja       | `X dias`       |
| 15+ dias         | Vermelho      | `X dias`       |
| Hoje             | Azul          | `Hoje`         |

**Drag & Drop:**
- Cards podem ser arrastados entre colunas para mudar o status.
- Ao soltar em outra coluna, abre confirmacao com campo de observacao.
- Mover para "Em Acompanhamento" exige cuidador atribuido.

**Endpoint:** `GET /api/v1/dashboard/kanban`

---

### 2.4 Gestao de Capacidade (Painel Lateral Direito)

| Elemento                | Tipo            | Dados                                     |
|-------------------------|-----------------|-------------------------------------------|
| Titulo                  | H3              | "Gestao de Capacidade"                    |
| Subtitulo               | Texto cinza     | "CUIDADORES ATIVOS"                       |
| Lista de cuidadores     | List            | Nome, ocupacao, barra visual              |

#### Item de Cuidador na Lista

```
┌─────────────────────────────────────────────┐
│ 👤 Joao Silva                            1  │  ← verde (baixa ocupacao)
│    0-1 acolhidos                             │
│                                              │
│ 👤 Maria Oliveira                        3  │  ← amarelo (media)
│    D- 3 acolhidos                            │
│                                              │
│ 👤 Pedro Albuquerque                     5  │  ← vermelho (cheio)
│    4+ acolhidos                              │
│                                              │
│ 👤 Carla Rocha                           2  │  ← verde
│    D- 2 acolhidos                            │
│                                              │
│ 👤 Jose Dantas                           0  │  ← verde (vazio)
│    0-1 acolhidos                             │
└─────────────────────────────────────────────┘
```

| Propriedade por cuidador | Tipo              | Dados                                   |
|--------------------------|-------------------|-----------------------------------------|
| Avatar                   | Icone/Foto        | Placeholder ou foto do cuidador         |
| Nome                     | Texto bold         | `cuidador.nome`                         |
| Quantidade de acolhidos  | Texto cinza        | "D- X acolhidos"                        |
| Badge numerico           | Badge circular     | Numero de acolhidos ativos              |

**Cores do badge numerico:**
| Percentual ocupacao | Cor        |
|---------------------|------------|
| 0-59%               | Verde      |
| 60-79%              | Amarelo    |
| 80-99%              | Laranja    |
| 100%                | Vermelho   |

#### Capacidade Total

| Elemento                | Tipo             | Dados                                    |
|-------------------------|------------------|------------------------------------------|
| Label                   | Texto            | "CAPACIDADE TOTAL"                       |
| Percentual              | Texto bold       | Ex: `78%`                                |
| Barra de progresso      | ProgressBar      | Proporcional, cor varia com percentual   |
| Botao "Balancear Cargas"| Button secondary | Abre sugestao de redistribuicao          |

**Calculo:**
```
Capacidade Total = (soma acolhidos ativos de todos cuidadores / soma capacidade_max) * 100
```

**Acao "Balancear Cargas":**
- Abre modal com sugestoes de reatribuicao.
- Mostra cuidadores sobrecarregados e cuidadores com vagas.
- Coordenador pode aceitar/rejeitar cada sugestao.

**Endpoint:** `GET /api/v1/dashboard/capacidade`

---

### 2.5 Footer

| Elemento                      | Tipo          | Comportamento                          |
|-------------------------------|---------------|----------------------------------------|
| Status do servidor            | Indicador     | ● Verde "Servidor Online" ou ● Vermelho "Offline" |
| Ultima sincronizacao          | Texto cinza   | "Ultima sincronizacao: Agora" / "Ha 5 min" |
| Link "Manual do Sistema"     | Link          | Abre documentacao de ajuda             |
| Link "Termos de Uso"         | Link          | Abre termos                            |
| Link "Suporte Tecnico"       | Link          | Abre canal de suporte                  |

---

## 3. Interacoes e Acoes

### 3.1 Botao "+ Novo Convidado"

Abre modal de cadastro rapido:

```
┌─────────────────────────────────────┐
│ Novo Convidado                   X  │
├─────────────────────────────────────┤
│ Nome Completo*     [              ] │
│ WhatsApp*          [(  )      -   ] │
│ Bairro*            [              ] │
│ Cidade*            [              ] │
│ Quem convidou?     [              ] │
│ Interesse*         [Frio ▼       ] │
│ Observacoes        [              ] │
│                    [              ] │
│ Atribuir cuidador? [Selecionar ▼ ] │
│                                     │
│         [Cancelar]  [Cadastrar]     │
└─────────────────────────────────────┘
```

### 3.2 Busca Global "Buscar convidado..."

- Busca em tempo real (debounce 300ms).
- Retorna acolhidos pelo nome ou whatsapp.
- Dropdown de resultados com: nome, status (badge), cuidador atribuido.
- Clique redireciona para o perfil do acolhido.
- Endpoint: `GET /api/v1/acolhidos?search={termo}&pageSize=5`

### 3.3 Menu "..." no Card do Acolhido

| Opcao               | Acao                                           |
|----------------------|------------------------------------------------|
| Ver perfil completo  | Navega para `/acolhidos/{id}`                  |
| Registrar contato    | Abre modal de registro de acompanhamento       |
| Reatribuir cuidador  | Abre modal de selecao de cuidador              |
| Enviar WhatsApp      | Abre `https://wa.me/{whatsapp}`                |
| Desativar            | Abre confirmacao com campo de motivo           |

---

## 4. Estados e Loading

| Estado              | Comportamento                                         |
|---------------------|-------------------------------------------------------|
| Carregando          | Skeleton loaders nos KPI cards e colunas do kanban    |
| Sem dados           | Empty state: "Nenhum acolhido cadastrado. Comece adicionando o primeiro!" |
| Erro de conexao     | Banner vermelho no topo: "Erro ao carregar dados. Tentar novamente." |
| Dados desatualizados| Footer mostra "Ultima sincronizacao: Ha 5 min" em amarelo |

---

## 5. Responsividade

| Breakpoint  | Adaptacao                                                |
|-------------|----------------------------------------------------------|
| Desktop     | Layout completo conforme wireframe (3 colunas kanban + painel lateral) |
| Tablet      | Kanban em 2 colunas com scroll horizontal, painel lateral colapsavel |
| Mobile      | Kanban em 1 coluna (swipe), painel de capacidade abaixo  |

---

## 6. Endpoints Necessarios

| Endpoint                          | Descricao                                  | Novo? |
|-----------------------------------|--------------------------------------------|-------|
| `GET /api/v1/dashboard/kpis`      | KPIs: retencao, casas ativas, inatividade  | Sim   |
| `GET /api/v1/dashboard/kanban`    | Acolhidos agrupados por status (kanban)    | Sim   |
| `GET /api/v1/dashboard/capacidade`| Capacidade consolidada dos cuidadores      | Sim   |
| `GET /api/v1/dashboard`           | Dashboard completo (existente, manter)     | Nao   |
| `PATCH /api/v1/acolhidos/{id}/status` | Mudar status via drag-and-drop         | Nao   |
| `POST /api/v1/acolhidos`          | Cadastro via modal "+ Novo Convidado"      | Nao   |
| `GET /api/v1/acolhidos?search=`   | Busca global                               | Nao   |
