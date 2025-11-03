# Frontend de Hinários - hinos_web

## Visão Geral

Aplicação web React desenvolvida com Vite para visualização e busca de hinários. Interface moderna e responsiva com suporte a tema claro/escuro.

## Tecnologias

- **React 18** - Biblioteca JavaScript para interfaces
- **TypeScript** - Tipagem estática
- **Vite** - Build tool e dev server
- **Tailwind CSS** - Framework CSS utilitário
- **Radix UI** - Componentes UI acessíveis
- **Lucide React** - Ícones
- **React Hook Form** - Gerenciamento de formulários

## Estrutura do Projeto

```
hinos_web/
├── src/
│   ├── components/      # Componentes React
│   │   ├── ui/         # Componentes UI base
│   │   ├── SearchPage.tsx
│   │   └── HymnDetailPage.tsx
│   ├── services/        # Serviços de API
│   │   └── api.ts      # Cliente HTTP para API
│   ├── types/          # Definições TypeScript
│   │   └── hymn.ts
│   ├── contexts/       # Contextos React
│   │   └── ThemeContext.tsx
│   └── App.tsx         # Componente principal
├── index.html
├── vite.config.ts
└── package.json
```

## Funcionalidades

### Página de Busca (SearchPage)
- Busca por número, título ou trecho da letra
- Filtros por categoria (Hinário, Cânticos, Suplementar, Novos)
- Listagem de resultados em cards
- Estados de loading e erro
- Auto-focus no campo de busca

### Página de Detalhes (HymnDetailPage)
- Visualização completa do hino
- Exibição de versos formatados
- Informações de tom (Key) e BPM
- Exportação de texto puro (copiar)
- Exportação formatada para Holyrics (download)
- Navegação de volta para busca

## Integração com API

O frontend consome a API REST através do serviço `src/services/api.ts`.

### Variáveis de Ambiente

Criar arquivo `.env` ou `.env.local`:
```
VITE_API_URL=http://localhost:5000
```

### Endpoints Utilizados

- `GET /api/hymns` - Listar hinos com filtros
- `GET /api/hymns/{id}` - Buscar hino por ID
- `GET /api/hymns/search` - Buscar por termo

## Executando Localmente

### Pré-requisitos
- Node.js 20+
- npm ou yarn

### Passos

1. Instalar dependências:
```bash
npm install
```

2. Criar arquivo `.env`:
```bash
echo "VITE_API_URL=http://localhost:5000" > .env
```

3. Executar em desenvolvimento:
```bash
npm run dev
```

4. Build para produção:
```bash
npm run build
```

A aplicação estará disponível em http://localhost:3000

## Docker

### Build da imagem:
```bash
docker build -t hinos_web .
```

### Executar container:
```bash
docker run -p 3000:80 hinos_web
```

### Variáveis de Ambiente no Docker

O Dockerfile aceita o build arg `VITE_API_URL`:
```bash
docker build --build-arg VITE_API_URL=http://api:8080 -t hinos_web .
```

## Build de Produção

O Vite gera um build otimizado na pasta `build/`:
- Minificação de código
- Tree shaking
- Code splitting
- Otimização de assets

O build é servido via nginx no Docker.

## Nginx Configuration

O arquivo `nginx.conf` configura:
- Servir arquivos estáticos
- Roteamento para React Router (SPA)
- Compressão Gzip
- Cache de assets estáticos
- Headers de segurança

## Tema

A aplicação suporta tema claro e escuro através do `ThemeContext`, utilizando `next-themes`.

## Tipos TypeScript

### Hymn
```typescript
interface Hymn {
  id: string;
  number: string;
  title: string;
  category: 'hinario' | 'canticos' | 'suplementar' | 'novos';
  hymnBook: string;
  verses: Verse[];
  key?: string;
  bpm?: number;
}
```

### Verse
```typescript
interface Verse {
  type: 'V1' | 'V2' | 'V3' | 'V4' | 'R' | 'Ponte' | 'C';
  lines: string[];
}
```

## Componentes UI

A aplicação utiliza componentes do Radix UI com estilização Tailwind:
- Button, Card, Input, Badge
- Dialog, Dropdown, Tabs
- E muitos outros componentes acessíveis

## Tratamento de Erros

- Loading states em todas as requisições
- Mensagens de erro amigáveis
- Fallbacks para estados vazios
- Retry automático (pode ser implementado)

## Performance

- Lazy loading de componentes
- Code splitting automático
- Cache de requisições (pode ser implementado)
- Otimização de imagens
- Compressão de assets

## Acessibilidade

- Componentes Radix UI seguem padrões ARIA
- Navegação por teclado
- Contraste adequado em ambos os temas
- Labels descritivos

## Melhorias Futuras

- Cache de requisições (React Query)
- Paginação de resultados
- Favoritos/Bookmarks
- Histórico de visualizações
- Compartilhamento de hinos
- PWA (Progressive Web App)
- Testes unitários e de integração
- Storybook para documentação de componentes
