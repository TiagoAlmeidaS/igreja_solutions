# Regras de Negocio - Central de Acolhimento

## 1. Regras de Acolhidos

### RN01 - Cadastro de Acolhido
- RN01.1: Ao cadastrar um acolhido, o status inicial obrigatorio e `NovoContato`.
- RN01.2: Ao cadastrar um acolhido, o crescimento inicial obrigatorio e `Novo`.
- RN01.3: O interesse deve ser informado no cadastro (Frio, Morno ou Quente).
- RN01.4: WhatsApp deve ser unico por igreja. O mesmo WhatsApp pode existir em igrejas diferentes.
- RN01.5: O campo `quem_convidou` e texto livre e opcional.

### RN02 - Transicoes de Status
```
NovoContato ──► PrimeiraVisita ──► EmAcompanhamento ──► Desativada
     │                                                       ▲
     └───────────────────────────────────────────────────────┘
```
- RN02.1: `NovoContato` pode ir para `PrimeiraVisita` ou `Desativada`.
- RN02.2: `PrimeiraVisita` pode ir para `EmAcompanhamento` ou `Desativada`.
- RN02.3: `EmAcompanhamento` pode ir para `Desativada`.
- RN02.4: `Desativada` pode ser reativada para `NovoContato` (reingresso).
- RN02.5: Toda mudanca de status deve registrar historico com data, usuario e motivo (se desativacao).

### RN03 - Transicoes de Interesse
- RN03.1: O interesse pode ser alterado livremente entre `Frio`, `Morno` e `Quente`.
- RN03.2: Nao ha restricao de transicao (pode regredir de Quente para Frio).
- RN03.3: Toda mudanca de interesse deve registrar historico.

### RN04 - Transicoes de Crescimento da Alma
- RN04.1: O crescimento pode ser alterado livremente entre `Novo`, `Crescendo` e `Firme`.
- RN04.2: Nao ha restricao de transicao (pode regredir de Firme para Novo).
- RN04.3: Toda mudanca de crescimento deve registrar historico.

### RN05 - Desativacao
- RN05.1: Ao desativar um acolhido, e obrigatorio informar o motivo.
- RN05.2: Ao desativar, o acolhido e desvinculado do cuidador (libera vaga).
- RN05.3: Acolhidos desativados nao contam na capacidade do cuidador.
- RN05.4: Acolhidos desativados nao aparecem nas listagens padrao (apenas com filtro).
- RN05.5: Desativacao e logica (soft delete), o registro permanece no banco.

---

## 2. Regras de Cuidadores

### RN06 - Capacidade
- RN06.1: Cada cuidador tem uma capacidade maxima configuravel (padrao: 5).
- RN06.2: A capacidade minima permitida e 1.
- RN06.3: A capacidade maxima permitida e 20.
- RN06.4: Apenas acolhidos com status **ativo** (NovoContato, PrimeiraVisita, EmAcompanhamento) contam na ocupacao.
- RN06.5: O sistema deve **bloquear** novas atribuicoes quando a ocupacao atingir 100%.
- RN06.6: O sistema deve **alertar** (sem bloquear) quando a ocupacao atingir 80% ou mais.

### RN07 - Disponibilidade
- RN07.1: Cuidadores com `disponibilidade = Nao` nao devem aparecer como opcao para novas atribuicoes.
- RN07.2: Marcar como indisponivel **nao** remove os acolhidos ja atribuidos.
- RN07.3: O coordenador pode forcar uma atribuicao para cuidador indisponivel (com aviso).

### RN08 - Reducao de Capacidade
- RN08.1: Se a capacidade for reduzida para abaixo da ocupacao atual, o sistema deve alertar mas **permitir** a mudanca.
- RN08.2: Nesse caso, novas atribuicoes ficam bloqueadas ate que a ocupacao fique abaixo da nova capacidade.
- RN08.3: Exemplo: cuidador com 4 acolhidos e capacidade reduzida para 3 → alertar, nao remover acolhidos.

---

## 3. Regras de Atribuicao

### RN09 - Atribuir Acolhido a Cuidador
- RN09.1: Ao atribuir um acolhido com status `NovoContato`, o status muda automaticamente para `PrimeiraVisita`.
- RN09.2: Ao atribuir, verificar se o cuidador tem vaga disponivel.
- RN09.3: Ao atribuir, verificar se o cuidador esta disponivel (alerta se nao).
- RN09.4: Ao atribuir, verificar se cuidador e acolhido pertencem a mesma igreja.
- RN09.5: A atribuicao deve ser registrada no historico de mudancas.

### RN10 - Reatribuir Acolhido
- RN10.1: Ao reatribuir, liberar a vaga do cuidador anterior.
- RN10.2: Ao reatribuir, ocupar uma vaga do novo cuidador.
- RN10.3: A reatribuicao deve registrar no historico (cuidador anterior → cuidador novo).
- RN10.4: O status do acolhido nao muda na reatribuicao.

---

## 4. Regras de Acompanhamento

### RN11 - Registro de Contato
- RN11.1: Somente o cuidador atribuido ou o coordenador podem registrar acompanhamento.
- RN11.2: A data do contato nao pode ser no futuro.
- RN11.3: A data do contato nao pode ser anterior a 30 dias da data atual.
- RN11.4: O tipo de contato e obrigatorio.
- RN11.5: Observacoes sao opcionais mas recomendadas.
- RN11.6: Ao registrar acompanhamento, o campo `ultimoContato` do acolhido e atualizado.

### RN12 - Atualizacoes durante Acompanhamento
- RN12.1: Ao registrar um acompanhamento, opcionalmente pode-se atualizar interesse, crescimento e/ou status.
- RN12.2: Cada atualizacao gera um registro separado no historico.
- RN12.3: As atualizacoes seguem as regras de transicao (RN02, RN03, RN04).

---

## 5. Regras de Multi-Tenancy (Igrejas)

### RN13 - Isolamento de Dados
- RN13.1: Um coordenador so pode ver/editar dados de sua igreja.
- RN13.2: Um cuidador so pode ver seus proprios acolhidos.
- RN13.3: O Admin pode ver dados de todas as igrejas.
- RN13.4: Nao e possivel atribuir um acolhido de uma igreja a um cuidador de outra.
- RN13.5: Relatorios e dashboards sao sempre filtrados pela igreja do usuario.

### RN14 - Coordenadores
- RN14.1: Cada igreja deve ter ao menos um coordenador.
- RN14.2: O sistema nao deve permitir desativar o ultimo coordenador de uma igreja.
- RN14.3: Uma igreja pode ter multiplos coordenadores.

---

## 6. Regras de Alertas

### RN15 - Alertas Automaticos
- RN15.1: Acolhido sem contato > 7 dias: alerta amarelo.
- RN15.2: Acolhido sem contato > 14 dias: alerta laranja.
- RN15.3: Acolhido sem contato > 30 dias: alerta vermelho.
- RN15.4: Cuidador com ocupacao >= 80%: alerta de sobrecarga.
- RN15.5: Cuidador com ocupacao = 100%: bloqueio de novas atribuicoes.
- RN15.6: Acolhido sem cuidador: aparece na lista de pendencias.

---

## 7. Regras de Auditoria

### RN16 - Historico
- RN16.1: Toda mudanca de estado (status, interesse, crescimento) deve ser registrada.
- RN16.2: Toda mudanca de cuidador (atribuicao, reatribuicao) deve ser registrada.
- RN16.3: O historico deve conter: data, usuario responsavel, valor anterior, valor novo.
- RN16.4: O historico e imutavel (nao pode ser editado ou excluido).
- RN16.5: O historico deve ser acessivel na timeline do acolhido.
