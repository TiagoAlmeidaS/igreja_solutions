# UI - Relatorios de Gestao

> Tela de relatorios do Coordenador. Acompanhamento de metas e evolucao espiritual da rede.

## 1. Layout Geral

```
┌───────────────────────────────────────────────────────────────────────────┐
│ ┌──────────────┐                                                          │
│ │  SIDEBAR     │   AREA DE CONTEUDO                                      │
│ │              │                                                          │
│ │ 🔍 Pesquisar│   ┌────────────────────────────┐  Coord. Silva          │
│ │              │   │ Relatorios de Gestao       │  Nivel Master          │
│ │ □ Dashboard  │   │ Acompanhamento de metas e  │                        │
│ │ □ Acolhimento│   │ evolucao espiritual da rede│  [Exportar] [Compart.] │
│ │ ■ Relatorios │   └────────────────────────────┘                        │
│ │ □ Lideres    │                                                          │
│ │              │   ┌─────────────────────────────────────────────────┐    │
│ │ ADMINISTRACAO│   │ FILTROS                                         │    │
│ │ □ Config     │   │ [Este Mes ▼] [Todos Setores ▼] [Tipo ▼] [Stat ▼│   │
│ │              │   └─────────────────────────────────────────────────┘    │
│ │              │                                                          │
│ │              │   ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐                  │
│ │              │   │ KPI1 │ │ KPI2 │ │ KPI3 │ │ KPI4 │                  │
│ │              │   │Total │ │Reten.│ │Visit.│ │Novas │                  │
│ │              │   │Acolh.│ │      │ │Realiz│ │Decis.│                  │
│ │              │   └──────┘ └──────┘ └──────┘ └──────┘                  │
│ │              │                                                          │
│ │              │   ┌──────────────────┐ ┌──────────────────────────┐     │
│ │              │   │ Grafico Retencao │ │ Crescimento: Metabolismo │     │
│ │              │   │ ao Longo do Tempo│ │ da Alma                  │     │
│ │              │   │ (barras mensais) │ │ (barras horizontais)     │     │
│ │              │   └──────────────────┘ └──────────────────────────┘     │
│ │              │                                                          │
│ │              │   ┌────────────────────────────────────────────────┐    │
│ │              │   │ Tabela: Resumo por Lideranca e Transicoes      │    │
│ │              │   └────────────────────────────────────────────────┘    │
│ │              │                                                          │
│ │              │   ┌────────────────────────────────────────────────┐    │
│ │              │   │ 💡 Insight da IA Torre                         │    │
│ │              │   └────────────────────────────────────────────────┘    │
│ │              │                                                          │
│ │ [+Novo       │                                                          │
│ │  Relatorio]  │                                                          │
│ └──────────────┘                                                          │
└───────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Componentes Detalhados

### 2.1 Sidebar (Navegacao Lateral)

| Elemento                   | Tipo         | Comportamento                            |
|----------------------------|--------------|------------------------------------------|
| Campo "Pesquisar relatorios..." | SearchInput | Filtra relatorios salvos             |
| Item "Dashboard"           | NavLink      | Navega para `/dashboard`                 |
| Item "Acolhimento"        | NavLink      | Navega para `/acolhidos`                 |
| Item "Relatorios" (ativo)  | NavLink      | Destaque azul, pagina atual              |
| Item "Lideres"             | NavLink      | Navega para `/cuidadores`                |
| Separador "ADMINISTRACAO"  | Divider      | Agrupa itens administrativos             |
| Item "Configuracoes"       | NavLink      | Navega para `/configuracoes`             |
| Botao "+ Novo Relatorio"   | Button blue  | Abre wizard de criacao de relatorio customizado |

### 2.2 Cabecalho da Pagina

| Elemento                   | Tipo           | Dados                                   |
|----------------------------|----------------|-----------------------------------------|
| Titulo "Relatorios de Gestao" | H1          | Fixo                                    |
| Subtitulo                  | Texto cinza    | "Acompanhamento de metas e evolucao espiritual da rede" |
| Info do usuario            | UserCard       | Nome "Coord. Silva", nivel/badge        |
| Botao "Exportar PDF"       | Button outline | Gera PDF do relatorio atual             |
| Botao "Compartilhar"       | Button primary | Abre modal de compartilhamento (email, link) |

### 2.3 Barra de Filtros

| Filtro                     | Tipo           | Opcoes                                   |
|----------------------------|----------------|------------------------------------------|
| Periodo                    | Select/Date    | Este Mes, Ultimo Mes, Ultimos 3 Meses, Ultimo Ano, Personalizado |
| Setor                      | Select         | Todos os Setores, [lista de bairros/regioes] |
| Tipo de Acolhimento        | Select         | Todos, Novo Contato, Primeira Visita, Em Acompanhamento |
| Status                     | Select         | Todos, Consolidacao, Integracao, Discipulado |

**Comportamento:**
- Filtros aplicam em cascata a todos os componentes da pagina.
- Ao alterar qualquer filtro, KPIs, graficos e tabela atualizam automaticamente.
- Filtros persistem na sessao (nao resetam ao navegar).

---

### 2.4 KPI Cards (4 metricas)

#### Card 1 - Total Acolhidos

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Titulo          | "TOTAL ACOLHIDOS"                                        |
| Icone           | Icone de grupo/pessoas (verde)                            |
| Valor principal | Numero inteiro (ex: `1.284`)                              |
| Variacao        | `+12% vs anterior` em verde ou `-X%` em vermelho          |

**Calculo:**
```
Total Acolhidos = COUNT(acolhidos WHERE ativo = true AND igreja_id = tenant)
Variacao = ((total_periodo_atual - total_periodo_anterior) / total_periodo_anterior) * 100
```

#### Card 2 - Taxa de Retencao

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Titulo          | "TAXA DE RETENCAO"                                       |
| Icone           | Icone de retorno/seta circular (azul)                     |
| Valor principal | Percentual (ex: `74.2%`)                                 |
| Meta            | `Meta: 80%` em cinza                                     |
| Indicador       | `Subiu 1.8%` em verde                                    |

**Calculo:**
```
Retencao = (acolhidos que permaneceram ativos no periodo / total inicio do periodo) * 100
```

#### Card 3 - Visitas Realizadas

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Titulo          | "VISITAS REALIZADAS"                                     |
| Icone           | Icone de localizacao/pin (roxo)                           |
| Valor principal | Numero inteiro (ex: `456`)                                |
| Meta            | `Meta: 15/Dia` em cinza                                  |

**Calculo:**
```
Visitas = COUNT(acompanhamentos WHERE tipo_contato IN (Visita, Presencial) AND periodo)
```

#### Card 4 - Novas Decisoes

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Titulo          | "NOVAS DECISOES"                                         |
| Icone           | Icone de coracao/decisao (rosa)                           |
| Valor principal | Numero inteiro (ex: `89`)                                 |
| Indicador       | `Recorde Semanal: Ha 2 dias` em destaque                  |

**Calculo:**
```
Novas Decisoes = COUNT(historico_mudancas WHERE campo = 'interesse'
                  AND valor_novo = 'Quente' AND periodo)
```

**Endpoint:** `GET /api/v1/relatorios/kpis?periodo=este-mes&setor=todos`

---

### 2.5 Grafico: Retencao ao Longo do Tempo

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Tipo de grafico | Barras verticais                                         |
| Eixo X          | Meses (S1, S2, S3, S4 ... S7) ou semanas                 |
| Eixo Y          | Percentual de retencao (0-100%)                           |
| Cor das barras  | Azul com gradiente                                        |
| Tooltip         | Ao passar o mouse: "Semana X: Y% retencao"               |
| Icone info      | (i) no canto - tooltip com explicacao do calculo          |

**Endpoint:** `GET /api/v1/relatorios/retencao-temporal?periodo=6-meses&agrupamento=mensal`

**Response:**
```json
{
  "dados": [
    { "periodo": "2025-09", "label": "S1", "percentual": 68 },
    { "periodo": "2025-10", "label": "S2", "percentual": 72 },
    { "periodo": "2025-11", "label": "S3", "percentual": 70 },
    { "periodo": "2025-12", "label": "S4", "percentual": 75 },
    { "periodo": "2026-01", "label": "S5", "percentual": 73 },
    { "periodo": "2026-02", "label": "S6", "percentual": 78 },
    { "periodo": "2026-03", "label": "S7", "percentual": 74 }
  ]
}
```

---

### 2.6 Grafico: Crescimento - Metabolismo da Alma

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Tipo de grafico | Barras horizontais empilhadas                            |
| Categorias (Y)  | Integracao (Fase 1), Consolidacao (Fase 2), Discipulado (Fase 3), Envio/Lideranca (Fase 4) |
| Eixo X          | Percentual (0-100%)                                       |
| Cores           | Azul = Maturidade, Azul claro = Novos                     |
| Legenda         | ● Maturidade  ● Novos                                    |

**Mapeamento de Fases para o modelo de dados:**

| Fase no wireframe         | Mapeamento no sistema                          |
|---------------------------|------------------------------------------------|
| Integracao (Fase 1)       | `crescimento = Novo` AND `status = NovoContato ou PrimeiraVisita` |
| Consolidacao (Fase 2)     | `crescimento = Crescendo` AND `status = EmAcompanhamento` |
| Discipulado (Fase 3)      | `crescimento = Firme` AND `status = EmAcompanhamento` |
| Envio/Lideranca (Fase 4)  | `crescimento = Firme` AND cuidador ativo (potenciais lideres) |

**Endpoint:** `GET /api/v1/relatorios/crescimento-fases?periodo=este-mes`

**Response:**
```json
{
  "fases": [
    { "fase": "Integracao", "numero": 1, "percentualMaturidade": 88, "percentualNovos": 12, "total": 245 },
    { "fase": "Consolidacao", "numero": 2, "percentualMaturidade": 62, "percentualNovos": 38, "total": 189 },
    { "fase": "Discipulado", "numero": 3, "percentualMaturidade": 45, "percentualNovos": 55, "total": 134 },
    { "fase": "Envio/Lideranca", "numero": 4, "percentualMaturidade": 21, "percentualNovos": 79, "total": 56 }
  ]
}
```

---

### 2.7 Tabela: Resumo por Lideranca e Transicoes

| Coluna           | Tipo            | Dados                                       |
|------------------|-----------------|---------------------------------------------|
| Lider de Area    | Avatar + Nome   | Iniciais coloridas + nome do cuidador        |
| Total de Visitas | Numero          | Total de acompanhamentos tipo Visita/Presencial |
| Conversoes       | Numero          | Mudancas de interesse para "Quente"          |
| Status Anterior  | Badge           | Status mais frequente de origem              |
| Status Atual     | Badge link      | Status mais frequente atual (clicavel)       |
| Acoes            | IconButton      | Icone para ver detalhes do cuidador          |

**Exemplo de dados:**

| Lider              | Total Visitas | Conversoes | Status Anterior  | Status Atual       |
|--------------------|---------------|------------|------------------|--------------------|
| MA - Marcos Andrade| 42            | -          | Novo             | → Consolidacao     |
| LN - Lucia Nogueira| 38           | -          | Consolidacao     | → Discipulado      |
| RP - Ricardo Piva  | 29            | -          | -                | → Consolidacao     |

**Link "Ver tudo"**: navega para listagem completa de cuidadores com metricas.

**Endpoint:** `GET /api/v1/relatorios/lideranca-transicoes?periodo=este-mes&page=1&pageSize=10`

**Response:**
```json
{
  "data": [
    {
      "cuidadorId": "bbbb0001-...",
      "nome": "Marcos Andrade",
      "iniciais": "MA",
      "cor": "#4A90D9",
      "totalVisitas": 42,
      "conversoes": 5,
      "statusAnteriorFrequente": "NovoContato",
      "statusAtualFrequente": "EmAcompanhamento"
    }
  ],
  "pagination": { "page": 1, "pageSize": 10, "totalItems": 8 }
}
```

---

### 2.8 Banner: Insight da IA Torre

| Propriedade     | Valor                                                     |
|-----------------|-----------------------------------------------------------|
| Icone           | Icone de lampada/IA (azul)                                |
| Titulo          | "Insight da IA Torre"                                    |
| Texto           | Resumo gerado com destaque em metricas e sugestoes        |
| Botao           | "Ver Plano Detalhado" → abre detalhamento                 |
| Background      | Azul claro (destaque sutil)                               |

**Exemplo de insight:**
> "O Setor Norte apresentou um crescimento de 24% em transicoes para Discipulado este mes. Considere replicar o modelo de treinamento em outros setores."

**Nota para implementacao:** Este componente pode ser estatico (regras pre-definidas) no MVP, com evolucao para IA generativa em fases futuras. No MVP, usar regras como:
- Se retencao caiu > 5%: alerta de queda
- Se cuidador com > 80% ocupacao: sugestao de balanceamento
- Se setor com maior crescimento: destaque positivo

**Endpoint (futuro):** `GET /api/v1/relatorios/insights`

---

### 2.9 Botao "+ Novo Relatorio"

Abre wizard para criar relatorio customizado:

**Passo 1 - Selecionar tipo:**
- Relatorio de retencao
- Relatorio de crescimento
- Relatorio de capacidade
- Relatorio de acompanhamentos
- Relatorio customizado

**Passo 2 - Configurar filtros:**
- Periodo, setor, status, cuidador

**Passo 3 - Preview e salvar:**
- Visualizar antes de salvar
- Nomear o relatorio
- Opcionalmente agendar envio periodico por email

---

## 3. Acoes de Exportacao

### 3.1 Exportar PDF

| Elemento         | Descricao                                                 |
|------------------|-----------------------------------------------------------|
| Conteudo         | KPIs + Graficos + Tabela, formatados para impressao       |
| Cabecalho PDF    | Logo + Nome da Igreja + Periodo + Data de geracao         |
| Rodape PDF       | "Gerado por Torre de Controle - Central de Acolhimento"   |
| Endpoint         | `GET /api/v1/relatorios/exportar?formato=pdf&periodo=...` |

### 3.2 Compartilhar

Modal de compartilhamento:
```
┌─────────────────────────────────┐
│ Compartilhar Relatorio       X  │
├─────────────────────────────────┤
│ Enviar por e-mail:              │
│ [email@exemplo.com        ] [+] │
│                                  │
│ Ou copiar link:                  │
│ [https://...relatorio/abc] [📋] │
│                                  │
│ Permissao: [Somente leitura ▼]  │
│                                  │
│       [Cancelar]  [Enviar]       │
└─────────────────────────────────┘
```

---

## 4. Endpoints Novos Necessarios

| Endpoint                                      | Descricao                                | Novo? |
|-----------------------------------------------|------------------------------------------|-------|
| `GET /api/v1/relatorios/kpis`                 | KPIs de relatorio com filtros            | Sim   |
| `GET /api/v1/relatorios/retencao-temporal`    | Retencao ao longo do tempo               | Sim   |
| `GET /api/v1/relatorios/crescimento-fases`    | Crescimento por fases espirituais        | Sim   |
| `GET /api/v1/relatorios/lideranca-transicoes` | Tabela de lideranca e transicoes         | Sim   |
| `GET /api/v1/relatorios/insights`             | Insights automaticos (futuro)            | Sim   |
| `GET /api/v1/relatorios/exportar`             | Exportacao em PDF/CSV                    | Sim   |
| `POST /api/v1/relatorios`                     | Salvar relatorio customizado             | Sim   |
| `GET /api/v1/relatorios`                      | Listar relatorios salvos                 | Sim   |
