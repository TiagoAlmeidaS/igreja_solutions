# Configuração do Caddy

Este documento descreve a configuração do Caddy como reverse proxy para o projeto Igreja Solutions.

## Visão Geral

O Caddy atua como reverse proxy e servidor web, gerenciando:
- HTTPS automático com Let's Encrypt
- Roteamento de requisições para os serviços
- Headers de segurança
- Compressão de conteúdo
- Dashboard administrativo

## Configuração

### Domínio de Produção

- **Subdomínio**: `igreja.tastechdeveloper.shop`
- **Email Let's Encrypt**: `tas.tech.developer@gmail.com`
- **Dashboard Admin**: `http://localhost:2019` (apenas local)

### Estrutura de Roteamento

```
igreja.tastechdeveloper.shop/
├── /                    → Frontend (hinos_web)
├── /api/*               → API Backend (hinos_api)
├── /swagger*            → Documentação Swagger
└── /health              → Health check da API
```

### Configurações Globais

As configurações globais estão definidas no bloco `{}` do Caddyfile:

- **Email**: `tas.tech.developer@gmail.com` - usado para notificações do Let's Encrypt
- **Admin Dashboard**: Porta `2019` - interface administrativa do Caddy
- **Logs**: Formato JSON, nível INFO, saída para stdout

## Serviços Expostos

### Frontend (hinos_web)
- **Rota**: `/`
- **Backend**: `hinos_web:80`
- **Descrição**: Aplicação React servida via Nginx

### API Backend (hinos_api)
- **Rota**: `/api/*`
- **Backend**: `hinos_api:8080`
- **Descrição**: API .NET 9 com Minimal API

### Swagger
- **Rota**: `/swagger*`
- **Backend**: `hinos_api:8080`
- **Descrição**: Documentação interativa da API

### Health Check
- **Rota**: `/health`
- **Backend**: `hinos_api:8080`
- **Descrição**: Endpoint de verificação de saúde

## Segurança

### Headers de Segurança Configurados

- **Strict-Transport-Security**: Força uso de HTTPS
- **X-XSS-Protection**: Proteção contra XSS
- **X-Content-Type-Options**: Previne MIME type sniffing
- **X-Frame-Options**: Previne clickjacking
- **Referrer-Policy**: Controla informações de referrer
- **Permissions-Policy**: Restringe recursos do navegador

### HTTPS Automático

O Caddy automaticamente:
- Obtém certificados SSL/TLS do Let's Encrypt
- Renova certificados automaticamente
- Redireciona HTTP para HTTPS
- Suporta apenas TLS 1.2 e 1.3

## Desenvolvimento Local

Para desenvolvimento local, o Caddy está configurado para responder em `localhost`:
- Acesse: `http://localhost`
- Logs em formato console (mais legível)
- Sem HTTPS (apenas HTTP)

## Produção

### Pré-requisitos

1. **DNS Configurado**: O subdomínio `igreja.tastechdeveloper.shop` deve apontar para o IP do servidor
2. **Portas Abertas**: 
   - `80` (HTTP)
   - `443` (HTTPS)
   - `2019` (Admin Dashboard - opcional, apenas para acesso local)

### Deploy

```bash
# Subir todos os serviços
docker-compose up -d

# Verificar logs do Caddy
docker-compose logs -f caddy

# Verificar status
docker-compose ps
```

### Verificação

Após o deploy, verifique:

1. **HTTPS**: Acesse `https://igreja.tastechdeveloper.shop`
2. **Frontend**: Deve carregar a aplicação React
3. **API**: Teste `https://igreja.tastechdeveloper.shop/api/health`
4. **Swagger**: Acesse `https://igreja.tastechdeveloper.shop/swagger`

## Dashboard Administrativo

O dashboard administrativo do Caddy está disponível em:
- **URL**: `http://localhost:2019`
- **Acesso**: Apenas localmente (não exposto publicamente)
- **Funcionalidades**: 
  - Visualização de configuração
  - Estatísticas de requisições
  - Gerenciamento de certificados

## Troubleshooting

### Certificado SSL não é gerado

1. Verifique se o DNS está apontando corretamente
2. Confirme que as portas 80 e 443 estão acessíveis
3. Verifique os logs: `docker-compose logs caddy`
4. Confirme o email configurado no Caddyfile

### Erro 502 Bad Gateway

1. Verifique se os serviços backend estão rodando: `docker-compose ps`
2. Confirme a conectividade na rede Docker: `docker network inspect hinos_network`
3. Verifique os health checks dos serviços

### Logs

```bash
# Logs do Caddy
docker-compose logs -f caddy

# Logs de todos os serviços
docker-compose logs -f

# Logs apenas de erros
docker-compose logs caddy | grep -i error
```

## Volumes Docker

O Caddy utiliza os seguintes volumes:

- **caddy_data**: Armazena certificados SSL/TLS e dados persistentes
- **caddy_config**: Armazena configurações do Caddy

## Referências

- [Documentação Oficial do Caddy](https://caddyserver.com/docs/)
- [Caddyfile Syntax](https://caddyserver.com/docs/caddyfile)
- [Let's Encrypt](https://letsencrypt.org/)

