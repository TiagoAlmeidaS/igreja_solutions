# Fixtures e Dados Iniciais - Central de Acolhimento

## 1. Seeds de Desenvolvimento

Dados iniciais para ambiente de desenvolvimento e testes.

### 1.1 Igrejas

```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "nome": "Igreja Batista Central",
    "endereco": "Rua Principal, 100 - Centro",
    "cidade": "Sao Paulo",
    "telefone": "(11) 3000-0001",
    "ativo": true
  },
  {
    "id": "22222222-2222-2222-2222-222222222222",
    "nome": "Comunidade Crista Vida Nova",
    "endereco": "Av. Brasil, 500 - Jardim America",
    "cidade": "Campinas",
    "telefone": "(19) 3000-0002",
    "ativo": true
  },
  {
    "id": "33333333-3333-3333-3333-333333333333",
    "nome": "Igreja Presbiteriana Renovada",
    "endereco": "Rua da Paz, 250 - Boa Vista",
    "cidade": "Curitiba",
    "telefone": "(41) 3000-0003",
    "ativo": true
  }
]
```

### 1.2 Usuarios

```json
[
  {
    "id": "aaaa0001-0000-0000-0000-000000000001",
    "nome": "Administrador Geral",
    "email": "admin@centralacolhimento.com",
    "senha": "Admin@123",
    "perfil": "Admin",
    "igreja_id": null
  },
  {
    "id": "aaaa0002-0000-0000-0000-000000000002",
    "nome": "Pastor Carlos Silva",
    "email": "carlos@igrejabatista.com",
    "senha": "Coord@123",
    "perfil": "Coordenador",
    "igreja_id": "11111111-1111-1111-1111-111111111111"
  },
  {
    "id": "aaaa0003-0000-0000-0000-000000000003",
    "nome": "Pastora Ana Oliveira",
    "email": "ana@vidanova.com",
    "senha": "Coord@123",
    "perfil": "Coordenador",
    "igreja_id": "22222222-2222-2222-2222-222222222222"
  },
  {
    "id": "aaaa0004-0000-0000-0000-000000000004",
    "nome": "Maria Santos",
    "email": "maria@igrejabatista.com",
    "senha": "Cuid@123",
    "perfil": "Cuidador",
    "igreja_id": "11111111-1111-1111-1111-111111111111"
  },
  {
    "id": "aaaa0005-0000-0000-0000-000000000005",
    "nome": "Jose Ferreira",
    "email": "jose@igrejabatista.com",
    "senha": "Cuid@123",
    "perfil": "Cuidador",
    "igreja_id": "11111111-1111-1111-1111-111111111111"
  },
  {
    "id": "aaaa0006-0000-0000-0000-000000000006",
    "nome": "Lucia Mendes",
    "email": "lucia@vidanova.com",
    "senha": "Cuid@123",
    "perfil": "Cuidador",
    "igreja_id": "22222222-2222-2222-2222-222222222222"
  }
]
```

### 1.3 Cuidadores

```json
[
  {
    "id": "bbbb0001-0000-0000-0000-000000000001",
    "usuario_id": "aaaa0004-0000-0000-0000-000000000004",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "whatsapp": "(11) 99999-0001",
    "cidade": "Sao Paulo",
    "disponibilidade": true,
    "capacidade_max": 5
  },
  {
    "id": "bbbb0002-0000-0000-0000-000000000002",
    "usuario_id": "aaaa0005-0000-0000-0000-000000000005",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "whatsapp": "(11) 99999-0002",
    "cidade": "Sao Paulo",
    "disponibilidade": true,
    "capacidade_max": 3
  },
  {
    "id": "bbbb0003-0000-0000-0000-000000000003",
    "usuario_id": "aaaa0006-0000-0000-0000-000000000006",
    "igreja_id": "22222222-2222-2222-2222-222222222222",
    "whatsapp": "(19) 99999-0003",
    "cidade": "Campinas",
    "disponibilidade": true,
    "capacidade_max": 5
  }
]
```

### 1.4 Acolhidos

```json
[
  {
    "id": "cccc0001-0000-0000-0000-000000000001",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "nome_completo": "Joao Pedro Almeida",
    "whatsapp": "(11) 98888-0001",
    "bairro": "Centro",
    "cidade": "Sao Paulo",
    "quem_convidou": "Maria Santos",
    "interesse": "Quente",
    "status": "EmAcompanhamento",
    "crescimento": "Crescendo",
    "observacoes": "Veio pela primeira vez no culto de domingo. Demonstrou muito interesse."
  },
  {
    "id": "cccc0002-0000-0000-0000-000000000002",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "nome_completo": "Fernanda Costa",
    "whatsapp": "(11) 98888-0002",
    "bairro": "Vila Madalena",
    "cidade": "Sao Paulo",
    "quem_convidou": "Jose Ferreira",
    "interesse": "Morno",
    "status": "PrimeiraVisita",
    "crescimento": "Novo",
    "observacoes": "Convidada por um amigo. Ainda timida mas aberta."
  },
  {
    "id": "cccc0003-0000-0000-0000-000000000003",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "cuidador_id": "bbbb0002-0000-0000-0000-000000000002",
    "nome_completo": "Ricardo Souza",
    "whatsapp": "(11) 98888-0003",
    "bairro": "Pinheiros",
    "cidade": "Sao Paulo",
    "quem_convidou": null,
    "interesse": "Frio",
    "status": "NovoContato",
    "crescimento": "Novo",
    "observacoes": "Chegou sozinho. Parece estar passando por dificuldades."
  },
  {
    "id": "cccc0004-0000-0000-0000-000000000004",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "cuidador_id": null,
    "nome_completo": "Camila Rodrigues",
    "whatsapp": "(11) 98888-0004",
    "bairro": "Moema",
    "cidade": "Sao Paulo",
    "quem_convidou": "Maria Santos",
    "interesse": "Quente",
    "status": "NovoContato",
    "crescimento": "Novo",
    "observacoes": "Muito animada, pediu para participar do grupo de jovens."
  },
  {
    "id": "cccc0005-0000-0000-0000-000000000005",
    "igreja_id": "22222222-2222-2222-2222-222222222222",
    "cuidador_id": "bbbb0003-0000-0000-0000-000000000003",
    "nome_completo": "Paulo Henrique Lima",
    "whatsapp": "(19) 98888-0005",
    "bairro": "Cambuí",
    "cidade": "Campinas",
    "quem_convidou": "Lucia Mendes",
    "interesse": "Morno",
    "status": "EmAcompanhamento",
    "crescimento": "Crescendo",
    "observacoes": "Frequentando regularmente, participando dos estudos biblicos."
  },
  {
    "id": "cccc0006-0000-0000-0000-000000000006",
    "igreja_id": "11111111-1111-1111-1111-111111111111",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "nome_completo": "Beatriz Tavares",
    "whatsapp": "(11) 98888-0006",
    "bairro": "Itaim Bibi",
    "cidade": "Sao Paulo",
    "quem_convidou": null,
    "interesse": "Quente",
    "status": "EmAcompanhamento",
    "crescimento": "Firme",
    "observacoes": "Ja se batizou, firme na fe. Pode se tornar cuidadora em breve."
  }
]
```

### 1.5 Acompanhamentos

```json
[
  {
    "acolhido_id": "cccc0001-0000-0000-0000-000000000001",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "data_contato": "2026-02-20",
    "tipo_contato": "WhatsApp",
    "observacoes": "Primeiro contato via WhatsApp. Se mostrou muito receptivo e quer saber mais sobre os horarios dos cultos."
  },
  {
    "acolhido_id": "cccc0001-0000-0000-0000-000000000001",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "data_contato": "2026-02-23",
    "tipo_contato": "Visita",
    "observacoes": "Visitei na casa dele. Conversamos por cerca de 1h. Muito aberto e com muitas perguntas."
  },
  {
    "acolhido_id": "cccc0001-0000-0000-0000-000000000001",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "data_contato": "2026-02-27",
    "tipo_contato": "Presencial",
    "observacoes": "Veio ao culto de quarta. Participou ativamente. Interesse crescendo."
  },
  {
    "acolhido_id": "cccc0002-0000-0000-0000-000000000002",
    "cuidador_id": "bbbb0001-0000-0000-0000-000000000001",
    "data_contato": "2026-02-22",
    "tipo_contato": "WhatsApp",
    "observacoes": "Mandei mensagem de boas-vindas. Respondeu de forma educada mas breve."
  },
  {
    "acolhido_id": "cccc0005-0000-0000-0000-000000000005",
    "cuidador_id": "bbbb0003-0000-0000-0000-000000000003",
    "data_contato": "2026-02-18",
    "tipo_contato": "Presencial",
    "observacoes": "Participou do estudo biblico. Fez perguntas profundas. Crescimento visivel."
  },
  {
    "acolhido_id": "cccc0005-0000-0000-0000-000000000005",
    "cuidador_id": "bbbb0003-0000-0000-0000-000000000003",
    "data_contato": "2026-02-25",
    "tipo_contato": "Ligacao",
    "observacoes": "Liguei para ver como estava. Contou que tem orado diariamente."
  }
]
```

---

## 2. Cenarios de Teste

### Cenario 1: Cuidador no limite de capacidade
- Maria Santos (cuidador bbbb0001) tem `capacidade_max: 5` e 3 acolhidos atribuidos.
- Ocupacao: 3/5 (60%) - abaixo do alerta.

### Cenario 2: Acolhido sem cuidador
- Camila Rodrigues (cccc0004) esta sem cuidador atribuido.
- Deve aparecer no relatorio de "acolhidos sem cuidador".

### Cenario 3: Diversidade de estados
- Os dados cobrem todos os estados de interesse (Frio, Morno, Quente).
- Os dados cobrem todos os status (NovoContato, PrimeiraVisita, EmAcompanhamento).
- Os dados cobrem todos os crescimentos (Novo, Crescendo, Firme).

### Cenario 4: Multi-igreja
- Igreja Batista Central tem 2 cuidadores e 4 acolhidos.
- Comunidade Vida Nova tem 1 cuidador e 1 acolhido.
- Dados sao isolados entre as igrejas.

### Cenario 5: Candidata a cuidadora
- Beatriz Tavares esta "Firme" e com interesse "Quente".
- Cenario ideal para ser promovida a cuidadora futuramente.

---

## 3. SQL de Seed (PostgreSQL)

```sql
-- Executar apos as migrations

-- Igrejas
INSERT INTO igrejas (id, nome, endereco, cidade, telefone, ativo, created_at, updated_at)
VALUES
  ('11111111-1111-1111-1111-111111111111', 'Igreja Batista Central', 'Rua Principal, 100 - Centro', 'Sao Paulo', '(11) 3000-0001', true, NOW(), NOW()),
  ('22222222-2222-2222-2222-222222222222', 'Comunidade Crista Vida Nova', 'Av. Brasil, 500 - Jardim America', 'Campinas', '(19) 3000-0002', true, NOW(), NOW()),
  ('33333333-3333-3333-3333-333333333333', 'Igreja Presbiteriana Renovada', 'Rua da Paz, 250 - Boa Vista', 'Curitiba', '(41) 3000-0003', true, NOW(), NOW());

-- Usuarios (senhas sao hash bcrypt de 'Admin@123', 'Coord@123', 'Cuid@123')
-- NOTA: Em producao, gerar os hashes via aplicacao. Abaixo sao placeholders.
INSERT INTO usuarios (id, igreja_id, nome, email, senha_hash, perfil, ativo, created_at, updated_at)
VALUES
  ('aaaa0001-0000-0000-0000-000000000001', NULL, 'Administrador Geral', 'admin@centralacolhimento.com', '$HASH_ADMIN', 0, true, NOW(), NOW()),
  ('aaaa0002-0000-0000-0000-000000000002', '11111111-1111-1111-1111-111111111111', 'Pastor Carlos Silva', 'carlos@igrejabatista.com', '$HASH_COORD', 1, true, NOW(), NOW()),
  ('aaaa0003-0000-0000-0000-000000000003', '22222222-2222-2222-2222-222222222222', 'Pastora Ana Oliveira', 'ana@vidanova.com', '$HASH_COORD', 1, true, NOW(), NOW()),
  ('aaaa0004-0000-0000-0000-000000000004', '11111111-1111-1111-1111-111111111111', 'Maria Santos', 'maria@igrejabatista.com', '$HASH_CUID', 2, true, NOW(), NOW()),
  ('aaaa0005-0000-0000-0000-000000000005', '11111111-1111-1111-1111-111111111111', 'Jose Ferreira', 'jose@igrejabatista.com', '$HASH_CUID', 2, true, NOW(), NOW()),
  ('aaaa0006-0000-0000-0000-000000000006', '22222222-2222-2222-2222-222222222222', 'Lucia Mendes', 'lucia@vidanova.com', '$HASH_CUID', 2, true, NOW(), NOW());
```
