# Features - Central de Acolhimento

## Roadmap de Funcionalidades

### Fase 1 - MVP (Minimum Viable Product)

#### F01 - Autenticacao e Controle de Acesso
- [ ] Login com e-mail e senha
- [ ] JWT com refresh token
- [ ] Perfis: Admin, Coordenador, Cuidador
- [ ] Segregacao de dados por igreja (multi-tenant logico)
- [ ] Tela de "Esqueci minha senha"

#### F02 - Cadastro de Igrejas
- [ ] CRUD completo de igrejas (Admin)
- [ ] Campos: nome, endereco, cidade, telefone de contato
- [ ] Listagem com busca e filtros
- [ ] Ativar/Desativar igreja

#### F03 - Cadastro de Pessoas Acolhidas
- [ ] Formulario de cadastro com campos:
  - Nome Completo
  - WhatsApp (com mascara e validacao)
  - Bairro
  - Cidade
  - Quem convidou?
  - Interesse (Frio / Morno / Quente)
  - Observacoes iniciais
- [ ] Status automatico: "Novo Contato" ao cadastrar
- [ ] Crescimento automatico: "Novo" ao cadastrar
- [ ] Listagem com filtros por status, interesse, crescimento
- [ ] Busca por nome ou WhatsApp
- [ ] Deteccao de duplicatas por WhatsApp
- [ ] Edicao e desativacao (soft delete)

#### F04 - Cadastro de Cuidadores
- [ ] Formulario de cadastro com campos:
  - Nome Completo
  - WhatsApp
  - Cidade
  - Disponibilidade (Sim / Nao)
  - Capacidade maxima (padrao: 5)
- [ ] Vinculacao automatica a igreja do coordenador
- [ ] Listagem com filtros por disponibilidade e ocupacao
- [ ] Indicador visual de capacidade (ex: barra 3/5)
- [ ] Edicao e desativacao

#### F05 - Atribuicao de Acolhidos a Cuidadores
- [ ] Tela de atribuicao com lista de acolhidos sem cuidador
- [ ] Selecao de cuidador com visualizacao de capacidade
- [ ] Bloqueio quando cuidador esta em 100% da capacidade
- [ ] Alerta visual quando cuidador esta em 80%+
- [ ] Reatribuicao de acolhido para outro cuidador
- [ ] Liberacao automatica de vaga ao desativar acolhido

#### F06 - Registro de Acompanhamento
- [ ] Formulario de registro de contato:
  - Data (padrao: hoje)
  - Tipo: WhatsApp / Visita / Ligacao / Presencial
  - Observacoes (texto livre)
- [ ] Opcao de atualizar interesse, crescimento e status no mesmo formulario
- [ ] Timeline de historico por acolhido
- [ ] Registros ordenados por data (mais recente primeiro)

---

### Fase 2 - Dashboard e Relatorios

> Wireframes de referencia: [ui-dashboard.md](./ui-dashboard.md) | [ui-relatorios.md](./ui-relatorios.md) | [ui-cuidadores.md](./ui-cuidadores.md)

#### F07 - Dashboard Torre de Controle (Coordenador)
- [ ] Header com navegacao: Dashboard, Relatorios, Cuidadores, Configuracoes
- [ ] Busca global "Buscar convidado..." com autocomplete
- [ ] Icone de notificacoes com badge de contagem
- [ ] KPI Card: Taxa de Retencao TCI (percentual + variacao + barra)
- [ ] KPI Card: Casas Ativas (percentual + meta)
- [ ] KPI Card: Alerta de Inatividade (casos urgentes, clicavel)
- [ ] Kanban "Jornada Espiritual" com 3 colunas:
  - Novo Contato (com contagem)
  - Primeira Visita (com contagem)
  - Em Acompanhamento (com contagem)
- [ ] Cards de acolhido no Kanban: nome, dias sem contato (badge colorido), observacao, icones de acao (WhatsApp, telefone)
- [ ] Drag-and-drop entre colunas do Kanban (muda status com confirmacao)
- [ ] Botao "+ Novo Convidado" abrindo modal de cadastro rapido
- [ ] Painel lateral "Gestao de Capacidade":
  - Lista de cuidadores ativos com badge numerico colorido
  - Barra de capacidade total com percentual
  - Botao "Balancear Cargas" com sugestoes de redistribuicao
- [ ] Footer: status do servidor, ultima sincronizacao, links de ajuda

#### F08 - Relatorios de Gestao
- [ ] Sidebar de navegacao com busca de relatorios
- [ ] Barra de filtros: Periodo, Setor, Tipo Acolhimento, Status
- [ ] KPI Card: Total Acolhidos (valor + variacao vs anterior)
- [ ] KPI Card: Taxa de Retencao (percentual + meta + variacao)
- [ ] KPI Card: Visitas Realizadas (valor + meta diaria)
- [ ] KPI Card: Novas Decisoes (valor + recorde semanal)
- [ ] Grafico de barras: Retencao ao Longo do Tempo (mensal)
- [ ] Grafico de barras horizontais: Crescimento Metabolismo da Alma por fases (Integracao, Consolidacao, Discipulado, Envio/Lideranca)
- [ ] Tabela: Resumo por Lideranca e Transicoes (cuidador, visitas, conversoes, status anterior → atual)
- [ ] Banner "Insight da IA Torre" (regras pre-definidas no MVP)
- [ ] Exportacao em PDF com cabecalho da igreja
- [ ] Compartilhamento via email e link
- [ ] Botao "+ Novo Relatorio" com wizard de relatorio customizado

#### F15 - Gestao de Cuidadores (Tela Dedicada)
- [ ] Grid de cards de cuidadores (4 colunas desktop)
- [ ] Card do cuidador: avatar, nome, visitas ativas, ultima atividade, barra de ocupacao com label de estado
- [ ] Tabs de filtro rapido: Todos, Disponiveis, Em Alerta
- [ ] Ordenacao: por lista/grupo e por ocupacao
- [ ] Drawer lateral (slide-in) ao clicar no card:
  - Info basica (nome, status, email, telefone)
  - Escala de atividades (visitas no mes)
  - Historico recentes (ultimos 5 acompanhamentos)
  - Botao "Editar Perfil"
- [ ] Modal "+ Adicionar Novo Cuidador" com criacao automatica de usuario
- [ ] Card placeholder "+ Mover Cuidador" (transferencia entre grupos)
- [ ] Pesquisa de cuidador com filtro client-side em tempo real

---

### Fase 3 - Experiencia Mobile

#### F09 - App Mobile (Cuidador)
- [ ] Login
- [ ] Lista de "Meus Acolhidos"
- [ ] Perfil do acolhido com timeline
- [ ] Registro rapido de acompanhamento
- [ ] Notificacao: acolhido sem contato > 7 dias
- [ ] Botao direto para WhatsApp do acolhido

#### F10 - App Mobile (Coordenador)
- [ ] Dashboard simplificado
- [ ] Lista de acolhidos com filtros
- [ ] Atribuicao rapida
- [ ] Notificacoes de alertas

---

### Fase 4 - Evolucoes Futuras

#### F11 - Notificacoes e Lembretes
- [ ] Notificacao push (mobile)
- [ ] E-mail de lembrete para cuidadores
- [ ] Alerta no dashboard para coordenadores
- [ ] Resumo semanal por e-mail

#### F12 - Integracao WhatsApp
- [ ] Envio de mensagem template via Meta Cloud API
- [ ] Registro automatico de contato ao enviar mensagem
- [ ] Chatbot basico para acolhimento inicial

#### F13 - Relatorios Avancados
- [ ] Evolucao temporal dos acolhidos (grafico de linha)
- [ ] Taxa de conversao por interesse
- [ ] Tempo medio de acompanhamento
- [ ] Comparativo entre igrejas (Admin)

#### F14 - Gestao de Grupos/Celulas
- [ ] Cadastro de grupos/celulas por igreja
- [ ] Vinculacao de acolhidos a grupos
- [ ] Lider de grupo como perfil adicional

---

## Prioridades

| Feature | Prioridade | Fase | Estimativa | Wireframe                |
|---------|-----------|------|------------|--------------------------|
| F01     | Critica   | MVP  | 2 sprints  | -                        |
| F02     | Critica   | MVP  | 1 sprint   | -                        |
| F03     | Critica   | MVP  | 2 sprints  | -                        |
| F04     | Critica   | MVP  | 1 sprint   | -                        |
| F05     | Critica   | MVP  | 1 sprint   | -                        |
| F06     | Critica   | MVP  | 2 sprints  | -                        |
| F07     | Alta      | F2   | 3 sprints  | [ui-dashboard.md](./ui-dashboard.md)   |
| F08     | Alta      | F2   | 2 sprints  | [ui-relatorios.md](./ui-relatorios.md) |
| F09     | Media     | F3   | 3 sprints  | -                        |
| F10     | Media     | F3   | 2 sprints  | -                        |
| F11     | Baixa     | F4   | 2 sprints  | -                        |
| F12     | Baixa     | F4   | 3 sprints  | -                        |
| F13     | Baixa     | F4   | 2 sprints  | -                        |
| F14     | Baixa     | F4   | 2 sprints  | -                        |
| F15     | Alta      | F2   | 2 sprints  | [ui-cuidadores.md](./ui-cuidadores.md) |
