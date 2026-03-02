# Modelo de Dados - Central de Acolhimento

## 1. Diagrama Entidade-Relacionamento (Textual)

```
┌──────────────┐       ┌───────────────┐       ┌──────────────────┐
│   IGREJAS    │       │   USUARIOS    │       │   CUIDADORES     │
├──────────────┤       ├───────────────┤       ├──────────────────┤
│ id (PK)      │◄──┐   │ id (PK)       │       │ id (PK)          │
│ nome         │   │   │ nome          │   ┌──►│ usuario_id (FK)  │
│ endereco     │   ├───│ igreja_id(FK) │   │   │ igreja_id (FK)───┤──►
│ cidade       │   │   │ email         │───┘   │ whatsapp         │
│ telefone     │   │   │ senha_hash    │       │ cidade           │
│ ativo        │   │   │ perfil        │       │ disponibilidade  │
│ created_at   │   │   │ ativo         │       │ capacidade_max   │
│ updated_at   │   │   │ created_at    │       │ ativo             │
└──────────────┘   │   │ updated_at    │       │ created_at       │
                   │   └───────────────┘       │ updated_at       │
                   │                           └────────┬─────────┘
                   │                                    │
                   │                                    │ 1:N
                   │                                    │
                   │   ┌───────────────────┐            │
                   │   │    ACOLHIDOS      │            │
                   │   ├───────────────────┤            │
                   │   │ id (PK)           │            │
                   ├───│ igreja_id (FK)    │            │
                   │   │ cuidador_id (FK)──┼────────────┘
                   │   │ nome_completo     │
                   │   │ whatsapp          │
                   │   │ bairro            │
                   │   │ cidade            │
                   │   │ quem_convidou     │
                   │   │ interesse         │  ── Frio | Morno | Quente
                   │   │ status            │  ── NovoContato | PrimeiraVisita |
                   │   │                   │     EmAcompanhamento | Desativada
                   │   │ crescimento       │  ── Novo | Crescendo | Firme
                   │   │ observacoes       │
                   │   │ ativo             │
                   │   │ created_at        │
                   │   │ updated_at        │
                   │   └────────┬──────────┘
                   │            │
                   │            │ 1:N
                   │            │
                   │   ┌────────▼─────────────────┐
                   │   │   ACOMPANHAMENTOS        │
                   │   ├──────────────────────────┤
                   │   │ id (PK)                  │
                   │   │ acolhido_id (FK)         │
                   │   │ cuidador_id (FK)         │
                   ├───│ igreja_id (FK)           │
                       │ data_contato             │
                       │ tipo_contato             │  ── WhatsApp | Visita |
                       │                          │     Ligacao | Presencial
                       │ observacoes              │
                       │ created_at               │
                       └──────────────────────────┘

                       ┌──────────────────────────┐
                       │   HISTORICO_MUDANCAS      │
                       ├──────────────────────────┤
                       │ id (PK)                  │
                       │ acolhido_id (FK)         │
                       │ usuario_id (FK)          │  ── quem fez a mudanca
                       │ campo                    │  ── interesse|status|crescimento|cuidador
                       │ valor_anterior           │
                       │ valor_novo               │
                       │ motivo                   │  ── opcional
                       │ created_at               │
                       └──────────────────────────┘
```

## 2. Detalhamento das Tabelas

### 2.1 igrejas

| Coluna     | Tipo         | Restricoes               | Descricao                  |
|------------|--------------|--------------------------|----------------------------|
| id         | UUID         | PK, auto-gerado         | Identificador unico        |
| nome       | VARCHAR(200) | NOT NULL                 | Nome da igreja             |
| endereco   | VARCHAR(500) | NULL                     | Endereco completo          |
| cidade     | VARCHAR(100) | NOT NULL                 | Cidade                     |
| telefone   | VARCHAR(20)  | NULL                     | Telefone de contato        |
| ativo      | BOOLEAN      | NOT NULL, DEFAULT true   | Se a igreja esta ativa     |
| created_at | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de criacao            |
| updated_at | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de ultima atualizacao |

### 2.2 usuarios

| Coluna     | Tipo         | Restricoes               | Descricao                 |
|------------|--------------|--------------------------|---------------------------|
| id         | UUID         | PK, auto-gerado         | Identificador unico       |
| igreja_id  | UUID         | FK(igrejas), NULL*       | Igreja vinculada          |
| nome       | VARCHAR(200) | NOT NULL                 | Nome completo             |
| email      | VARCHAR(200) | NOT NULL, UNIQUE         | E-mail de login           |
| senha_hash | VARCHAR(500) | NOT NULL                 | Hash bcrypt da senha      |
| perfil     | SMALLINT     | NOT NULL                 | 0=Admin, 1=Coordenador, 2=Cuidador |
| ativo      | BOOLEAN      | NOT NULL, DEFAULT true   | Se o usuario esta ativo   |
| created_at | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de criacao           |
| updated_at | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de ultima atualizacao|

*Admin pode ter `igreja_id` NULL (acesso global).

### 2.3 cuidadores

| Coluna          | Tipo         | Restricoes               | Descricao                 |
|-----------------|--------------|--------------------------|---------------------------|
| id              | UUID         | PK, auto-gerado         | Identificador unico       |
| usuario_id      | UUID         | FK(usuarios), UNIQUE     | Usuario vinculado         |
| igreja_id       | UUID         | FK(igrejas), NOT NULL    | Igreja vinculada          |
| whatsapp        | VARCHAR(20)  | NOT NULL                 | Numero WhatsApp           |
| cidade          | VARCHAR(100) | NOT NULL                 | Cidade                    |
| disponibilidade | BOOLEAN      | NOT NULL, DEFAULT true   | Se esta disponivel        |
| capacidade_max  | INT          | NOT NULL, DEFAULT 5      | Maximo de acolhidos       |
| ativo           | BOOLEAN      | NOT NULL, DEFAULT true   | Se o cuidador esta ativo  |
| created_at      | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de criacao           |
| updated_at      | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de ultima atualizacao|

**Indices:**
- UNIQUE(igreja_id, whatsapp) - evita duplicata na mesma igreja

### 2.4 acolhidos

| Coluna         | Tipo         | Restricoes               | Descricao                         |
|----------------|--------------|--------------------------|-----------------------------------|
| id             | UUID         | PK, auto-gerado         | Identificador unico               |
| igreja_id      | UUID         | FK(igrejas), NOT NULL    | Igreja vinculada                  |
| cuidador_id    | UUID         | FK(cuidadores), NULL     | Cuidador atribuido (pode ser null)|
| nome_completo  | VARCHAR(200) | NOT NULL                 | Nome completo                     |
| whatsapp       | VARCHAR(20)  | NOT NULL                 | Numero WhatsApp                   |
| bairro         | VARCHAR(100) | NOT NULL                 | Bairro                            |
| cidade         | VARCHAR(100) | NOT NULL                 | Cidade                            |
| quem_convidou  | VARCHAR(200) | NULL                     | Nome de quem convidou             |
| interesse      | SMALLINT     | NOT NULL, DEFAULT 0      | 0=Frio, 1=Morno, 2=Quente        |
| status         | SMALLINT     | NOT NULL, DEFAULT 0      | 0=NovoContato, 1=PrimeiraVisita, 2=EmAcompanhamento, 3=Desativada |
| crescimento    | SMALLINT     | NOT NULL, DEFAULT 0      | 0=Novo, 1=Crescendo, 2=Firme     |
| observacoes    | TEXT         | NULL                     | Observacoes iniciais              |
| ativo          | BOOLEAN      | NOT NULL, DEFAULT true   | Se o registro esta ativo          |
| created_at     | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de criacao                   |
| updated_at     | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de ultima atualizacao        |

**Indices:**
- UNIQUE(igreja_id, whatsapp) - evita duplicata na mesma igreja
- INDEX(igreja_id, status)
- INDEX(igreja_id, interesse)
- INDEX(igreja_id, crescimento)
- INDEX(cuidador_id)

### 2.5 acompanhamentos

| Coluna        | Tipo         | Restricoes               | Descricao                     |
|---------------|--------------|--------------------------|-------------------------------|
| id            | UUID         | PK, auto-gerado         | Identificador unico           |
| acolhido_id   | UUID         | FK(acolhidos), NOT NULL  | Acolhido acompanhado          |
| cuidador_id   | UUID         | FK(cuidadores), NOT NULL | Cuidador que fez o contato    |
| igreja_id     | UUID         | FK(igrejas), NOT NULL    | Igreja (para query filter)    |
| data_contato  | DATE         | NOT NULL                 | Data do contato               |
| tipo_contato  | SMALLINT     | NOT NULL                 | 0=WhatsApp, 1=Visita, 2=Ligacao, 3=Presencial |
| observacoes   | TEXT         | NULL                     | Observacoes do contato        |
| created_at    | TIMESTAMP    | NOT NULL, DEFAULT NOW()  | Data de criacao               |

**Indices:**
- INDEX(acolhido_id, data_contato DESC)
- INDEX(igreja_id)
- INDEX(cuidador_id)

### 2.6 historico_mudancas

| Coluna         | Tipo         | Restricoes              | Descricao                          |
|----------------|--------------|-------------------------|------------------------------------|
| id             | UUID         | PK, auto-gerado        | Identificador unico                |
| acolhido_id    | UUID         | FK(acolhidos), NOT NULL | Acolhido afetado                   |
| usuario_id     | UUID         | FK(usuarios), NOT NULL  | Usuario que fez a alteracao        |
| campo          | VARCHAR(50)  | NOT NULL                | Campo alterado (interesse, status, crescimento, cuidador) |
| valor_anterior | VARCHAR(100) | NOT NULL                | Valor antes da mudanca             |
| valor_novo     | VARCHAR(100) | NOT NULL                | Valor depois da mudanca            |
| motivo         | TEXT         | NULL                    | Motivo da mudanca (opcional)       |
| created_at     | TIMESTAMP    | NOT NULL, DEFAULT NOW() | Data da mudanca                    |

**Indices:**
- INDEX(acolhido_id, created_at DESC)

---

## 3. Relacionamentos

| Origem          | Destino         | Tipo | Descricao                                  |
|-----------------|-----------------|------|--------------------------------------------|
| usuarios        | igrejas         | N:1  | Cada usuario pertence a uma igreja         |
| cuidadores      | usuarios        | 1:1  | Cada cuidador tem um usuario               |
| cuidadores      | igrejas         | N:1  | Cada cuidador pertence a uma igreja        |
| acolhidos       | igrejas         | N:1  | Cada acolhido pertence a uma igreja        |
| acolhidos       | cuidadores      | N:1  | Cada acolhido pode ter um cuidador         |
| acompanhamentos | acolhidos       | N:1  | Cada acompanhamento refere-se a um acolhido|
| acompanhamentos | cuidadores      | N:1  | Cada acompanhamento foi feito por um cuidador|
| historico       | acolhidos       | N:1  | Cada historico refere-se a um acolhido     |
| historico       | usuarios        | N:1  | Cada historico foi feito por um usuario    |

---

## 4. Enums Mapeados

### EstadoInteresse
| Valor | Nome   | Descricao                              |
|-------|--------|----------------------------------------|
| 0     | Frio   | Sem demonstracao de interesse ativo    |
| 1     | Morno  | Algum interesse, ainda em avaliacao    |
| 2     | Quente | Forte interesse, engajado             |

### StatusAcompanhamento
| Valor | Nome               | Descricao                              |
|-------|--------------------|----------------------------------------|
| 0     | NovoContato        | Recem cadastrado, sem contato ainda    |
| 1     | PrimeiraVisita     | Primeiro contato/visita realizado      |
| 2     | EmAcompanhamento   | Sendo acompanhado regularmente         |
| 3     | Desativada         | Acompanhamento encerrado               |

### CrescimentoAlma
| Valor | Nome      | Descricao                               |
|-------|-----------|-----------------------------------------|
| 0     | Novo      | Inicio da jornada espiritual            |
| 1     | Crescendo | Em processo de crescimento              |
| 2     | Firme     | Firme na fe, crescimento consolidado    |

### TipoContato
| Valor | Nome       | Descricao                               |
|-------|------------|-----------------------------------------|
| 0     | WhatsApp   | Contato via WhatsApp                    |
| 1     | Visita     | Visita presencial na casa               |
| 2     | Ligacao    | Ligacao telefonica                      |
| 3     | Presencial | Encontro presencial (igreja, cafe, etc) |

### PerfilUsuario
| Valor | Nome         | Descricao                                |
|-------|--------------|------------------------------------------|
| 0     | Admin        | Acesso total, multi-igreja               |
| 1     | Coordenador  | Gestao completa de uma igreja            |
| 2     | Cuidador     | Acesso aos seus acolhidos                |
