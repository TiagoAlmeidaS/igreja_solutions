# Web de Hinários

This is a code bundle for Web de Hinários. The original project is available at https://www.figma.com/design/y7fBBJSregKfB3lFzwNGtV/Web-de-Hin%C3%A1rios.

## Configuração

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto com a seguinte variável:

```env
VITE_API_URL=http://localhost:5000
```

- **Em desenvolvimento local**: Use `http://localhost:5000` (porta padrão da API)
- **Em produção via Docker**: Use `http://localhost:5000` ou a URL do servidor onde a API está hospedada

### Instalação e Execução

1. Instale as dependências:
```bash
npm i
```

2. Execute o servidor de desenvolvimento:
```bash
npm run dev
```

A aplicação estará disponível em `http://localhost:5173` (ou outra porta indicada pelo Vite).

### Build para Produção

```bash
npm run build
```

Os arquivos serão gerados na pasta `build/`.

## Integração com API

A aplicação está totalmente integrada com a API `hinos_api`. Certifique-se de que:

1. A API está rodando e acessível na URL configurada em `VITE_API_URL`
2. A API possui CORS configurado para permitir requisições do frontend
3. Os endpoints estão disponíveis conforme documentado na API

### Funcionalidades Integradas

- ✅ Listagem de hinos com filtros por categoria
- ✅ Busca de hinos por termo
- ✅ Visualização de detalhes do hino
- ✅ Criação de novos hinos via Chat IA
- ✅ Tratamento de erros e estados de loading
