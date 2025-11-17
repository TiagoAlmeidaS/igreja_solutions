# IgrejaConecta - Frontend

Frontend React com Vite para automação de avisos e transmissões via WhatsApp.

## Estrutura

```
frontend/
├── src/
│   ├── pages/           # Páginas da aplicação
│   ├── components/      # Componentes reutilizáveis
│   ├── services/        # Serviços de API
│   ├── App.jsx          # Componente principal
│   └── main.jsx         # Entry point
├── vite.config.js
└── package.json
```

## Instalação

```bash
npm install
```

## Executar

```bash
# Desenvolvimento
npm run dev

# Build para produção
npm run build

# Preview do build
npm run preview
```

## Variáveis de Ambiente

Criar arquivo `.env`:

```
VITE_API_URL=http://localhost:8000/api/v1
```

## Próximos Passos

- [ ] Implementar página de login
- [ ] Implementar dashboard
- [ ] Implementar gerenciamento de contatos
- [ ] Implementar criação de transmissões
- [ ] Implementar agendamento
- [ ] Implementar templates
- [ ] Implementar histórico

