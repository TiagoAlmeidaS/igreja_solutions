# Casos de Uso - Central de Acolhimento

## Atores

| Ator         | Descrição                                                        |
|--------------|------------------------------------------------------------------|
| Admin        | Administrador geral do sistema, gerencia igrejas e coordenadores |
| Coordenador  | Líder de uma igreja, gerencia cuidadores e acolhidos             |
| Cuidador     | Membro que acompanha pessoas acolhidas                           |

---

## UC01 - Gerenciar Igreja

**Ator principal:** Admin

**Pré-condição:** Usuário autenticado como Admin.

**Fluxo principal:**
1. Admin acessa a lista de igrejas.
2. Admin clica em "Nova Igreja".
3. Sistema exibe formulário com: Nome, Endereço, Cidade, Contato.
4. Admin preenche e confirma.
5. Sistema valida e salva a igreja.
6. Sistema redireciona para a lista atualizada.

**Fluxos alternativos:**
- 5a. Dados inválidos: sistema exibe mensagens de erro nos campos.
- Admin pode editar ou desativar igrejas existentes.

---

## UC02 - Cadastrar Coordenador

**Ator principal:** Admin

**Pré-condição:** Ao menos uma igreja cadastrada.

**Fluxo principal:**
1. Admin acessa "Gerenciar Coordenadores".
2. Admin clica em "Novo Coordenador".
3. Sistema exibe formulário: Nome, E-mail, Senha, Igreja (select).
4. Admin preenche e confirma.
5. Sistema cria usuário com perfil Coordenador vinculado à igreja.

**Pós-condição:** Coordenador pode fazer login e gerenciar sua igreja.

---

## UC03 - Cadastrar Pessoa Acolhida

**Ator principal:** Coordenador

**Pré-condição:** Coordenador autenticado.

**Fluxo principal:**
1. Coordenador acessa "Pessoas Acolhidas".
2. Coordenador clica em "Novo Acolhido".
3. Sistema exibe formulário:
   - Nome Completo (obrigatório)
   - WhatsApp (obrigatório)
   - Bairro (obrigatório)
   - Cidade (obrigatório)
   - Quem convidou? (opcional)
   - Interesse: Frio | Morno | Quente (obrigatório)
   - Observações iniciais (opcional)
4. Coordenador preenche e confirma.
5. Sistema salva com status "Novo Contato" e crescimento "Novo".
6. Sistema vincula o acolhido à igreja do coordenador.

**Fluxos alternativos:**
- 3a. Coordenador pode opcionalmente já atribuir um cuidador neste momento.
- 5a. Se WhatsApp já existir para a mesma igreja, sistema alerta duplicidade.

---

## UC04 - Cadastrar Cuidador

**Ator principal:** Coordenador

**Pré-condição:** Coordenador autenticado.

**Fluxo principal:**
1. Coordenador acessa "Cuidadores".
2. Coordenador clica em "Novo Cuidador".
3. Sistema exibe formulário:
   - Nome Completo (obrigatório)
   - WhatsApp (obrigatório)
   - Cidade (obrigatório)
   - Disponibilidade: Sim | Não (obrigatório)
   - Capacidade máxima (número, padrão: 5)
4. Coordenador preenche e confirma.
5. Sistema salva o cuidador vinculado à igreja do coordenador.

---

## UC05 - Atribuir Acolhido a Cuidador

**Ator principal:** Coordenador

**Pré-condição:** Existem acolhidos sem cuidador e cuidadores disponíveis.

**Fluxo principal:**
1. Coordenador acessa lista de acolhidos sem cuidador.
2. Coordenador seleciona um acolhido.
3. Sistema exibe lista de cuidadores disponíveis com ocupação (ex: 2/5).
4. Coordenador seleciona um cuidador.
5. Sistema valida capacidade do cuidador.
6. Sistema atribui o acolhido ao cuidador.
7. Status do acolhido muda para "Primeira Visita" (se estava em "Novo Contato").

**Fluxos alternativos:**
- 5a. Cuidador em capacidade máxima: sistema bloqueia e exibe alerta.
- 5b. Cuidador indisponível: sistema exibe aviso mas permite forçar atribuição.

---

## UC06 - Registrar Acompanhamento

**Ator principal:** Cuidador

**Pré-condição:** Cuidador autenticado, com acolhidos atribuídos.

**Fluxo principal:**
1. Cuidador acessa "Meus Acolhidos".
2. Cuidador seleciona um acolhido.
3. Cuidador clica em "Registrar Contato".
4. Sistema exibe formulário:
   - Data do contato (padrão: hoje)
   - Tipo: WhatsApp | Visita | Ligação | Presencial
   - Observações
   - Atualizar interesse? (opcional): Frio | Morno | Quente
   - Atualizar crescimento? (opcional): Novo | Crescendo | Firme
   - Atualizar status? (opcional): Novo Contato | Primeira Visita | Em Acompanhamento | Desativada
5. Cuidador preenche e confirma.
6. Sistema salva o registro de acompanhamento.
7. Se houve alteração de interesse/crescimento/status, sistema registra no histórico.

---

## UC07 - Visualizar Dashboard

**Ator principal:** Coordenador

**Pré-condição:** Coordenador autenticado.

**Fluxo principal:**
1. Coordenador acessa o Dashboard.
2. Sistema exibe:
   - Total de acolhidos (com breakdown por status)
   - Total de cuidadores (disponíveis vs indisponíveis)
   - Gráfico de acolhidos por interesse (Frio/Morno/Quente)
   - Gráfico de acolhidos por crescimento (Novo/Crescendo/Firme)
   - Lista de cuidadores com capacidade (ocupação visual)
   - Alertas: acolhidos sem contato > 7 dias, cuidadores sobrecarregados

---

## UC08 - Reatribuir Acolhido

**Ator principal:** Coordenador

**Pré-condição:** Acolhido já atribuído a um cuidador.

**Fluxo principal:**
1. Coordenador acessa o perfil do acolhido.
2. Coordenador clica em "Reatribuir Cuidador".
3. Sistema exibe lista de cuidadores disponíveis com ocupação.
4. Coordenador seleciona novo cuidador.
5. Sistema valida capacidade e transfere o acolhido.
6. Sistema registra a mudança no histórico.

---

## UC09 - Desativar Acolhido

**Ator principal:** Coordenador

**Pré-condição:** Acolhido com status ativo.

**Fluxo principal:**
1. Coordenador acessa o perfil do acolhido.
2. Coordenador clica em "Desativar".
3. Sistema solicita motivo da desativação.
4. Coordenador informa o motivo.
5. Sistema altera status para "Desativada" e libera a vaga do cuidador.
6. Registro fica no histórico (soft delete).

---

## UC10 - Consultar Histórico de Acolhido

**Ator principal:** Coordenador, Cuidador

**Pré-condição:** Usuário autenticado com acesso ao acolhido.

**Fluxo principal:**
1. Usuário acessa o perfil do acolhido.
2. Sistema exibe timeline com:
   - Data de cadastro
   - Mudanças de status
   - Mudanças de interesse
   - Mudanças de crescimento
   - Atribuições/reatribuições de cuidador
   - Todos os registros de acompanhamento

---

## Matriz de Permissões

| Funcionalidade                | Admin | Coordenador | Cuidador |
|-------------------------------|-------|-------------|----------|
| Gerenciar igrejas             | Sim   | Nao         | Nao      |
| Gerenciar coordenadores       | Sim   | Nao         | Nao      |
| Cadastrar cuidadores          | Sim   | Sim         | Nao      |
| Cadastrar acolhidos           | Sim   | Sim         | Nao      |
| Atribuir acolhidos            | Sim   | Sim         | Nao      |
| Registrar acompanhamento      | Nao   | Sim         | Sim*     |
| Visualizar dashboard          | Sim   | Sim         | Nao      |
| Ver todos acolhidos da igreja | Sim   | Sim         | Nao      |
| Ver seus acolhidos            | Nao   | Nao         | Sim      |
| Alterar status/interesse      | Sim   | Sim         | Sim*     |

*Cuidador: somente para acolhidos atribuídos a ele.
