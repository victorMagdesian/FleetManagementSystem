# 🔧 Como Corrigir o Erro de CORS

## ✅ Mudanças Aplicadas

Já corrigi o código do backend:
1. ✅ Adicionado `http://localhost:4200` na política de CORS
2. ✅ Movido `UseCors()` para ANTES de `UseHttpsRedirection()` e `UseAuthorization()`

## 🚨 IMPORTANTE: Você PRECISA Reiniciar o Backend!

As mudanças no código só serão aplicadas quando você reiniciar o backend.

### Passo a Passo:

#### 1. Parar o Backend
No terminal onde o backend está rodando, pressione:
```
Ctrl + C
```

#### 2. Reiniciar o Backend
Execute novamente:
```bash
dotnet run --project src/FleetManager.Api
```

#### 3. Aguardar Inicialização
Espere até ver esta mensagem:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

#### 4. Recarregar o Frontend
No navegador, pressione `F5` ou `Ctrl + F5` (hard refresh)

---

## 🔍 Verificar se Funcionou

### No Console do Navegador (F12):
- ❌ **ANTES**: `Access to XMLHttpRequest at 'http://localhost:5000/api/vehicles' from origin 'http://localhost:4200' has been blocked by CORS policy`
- ✅ **DEPOIS**: Requisições devem funcionar sem erros de CORS

### No Terminal do Backend:
Você deve ver logs das requisições:
```
[12:34:56 INF] HTTP GET /api/vehicles responded 200 in 123.4567 ms
```

---

## 🐛 Se Ainda Não Funcionar

### Opção 1: Verificar se o Backend Reiniciou
```bash
# Verificar se está rodando na porta 5000
# No PowerShell:
Get-NetTCPConnection -LocalPort 5000

# Deve mostrar algo como:
# LocalAddress  LocalPort  RemoteAddress  RemotePort  State
# 0.0.0.0       5000       0.0.0.0        0            Listen
```

### Opção 2: Limpar Cache do Navegador
1. Abra DevTools (F12)
2. Clique com botão direito no ícone de refresh
3. Selecione "Limpar cache e recarregar forçadamente"

### Opção 3: Verificar a URL da API
No arquivo `frontend/src/app/core/services/api.service.ts`, verifique se a URL está correta:
```typescript
private readonly baseUrl = 'http://localhost:5000';
```

### Opção 4: Usar CORS Temporariamente Permissivo (apenas para teste)
Se ainda não funcionar, podemos temporariamente usar a política "AllowAll" em desenvolvimento.

Edite `src/FleetManager.Api/Program.cs`:
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");  // Mudou de "Development" para "AllowAll"
    
    app.UseSwagger();
    // ...
}
```

Depois reinicie o backend novamente.

---

## 📊 Configuração Atual de CORS

### Política "Development" (usada em desenvolvimento):
```csharp
policy.WithOrigins(
    "http://localhost:5000",   // API
    "https://localhost:5001",  // API HTTPS
    "http://localhost:3000",   // Outros frontends
    "http://localhost:4200"    // Angular ✅ ADICIONADO
)
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials();
```

### Política "AllowAll" (backup):
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

---

## ✅ Checklist de Resolução

- [ ] Parei o backend (Ctrl+C)
- [ ] Reiniciei o backend (`dotnet run --project src/FleetManager.Api`)
- [ ] Vi a mensagem "Now listening on: http://localhost:5000"
- [ ] Recarreguei o frontend (F5)
- [ ] Verifiquei o console do navegador (F12)
- [ ] Não há mais erros de CORS
- [ ] As requisições estão funcionando

---

## 🎯 Teste Rápido

Abra o console do navegador (F12) e execute:
```javascript
fetch('http://localhost:5000/api/vehicles')
  .then(r => r.json())
  .then(data => console.log('Sucesso!', data))
  .catch(err => console.error('Erro:', err));
```

Se funcionar, o CORS está OK! ✅

---

**Lembre-se**: Sempre que alterar o código do backend, você precisa reiniciar o servidor!
