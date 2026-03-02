# Contratos da API - Central de Acolhimento

Base URL: `/api/v1`

Autenticacao: Bearer JWT em todas as rotas (exceto login).

---

## 1. Autenticacao

### POST /api/v1/auth/login

**Request:**
```json
{
  "email": "carlos@igrejabatista.com",
  "senha": "Coord@123"
}
```

**Response 200:**
```json
{
  "accessToken": "eyJhbGciOi...",
  "refreshToken": "dGhpcyBpcyBh...",
  "expiresIn": 900,
  "usuario": {
    "id": "aaaa0002-...",
    "nome": "Pastor Carlos Silva",
    "email": "carlos@igrejabatista.com",
    "perfil": "Coordenador",
    "igrejaId": "11111111-...",
    "igrejaNome": "Igreja Batista Central"
  }
}
```

**Response 401:**
```json
{
  "error": "Credenciais invalidas"
}
```

### POST /api/v1/auth/refresh

**Request:**
```json
{
  "refreshToken": "dGhpcyBpcyBh..."
}
```

**Response 200:**
```json
{
  "accessToken": "eyJhbGciOi...",
  "refreshToken": "bmV3IHJlZnJl...",
  "expiresIn": 900
}
```

---

## 2. Igrejas (Admin)

### GET /api/v1/igrejas
Lista todas as igrejas.

**Query params:** `?ativo=true&page=1&pageSize=20&search=batista`

**Response 200:**
```json
{
  "data": [
    {
      "id": "11111111-...",
      "nome": "Igreja Batista Central",
      "cidade": "Sao Paulo",
      "telefone": "(11) 3000-0001",
      "ativo": true,
      "totalCuidadores": 2,
      "totalAcolhidos": 4
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 3,
    "totalPages": 1
  }
}
```

### POST /api/v1/igrejas

**Request:**
```json
{
  "nome": "Nova Igreja",
  "endereco": "Rua Nova, 10",
  "cidade": "Sao Paulo",
  "telefone": "(11) 3000-9999"
}
```

**Response 201:**
```json
{
  "id": "44444444-...",
  "nome": "Nova Igreja",
  "endereco": "Rua Nova, 10",
  "cidade": "Sao Paulo",
  "telefone": "(11) 3000-9999",
  "ativo": true,
  "createdAt": "2026-03-01T10:00:00Z"
}
```

### PUT /api/v1/igrejas/{id}

### DELETE /api/v1/igrejas/{id}
Soft delete (ativo = false).

---

## 3. Acolhidos

### GET /api/v1/acolhidos
Lista acolhidos da igreja do usuario autenticado.

**Query params:**
```
?status=0,1,2          # Filtro por status (multiplos)
&interesse=1,2         # Filtro por interesse
&crescimento=0,1       # Filtro por crescimento
&cuidadorId=bbbb0001   # Filtro por cuidador
&semCuidador=true      # Apenas sem cuidador
&search=joao           # Busca por nome ou whatsapp
&page=1&pageSize=20
&orderBy=nome&order=asc
```

**Response 200:**
```json
{
  "data": [
    {
      "id": "cccc0001-...",
      "nomeCompleto": "Joao Pedro Almeida",
      "whatsapp": "(11) 98888-0001",
      "bairro": "Centro",
      "cidade": "Sao Paulo",
      "quemConvidou": "Maria Santos",
      "interesse": "Quente",
      "status": "EmAcompanhamento",
      "crescimento": "Crescendo",
      "observacoes": "Veio pela primeira vez...",
      "cuidador": {
        "id": "bbbb0001-...",
        "nome": "Maria Santos"
      },
      "ultimoContato": "2026-02-27",
      "diasSemContato": 2,
      "createdAt": "2026-02-18T08:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 4,
    "totalPages": 1
  }
}
```

### GET /api/v1/acolhidos/{id}
Retorna detalhes completos incluindo historico.

**Response 200:**
```json
{
  "id": "cccc0001-...",
  "nomeCompleto": "Joao Pedro Almeida",
  "whatsapp": "(11) 98888-0001",
  "bairro": "Centro",
  "cidade": "Sao Paulo",
  "quemConvidou": "Maria Santos",
  "interesse": "Quente",
  "status": "EmAcompanhamento",
  "crescimento": "Crescendo",
  "observacoes": "Veio pela primeira vez...",
  "cuidador": {
    "id": "bbbb0001-...",
    "nome": "Maria Santos",
    "whatsapp": "(11) 99999-0001"
  },
  "acompanhamentos": [
    {
      "id": "...",
      "dataContato": "2026-02-27",
      "tipoContato": "Presencial",
      "observacoes": "Veio ao culto de quarta...",
      "cuidadorNome": "Maria Santos"
    }
  ],
  "historico": [
    {
      "id": "...",
      "campo": "interesse",
      "valorAnterior": "Morno",
      "valorNovo": "Quente",
      "usuarioNome": "Maria Santos",
      "createdAt": "2026-02-27T20:00:00Z"
    }
  ],
  "createdAt": "2026-02-18T08:00:00Z",
  "updatedAt": "2026-02-27T20:00:00Z"
}
```

### POST /api/v1/acolhidos

**Request:**
```json
{
  "nomeCompleto": "Novo Acolhido",
  "whatsapp": "(11) 98888-9999",
  "bairro": "Liberdade",
  "cidade": "Sao Paulo",
  "quemConvidou": "Jose Ferreira",
  "interesse": 0,
  "observacoes": "Chegou timido mas receptivo.",
  "cuidadorId": null
}
```

**Response 201:**
```json
{
  "id": "cccc9999-...",
  "nomeCompleto": "Novo Acolhido",
  "status": "NovoContato",
  "crescimento": "Novo",
  "interesse": "Frio",
  "createdAt": "2026-03-01T10:00:00Z"
}
```

**Response 409 (duplicata):**
```json
{
  "error": "Ja existe um acolhido com este WhatsApp nesta igreja",
  "existingId": "cccc0003-..."
}
```

### PUT /api/v1/acolhidos/{id}

### PATCH /api/v1/acolhidos/{id}/status

**Request:**
```json
{
  "status": 3,
  "motivo": "Mudou de cidade"
}
```

### PATCH /api/v1/acolhidos/{id}/cuidador

**Request:**
```json
{
  "cuidadorId": "bbbb0002-..."
}
```

**Response 422 (capacidade esgotada):**
```json
{
  "error": "Cuidador atingiu a capacidade maxima",
  "capacidadeAtual": 3,
  "capacidadeMaxima": 3
}
```

---

## 4. Cuidadores

### GET /api/v1/cuidadores
Lista cuidadores da igreja do usuario autenticado.

**Query params:**
```
?disponivel=true
&search=maria
&page=1&pageSize=20
```

**Response 200:**
```json
{
  "data": [
    {
      "id": "bbbb0001-...",
      "nome": "Maria Santos",
      "whatsapp": "(11) 99999-0001",
      "cidade": "Sao Paulo",
      "disponibilidade": true,
      "capacidadeMax": 5,
      "acolhidosAtivos": 3,
      "ocupacao": 60,
      "alertaSobrecarga": false
    }
  ],
  "pagination": { ... }
}
```

### GET /api/v1/cuidadores/{id}

### POST /api/v1/cuidadores

**Request:**
```json
{
  "nomeCompleto": "Novo Cuidador",
  "email": "novo@igrejabatista.com",
  "whatsapp": "(11) 99999-9999",
  "cidade": "Sao Paulo",
  "disponibilidade": true,
  "capacidadeMax": 5
}
```

*Nota: cria automaticamente um usuario com perfil Cuidador.*

### PUT /api/v1/cuidadores/{id}

### PATCH /api/v1/cuidadores/{id}/disponibilidade

**Request:**
```json
{
  "disponibilidade": false
}
```

### PATCH /api/v1/cuidadores/{id}/capacidade

**Request:**
```json
{
  "capacidadeMax": 8
}
```

---

## 5. Acompanhamentos

### GET /api/v1/acolhidos/{acolhidoId}/acompanhamentos
Lista acompanhamentos de um acolhido.

**Query params:** `?page=1&pageSize=20`

### POST /api/v1/acolhidos/{acolhidoId}/acompanhamentos

**Request:**
```json
{
  "dataContato": "2026-03-01",
  "tipoContato": 0,
  "observacoes": "Conversa longa via WhatsApp. Esta animado para o proximo culto.",
  "atualizarInteresse": 2,
  "atualizarCrescimento": null,
  "atualizarStatus": null
}
```

**Response 201:**
```json
{
  "id": "...",
  "dataContato": "2026-03-01",
  "tipoContato": "WhatsApp",
  "observacoes": "Conversa longa via WhatsApp...",
  "cuidadorNome": "Maria Santos",
  "mudancasRegistradas": [
    {
      "campo": "interesse",
      "de": "Morno",
      "para": "Quente"
    }
  ]
}
```

---

## 6. Dashboard

### GET /api/v1/dashboard
Retorna metricas da igreja do usuario autenticado.

**Response 200:**
```json
{
  "resumo": {
    "totalAcolhidos": 4,
    "totalCuidadores": 2,
    "cuidadoresDisponiveis": 2,
    "acolhidosSemCuidador": 1,
    "acolhidosSemContato7dias": 0,
    "acolhidosSemContato14dias": 1,
    "acolhidosSemContato30dias": 0
  },
  "porInteresse": {
    "frio": 1,
    "morno": 1,
    "quente": 2
  },
  "porCrescimento": {
    "novo": 2,
    "crescendo": 1,
    "firme": 1
  },
  "porStatus": {
    "novoContato": 2,
    "primeiraVisita": 1,
    "emAcompanhamento": 1,
    "desativada": 0
  },
  "cuidadores": [
    {
      "id": "bbbb0001-...",
      "nome": "Maria Santos",
      "ocupacao": 3,
      "capacidade": 5,
      "percentual": 60,
      "alertaSobrecarga": false
    },
    {
      "id": "bbbb0002-...",
      "nome": "Jose Ferreira",
      "ocupacao": 1,
      "capacidade": 3,
      "percentual": 33,
      "alertaSobrecarga": false
    }
  ]
}
```

---

## 7. Cuidador - Meus Acolhidos (Mobile)

### GET /api/v1/meus-acolhidos
Lista acolhidos atribuidos ao cuidador autenticado.

**Response 200:**
```json
{
  "data": [
    {
      "id": "cccc0001-...",
      "nomeCompleto": "Joao Pedro Almeida",
      "whatsapp": "(11) 98888-0001",
      "interesse": "Quente",
      "status": "EmAcompanhamento",
      "crescimento": "Crescendo",
      "ultimoContato": "2026-02-27",
      "diasSemContato": 2,
      "alertaContato": false
    }
  ]
}
```

---

## 8. Dashboard - Endpoints Detalhados (UI Torre de Controle)

> Endpoints adicionais para alimentar os componentes do Dashboard.
> Documentacao visual: [ui-dashboard.md](./ui-dashboard.md)

### GET /api/v1/dashboard/kpis
Retorna os 3 KPI cards do topo do Dashboard.

**Query params:** `?periodo=este-mes`

**Response 200:**
```json
{
  "taxaRetencao": {
    "percentual": 87.4,
    "variacao": 2.4,
    "direcao": "up"
  },
  "casasAtivas": {
    "percentual": 92,
    "meta": 95
  },
  "alertaInatividade": {
    "casosUrgentes": 14,
    "descricao": "Exige intervencao nas ultimas 48h"
  }
}
```

### GET /api/v1/dashboard/kanban
Retorna acolhidos agrupados por status para o board Kanban "Jornada Espiritual".

**Query params:** `?page=1&pageSize=20`

**Response 200:**
```json
{
  "novoContato": {
    "total": 4,
    "acolhidos": [
      {
        "id": "cccc0001-...",
        "nomeCompleto": "Ana Souza",
        "diasSemContato": 3,
        "corAlerta": "verde",
        "ultimaObservacao": "Pendente retorno",
        "whatsapp": "(11) 98888-0001",
        "interesse": "Morno",
        "cuidadorNome": "Maria Santos"
      }
    ]
  },
  "primeiraVisita": {
    "total": 2,
    "acolhidos": [...]
  },
  "emAcompanhamento": {
    "total": 8,
    "acolhidos": [...]
  }
}
```

### GET /api/v1/dashboard/capacidade
Retorna dados de capacidade dos cuidadores para o painel lateral.

**Response 200:**
```json
{
  "capacidadeTotal": {
    "ocupacao": 19,
    "capacidade": 25,
    "percentual": 76
  },
  "cuidadores": [
    {
      "id": "bbbb0001-...",
      "nome": "Joao Silva",
      "acolhidosAtivos": 1,
      "capacidadeMax": 5,
      "percentual": 20,
      "corBadge": "verde"
    },
    {
      "id": "bbbb0002-...",
      "nome": "Maria Oliveira",
      "acolhidosAtivos": 3,
      "capacidadeMax": 5,
      "percentual": 60,
      "corBadge": "amarelo"
    },
    {
      "id": "bbbb0003-...",
      "nome": "Pedro Albuquerque",
      "acolhidosAtivos": 5,
      "capacidadeMax": 5,
      "percentual": 100,
      "corBadge": "vermelho"
    }
  ]
}
```

### POST /api/v1/dashboard/balancear
Gera sugestoes de balanceamento de carga entre cuidadores.

**Response 200:**
```json
{
  "sugestoes": [
    {
      "acolhidoId": "cccc0003-...",
      "acolhidoNome": "Ricardo Souza",
      "deCuidadorId": "bbbb0003-...",
      "deCuidadorNome": "Pedro Albuquerque",
      "paraCuidadorId": "bbbb0001-...",
      "paraCuidadorNome": "Joao Silva",
      "motivo": "Cuidador origem em 100%, cuidador destino em 20%"
    }
  ]
}
```

---

## 9. Relatorios - Endpoints

> Endpoints para alimentar a tela de Relatorios de Gestao.
> Documentacao visual: [ui-relatorios.md](./ui-relatorios.md)

### GET /api/v1/relatorios/kpis
KPIs filtrados para a pagina de relatorios.

**Query params:** `?periodo=este-mes&setor=todos&tipo=todos&status=todos`

**Response 200:**
```json
{
  "totalAcolhidos": {
    "valor": 1284,
    "variacao": 12,
    "direcao": "up"
  },
  "taxaRetencao": {
    "percentual": 74.2,
    "meta": 80,
    "variacao": 1.8,
    "direcao": "up"
  },
  "visitasRealizadas": {
    "valor": 456,
    "metaDiaria": 15
  },
  "novasDecisoes": {
    "valor": 89,
    "recordeSemanal": "Ha 2 dias"
  }
}
```

### GET /api/v1/relatorios/retencao-temporal
Dados para grafico de retencao ao longo do tempo.

**Query params:** `?periodo=6-meses&agrupamento=mensal`

**Response 200:**
```json
{
  "dados": [
    { "periodo": "2025-09", "label": "Set", "percentual": 68 },
    { "periodo": "2025-10", "label": "Out", "percentual": 72 },
    { "periodo": "2025-11", "label": "Nov", "percentual": 70 },
    { "periodo": "2025-12", "label": "Dez", "percentual": 75 },
    { "periodo": "2026-01", "label": "Jan", "percentual": 73 },
    { "periodo": "2026-02", "label": "Fev", "percentual": 78 },
    { "periodo": "2026-03", "label": "Mar", "percentual": 74 }
  ]
}
```

### GET /api/v1/relatorios/crescimento-fases
Dados para grafico de metabolismo da alma por fases.

**Query params:** `?periodo=este-mes`

**Response 200:**
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

### GET /api/v1/relatorios/lideranca-transicoes
Dados para tabela de resumo por lideranca.

**Query params:** `?periodo=este-mes&page=1&pageSize=10`

**Response 200:**
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
  "pagination": { "page": 1, "pageSize": 10, "totalItems": 8, "totalPages": 1 }
}
```

### GET /api/v1/relatorios/exportar
Exporta relatorio em PDF ou CSV.

**Query params:** `?formato=pdf&periodo=este-mes&setor=todos`

**Response 200:** Arquivo binario (PDF ou CSV) com header `Content-Disposition: attachment`.

---

## 10. Cuidadores - Endpoints Detalhados

> Endpoints adicionais para a tela de Gestao de Cuidadores.
> Documentacao visual: [ui-cuidadores.md](./ui-cuidadores.md)

### GET /api/v1/cuidadores/{id}/atividades
Historico de acompanhamentos registrados pelo cuidador.

**Query params:** `?page=1&pageSize=5`

**Response 200:**
```json
{
  "data": [
    {
      "id": "...",
      "tipo": "Visita",
      "descricao": "Visita Finalizada",
      "data": "2026-02-15",
      "acolhidoNome": "Joao Pedro Almeida",
      "observacoes": "Conversa produtiva sobre crescimento espiritual"
    },
    {
      "id": "...",
      "tipo": "Relatorio",
      "descricao": "Relatorio de Visita Enviado",
      "data": "2026-02-14",
      "acolhidoNome": "Fernanda Costa",
      "observacoes": "Debates as 16:35"
    }
  ],
  "pagination": { "page": 1, "pageSize": 5, "totalItems": 24 }
}
```

### GET /api/v1/cuidadores/{id}/metricas
Metricas do cuidador para o Drawer de perfil.

**Response 200:**
```json
{
  "visitasEsteMes": 24,
  "totalAcolhidosAtivos": 6,
  "capacidadeMax": 8,
  "percentualOcupacao": 75,
  "ultimaAtividade": "2026-03-01T14:30:00Z",
  "tempoRelativo": "Ha 2h",
  "mediaVisitasPorSemana": 6.0
}
```

---

## 11. Codigos de Erro Padrao

| Codigo | Descricao                    |
|--------|------------------------------|
| 400    | Dados invalidos              |
| 401    | Nao autenticado              |
| 403    | Sem permissao                |
| 404    | Recurso nao encontrado       |
| 409    | Conflito (duplicata)         |
| 422    | Regra de negocio violada     |
| 429    | Rate limit excedido          |
| 500    | Erro interno do servidor     |

**Formato de erro padrao:**
```json
{
  "error": "Mensagem descritiva do erro",
  "details": [
    {
      "campo": "whatsapp",
      "mensagem": "Formato invalido. Use (XX) XXXXX-XXXX"
    }
  ]
}
```
