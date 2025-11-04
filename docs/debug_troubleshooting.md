# Troubleshooting do Debug no VS Code

## Problemas Identificados e Soluções

### 1. **Build Task usando projeto ao invés de solution**
**Problema:** O `tasks.json` estava compilando apenas o `.csproj`, não a solution completa.

**Solução:** Atualizado para usar `hinos_api.sln`:
```json
"args": [
  "build",
  "${workspaceFolder}/hinos_api/hinos_api.sln",
  ...
]
```

### 2. **Falta de variável DOTNET_ENVIRONMENT**
**Problema:** Algumas configurações do .NET podem não ser aplicadas sem esta variável.

**Solução:** Adicionado `DOTNET_ENVIRONMENT: "Development"` nas configurações de debug.

### 3. **Configurações de logging muito verbosas**
**Problema:** Logs excessivos podem atrasar ou esconder mensagens importantes.

**Solução:** Configurado `engineLogging: false` e `moduleLoad: false`.

### 4. **Falta de justMyCode**
**Problema:** Debug pode não funcionar corretamente em todos os casos.

**Solução:** Adicionado `justMyCode: false` para melhor compatibilidade.

## Como Verificar se Está Funcionando

### 1. Testar Build Manual
```bash
cd hinos_api
dotnet build hinos_api.sln
```

### 2. Testar Execução Manual
```bash
cd hinos_api
dotnet run --urls "http://localhost:5000"
```

Se funcionar manualmente, o problema está na configuração do VS Code.

### 3. Verificar Debug Console
- Pressione `F5` para iniciar o debug
- Verifique se a aba **DEBUG CONSOLE** aparece
- Verifique se há mensagens de erro ou warnings

### 4. Verificar Terminal Integrado
- O console foi configurado como `integratedTerminal`
- Verifique se um novo terminal aparece quando inicia o debug
- Procure por mensagens como "Now listening on: http://localhost:5000"

## Possíveis Problemas Adicionais

### Porta já em uso
```powershell
# Verificar se a porta está em uso
netstat -ano | findstr :5000

# Se estiver, matar o processo ou usar outra porta
# No launch.json, altere ASPNETCORE_URLS para outra porta
```

### Problemas de Permissão
- Certifique-se de que o VS Code tem permissões para executar processos
- Tente executar como administrador (se necessário)

### Extensões Necessárias
Certifique-se de ter instalado:
- **C# Dev Kit** ou **C#** (Microsoft)
- **.NET Extension Pack** (opcional, mas recomendado)

### Verificar Logs do VS Code
1. Abra o Output (`Ctrl+Shift+U`)
2. Selecione "Debug Console" no dropdown
3. Veja se há mensagens de erro

## Configuração Recomendada

Após as correções, sua configuração deve:
- ✅ Compilar a solution completa
- ✅ Mostrar output no terminal integrado
- ✅ Abrir o Swagger automaticamente quando pronto
- ✅ Permitir debug completo com breakpoints

## Se Ainda Não Funcionar

1. **Limpar e Reconstruir:**
   ```bash
   cd hinos_api
   dotnet clean
   dotnet build hinos_api.sln
   ```

2. **Verificar se o .NET SDK está instalado:**
   ```bash
   dotnet --version
   ```

3. **Reiniciar o VS Code**

4. **Verificar se há erros de compilação:**
   - Abra a aba **PROBLEMS** (`Ctrl+Shift+M`)
   - Corrija todos os erros antes de tentar debug

5. **Testar com outro método:**
   - Use o terminal integrado diretamente:
   ```bash
   cd hinos_api
   dotnet run
   ```

