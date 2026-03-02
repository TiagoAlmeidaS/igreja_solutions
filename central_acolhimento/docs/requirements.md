# Requisitos - Central de Acolhimento

## 1. Requisitos Funcionais

### RF01 - Gestão de Igrejas
- RF01.1: O sistema deve permitir cadastrar igrejas com nome, endereço, cidade e contato.
- RF01.2: Cada igreja deve ter ao menos um coordenador vinculado.
- RF01.3: Todos os dados (cuidadores, acolhidos, acompanhamentos) devem ser segregados por igreja.

### RF02 - Gestão de Coordenadores
- RF02.1: O coordenador deve ser vinculado a uma igreja.
- RF02.2: O coordenador deve ter acesso ao dashboard com visão geral dos cuidadores e acolhidos de sua igreja.
- RF02.3: O coordenador pode cadastrar, editar e desativar cuidadores.
- RF02.4: O coordenador pode cadastrar, editar e desativar pessoas acolhidas.
- RF02.5: O coordenador pode atribuir e reatribuir acolhidos a cuidadores.
- RF02.6: O coordenador pode visualizar relatórios e métricas.

### RF03 - Cadastro de Pessoas Acolhidas
- RF03.1: Campos obrigatórios: Nome Completo, WhatsApp, Bairro, Cidade.
- RF03.2: Campos opcionais: Quem convidou, Observações iniciais.
- RF03.3: Cada acolhido deve ter um **estado de interesse**: Frio, Morno, Quente.
- RF03.4: Cada acolhido deve ter um **status de acompanhamento**: Novo Contato, Primeira Visita, Em Acompanhamento, Desativada.
- RF03.5: Cada acolhido deve ter um **crescimento da alma**: Novo, Crescendo, Firme.
- RF03.6: O acolhido deve ser vinculado a uma igreja.
- RF03.7: O acolhido pode ser atribuído a um cuidador.

### RF04 - Cadastro de Cuidadores
- RF04.1: Campos obrigatórios: Nome Completo, WhatsApp, Igreja, Cidade.
- RF04.2: Cada cuidador deve ter um status de **disponibilidade**: Sim, Não.
- RF04.3: Cada cuidador deve ter uma **capacidade máxima** de acolhidos simultâneos.
- RF04.4: O sistema deve impedir atribuição de novos acolhidos quando a capacidade estiver esgotada.
- RF04.5: O cuidador deve ser vinculado a uma igreja.

### RF05 - Gestão de Acompanhamento
- RF05.1: O sistema deve registrar cada interação/contato entre cuidador e acolhido.
- RF05.2: Cada registro de acompanhamento deve conter: data, tipo de contato (WhatsApp, Visita, Ligação, Presencial), observações.
- RF05.3: O sistema deve permitir alterar o status de acompanhamento do acolhido.
- RF05.4: O sistema deve permitir alterar o estado de interesse do acolhido.
- RF05.5: O sistema deve permitir alterar o crescimento da alma do acolhido.
- RF05.6: Deve existir um histórico completo de mudanças de estado/status.

### RF06 - Gestão de Capacidade
- RF06.1: Cada cuidador deve ter um limite configurável de acolhidos (padrão: 5).
- RF06.2: O sistema deve exibir visualmente a ocupação de cada cuidador (ex: 3/5).
- RF06.3: Alertas devem ser exibidos quando um cuidador atingir 80% da capacidade.
- RF06.4: O sistema deve bloquear novas atribuições quando a capacidade estiver em 100%.
- RF06.5: O coordenador pode ajustar o limite de capacidade de cada cuidador.

### RF07 - Dashboard e Relatórios
- RF07.1: Dashboard com visão geral: total de acolhidos, cuidadores, por status, por interesse.
- RF07.2: Relatório de acolhidos por status de acompanhamento.
- RF07.3: Relatório de acolhidos por estado de interesse.
- RF07.4: Relatório de acolhidos por crescimento da alma.
- RF07.5: Relatório de capacidade dos cuidadores.
- RF07.6: Relatório de acolhidos sem cuidador atribuído.
- RF07.7: Relatório de acolhidos sem contato recente (últimos 7, 14, 30 dias).

### RF08 - Autenticação e Autorização
- RF08.1: Login por e-mail/senha.
- RF08.2: Perfis de acesso: Admin (multi-igreja), Coordenador (por igreja), Cuidador (somente seus acolhidos).
- RF08.3: Cuidadores podem visualizar e registrar acompanhamentos apenas de seus acolhidos.
- RF08.4: Coordenadores podem gerenciar todos os dados de sua igreja.
- RF08.5: Admin pode gerenciar todas as igrejas e usuários.

---

## 2. Requisitos Não-Funcionais

### RNF01 - Performance
- RNF01.1: API deve responder em menos de 500ms para operações CRUD simples.
- RNF01.2: Dashboard deve carregar em menos de 2 segundos.

### RNF02 - Segurança
- RNF02.1: Senhas armazenadas com hash bcrypt.
- RNF02.2: Autenticação via JWT com refresh token.
- RNF02.3: Dados segregados por igreja (multi-tenant por filtro).
- RNF02.4: HTTPS obrigatório em produção.
- RNF02.5: Rate limiting na API.

### RNF03 - Disponibilidade
- RNF03.1: SLA de 99% de uptime.
- RNF03.2: Deploy via Docker com health checks.

### RNF04 - Escalabilidade
- RNF04.1: Arquitetura stateless para escalar horizontalmente.
- RNF04.2: Banco de dados PostgreSQL com connection pooling.

### RNF05 - Usabilidade
- RNF05.1: Interface responsiva (mobile-first).
- RNF05.2: Acessível em navegadores modernos (Chrome, Firefox, Safari, Edge).
- RNF05.3: App mobile com suporte Android e iOS via Expo.

### RNF06 - Manutenibilidade
- RNF06.1: Código com cobertura de testes mínima de 80%.
- RNF06.2: Documentação de API via Swagger/OpenAPI.
- RNF06.3: Migrations versionadas para banco de dados.
