# Configuração Dinâmica de Portas e URLs

Este documento explica como configurar dinamicamente as portas e URLs das aplicações sem modificar arquivos de código.

## Como Funciona

O sistema usa uma hierarquia de configuração onde as variáveis de ambiente têm prioridade sobre os arquivos de configuração:

1. **Variáveis de Ambiente** (maior prioridade)
2. **appsettings.Development.json**
3. **launchSettings.json** (valores padrão)

## API (hinos_api)

### Configuração via Variáveis de Ambiente

Defina a variável de ambiente `ASPNETCORE_URLS` antes de executar a aplicação:

**Windows (PowerShell):**
```powershell
$env:ASPNETCORE_URLS = "http://localhost:5000"
dotnet run
```

**Windows (CMD):**
```cmd
set ASPNETCORE_URLS=http://localhost:5000
dotnet run
```

**Linux/Mac:**
```bash
export ASPNETCORE_URLS="http://localhost:5000"
dotnet run
```

### Configuração via appsettings.Development.json

Edite o arquivo `hinos_api/appsettings.Development.json`:

```json
{
  "Urls": {
    "Http": "http://localhost:5000",
    "Https": "https://localhost:5001"
  }
}
```

**Nota:** O ASP.NET Core lê `ASPNETCORE_URLS` automaticamente. Se você definir essa variável, ela terá prioridade sobre o `appsettings.Development.json`.

### Exemplos de Uso

#### Mudar para porta 8080:
```powershell
$env:ASPNETCORE_URLS = "http://localhost:8080"
dotnet run
```

#### Usar múltiplas portas (HTTP + HTTPS):
```powershell
$env:ASPNETCORE_URLS = "https://localhost:5001;http://localhost:5000"
dotnet run
```

#### Escutar em todas as interfaces:
```powershell
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
dotnet run
```

## Frontend (hinos_web)

### Configuração via Variável de Ambiente

Crie um arquivo `.env` na raiz do projeto `hinos_web`:

```env
VITE_API_URL=http://localhost:5000
```

**Nota:** O Vite só lê variáveis que começam com `VITE_`.

### Configuração Temporária

Para testar com uma URL diferente sem criar arquivo:

**Windows (PowerShell):**
```powershell
$env:VITE_API_URL = "http://localhost:8080"
npm run dev
```

**Linux/Mac:**
```bash
VITE_API_URL=http://localhost:8080 npm run dev
```

## VS Code Launch Configuration

O arquivo `.vscode/launch.json` já está configurado para usar variáveis de ambiente:

- **HTTP**: Usa `${env:API_HTTP_URL}` ou padrão `http://localhost:5000`
- **HTTPS**: Usa `${env:API_URLS}` ou padrão `https://localhost:5001;http://localhost:5000`

Para usar uma porta diferente, defina a variável antes de iniciar o debug:

```powershell
$env:API_HTTP_URL = "http://localhost:8080"
```

Depois inicie o debug normalmente no VS Code.

## Docker Compose

No `docker-compose.yml`, as portas são configuradas nos serviços. Para mudar:

```yaml
hinos_api:
  ports:
    - "8080:8080"  # Altere aqui
```

## Resolução de Conflitos de Porta

Se uma porta estiver em uso, você pode:

1. **Encontrar o processo que está usando a porta:**
   ```powershell
   # Windows
   netstat -ano | findstr :5000
   
   # Linux/Mac
   lsof -i :5000
   ```

2. **Usar uma porta diferente:**
   ```powershell
   $env:ASPNETCORE_URLS = "http://localhost:5001"
   ```

3. **Parar o processo que está usando a porta**

## Ordem de Precedência (Resumo)

### API:
1. `ASPNETCORE_URLS` (variável de ambiente)
2. `appsettings.Development.json` → `Urls:Http` / `Urls:Https`
3. `launchSettings.json` → `applicationUrl`
4. Padrão: `http://localhost:5000`

### Frontend:
1. `VITE_API_URL` (variável de ambiente ou `.env`)
2. Código: `http://localhost:5000` (padrão em `api.ts`)

