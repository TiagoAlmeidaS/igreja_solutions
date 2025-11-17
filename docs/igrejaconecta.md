# **IgrejaConecta**

### *MicroSaaS para Automação de Avisos e Transmissões via WhatsApp*

> **Data de criação:** 15 de novembro de 2025  
> **Autor:** Igreja Solutions  
> **Linguagens:** Python (FastAPI) + React (Vite)  
> **Deploy:** Railway / Vercel

---

## 1. Visão Geral do Produto

Um **MicroSaaS para igrejas** que permite:

- Enviar avisos automáticos
- Criar links de transmissão ao vivo com botão
- Agendar lembretes
- Gerenciar contatos e grupos
- Usar templates de mensagem

Tudo via **WhatsApp oficial (Meta Cloud API)**.

---

## 2. Requisitos Funcionais (RF)

| ID | Descrição |
|----|---------|
| RF01 | Cadastro e login de igrejas |
| RF02 | Configurar número do WhatsApp Business |
| RF03 | Importar contatos via CSV |
| RF04 | Criar transmissão com link e botão |
| RF05 | Agendar envio de mensagem |
| RF06 | Enviar mensagem em massa |
| RF07 | Visualizar histórico de envios |
| RF08 | Criar e usar templates |
| RF09 | Gerenciar tags de contatos |
| RF10 | Dashboard com métricas |

---

## 3. Requisitos Não Funcionais (RNF)

| ID | Descrição |
|----|---------|
| RNF01 | 99% de uptime |
| RNF02 | Tempo de resposta < 2s |
| RNF03 | Suporte a 10.000 contatos por igreja |
| RNF04 | Criptografia de tokens (AES) |
| RNF05 | Conformidade com LGPD |
| RNF06 | Interface responsiva (mobile-first) |

---

## 4. Arquitetura do Sistema

```
[Frontend - React]

       ↓ HTTPS (JWT)

[Backend - FastAPI]

       ↓

[PostgreSQL] ←→ [Redis (Celery)]

       ↓

[Meta WhatsApp Cloud API]
```

---

## 5. Estrutura de Pastas

```
igrejaconecta/
│
├── backend/
│   ├── app/
│   │   ├── main.py
│   │   ├── api/
│   │   │   ├── v1/
│   │   │   │   ├── auth.py
│   │   │   │   ├── church.py
│   │   │   │   ├── contact.py
│   │   │   │   ├── broadcast.py
│   │   │   │   └── template.py
│   │   ├── core/
│   │   │   ├── security.py
│   │   │   └── config.py
│   │   ├── models/
│   │   │   ├── church.py
│   │   │   ├── contact.py
│   │   │   └── broadcast.py
│   │   ├── schemas/
│   │   ├── services/
│   │   │   ├── whatsapp.py
│   │   │   └── scheduler.py
│   │   └── tasks.py (Celery)
│   ├── requirements.txt
│   └── Dockerfile
│
├── frontend/
│   ├── src/
│   │   ├── pages/
│   │   ├── components/
│   │   ├── services/api.js
│   │   └── App.jsx
│   ├── vite.config.js
│   └── package.json
│
└── docker-compose.yml
```

---

## 6. Banco de Dados (PostgreSQL)

```sql
-- Tabela: igrejas
CREATE TABLE churches (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    admin_name VARCHAR(100),
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    phone VARCHAR(20),
    whatsapp_phone_id TEXT,
    whatsapp_access_token TEXT ENCRYPTED,
    created_at TIMESTAMP DEFAULT NOW(),
    is_active BOOLEAN DEFAULT TRUE
);

-- Tabela: contatos
CREATE TABLE contacts (
    id SERIAL PRIMARY KEY,
    church_id INT REFERENCES churches(id),
    name VARCHAR(100),
    phone VARCHAR(20) NOT NULL,
    tags TEXT[], -- ex: {'membro', 'líder'}
    created_at TIMESTAMP DEFAULT NOW()
);

-- Tabela: transmissões
CREATE TABLE broadcasts (
    id SERIAL PRIMARY KEY,
    church_id INT REFERENCES churches(id),
    title VARCHAR(100),
    message TEXT,
    link_url TEXT,
    button_text VARCHAR(50),
    contact_tags TEXT[],
    scheduled_at TIMESTAMP,
    sent_at TIMESTAMP,
    status VARCHAR(20) DEFAULT 'pending', -- pending, sent, failed
    total_sent INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Tabela: templates
CREATE TABLE templates (
    id SERIAL PRIMARY KEY,
    church_id INT REFERENCES churches(id),
    name VARCHAR(50),
    message TEXT,
    link_url TEXT,
    button_text VARCHAR(50),
    created_at TIMESTAMP DEFAULT NOW()
);
```

---

## 7. Variáveis de Ambiente (`.env`)

```env
# Backend
DATABASE_URL=postgresql://user:pass@localhost:5432/igrejaconecta
REDIS_URL=redis://localhost:6379/0
SECRET_KEY=supersecretkey123
JWT_ALGORITHM=HS256
JWT_EXPIRE_MINUTES=1440

# Meta WhatsApp
WHATSAPP_API_VERSION=v20.0

# Frontend
VITE_API_URL=http://localhost:8000/api/v1
```

---

## 8. Endpoints da API (FastAPI)

| Método | Endpoint | Descrição |
|-------|---------|----------|
| POST | `/auth/register` | Cadastro de igreja |
| POST | `/auth/login` | Login → JWT |
| GET | `/church/me` | Dados da igreja |
| POST | `/contacts/upload` | CSV → contatos |
| POST | `/broadcasts` | Criar transmissão |
| GET | `/broadcasts` | Listar |
| POST | `/broadcasts/{id}/send` | Enviar agora |
| POST | `/templates` | Criar template |

---

## 9. Exemplo: Envio de Mensagem com Botão (Meta API)

```python
# services/whatsapp.py
import requests
from fastapi import HTTPException

def send_interactive_message(
    to: str,
    body: str,
    button_text: str,
    url: str,
    phone_id: str,
    token: str
):
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }
    payload = {
        "messaging_product": "whatsapp",
        "to": to,
        "type": "interactive",
        "interactive": {
            "type": "button",
            "body": {"text": body},
            "action": {
                "buttons": [
                    {
                        "type": "url",
                        "url": url,
                        "title": button_text[:20]
                    }
                ]
            }
        }
    }
    url = f"https://graph.facebook.com/v20.0/{phone_id}/messages"
    response = requests.post(url, json=payload, headers=headers)
    if response.status_code != 200:
        raise HTTPException(500, f"WhatsApp error: {response.text}")
    return response.json()
```

---

## 10. Agendamento com Celery + Redis

```python
# tasks.py
from celery import Celery
from services.whatsapp import send_interactive_message

celery = Celery(__name__, broker="redis://localhost:6379/0")

@celery.task
def send_scheduled_broadcast(broadcast_id: int):
    # Buscar do banco
    broadcast = get_broadcast(broadcast_id)
    contacts = get_contacts_by_tags(broadcast.church_id, broadcast.contact_tags)
    
    for contact in contacts:
        send_interactive_message(
            to=contact.phone,
            body=broadcast.message,
            button_text=broadcast.button_text,
            url=broadcast.link_url,
            phone_id=broadcast.church.whatsapp_phone_id,
            token=broadcast.church.whatsapp_access_token
        )
    
    update_broadcast_status(broadcast_id, "sent", len(contacts))
```

---

## 11. Exemplo: Criar Transmissão (Frontend → Backend)

```json
POST /api/v1/broadcasts
{
  "title": "Culto de Domingo",
  "message": "*Culto ao Vivo - 09h*\nParticipe conosco!",
  "link_url": "https://youtube.com/live/abc123",
  "button_text": "Assistir Agora",
  "contact_tags": ["membro", "visitante"],
  "scheduled_at": "2025-11-16T08:30:00"
}
```

---

## 12. Segurança e LGPD

- Tokens criptografados no banco (Fernet)
- JWT com expiração
- Consentimento obrigatório no cadastro do contato
- Botão "Descadastrar" em todas as mensagens
- Logs de envio com data/hora

---

## 13. Fluxo de Cadastro da Igreja

1. Acessa `igrejaconecta.com`
2. Preenche: nome, email, senha, telefone
3. Recebe email com código
4. Acessa Meta → adiciona número → copia `Phone ID` e `Token`
5. Cola no sistema → valida com mensagem de teste
6. Pronto para usar!

---

## 14. Template de Mensagem Padrão

```text
*Culto ao Vivo - Domingo 09h*

Venha adorar conosco! Transmissão começa em breve.

[Botão: Assistir Agora → https://youtube.com/live/abc123]

Deus te abençoe!  
Pr. João Silva
```

---

## 15. Status do Projeto

**Fase Atual:** Estrutura base criada  
**Próximos Passos:**
1. Implementação do backend com FastAPI
2. Implementação do frontend com React
3. Configuração do Docker Compose
4. Integração com Meta WhatsApp Cloud API
5. Testes unitários e de integração

---

## 16. Referências

- [Meta WhatsApp Cloud API Documentation](https://developers.facebook.com/docs/whatsapp/cloud-api)
- [FastAPI Documentation](https://fastapi.tiangolo.com/)
- [React Documentation](https://react.dev/)
- [Celery Documentation](https://docs.celeryproject.org/)

