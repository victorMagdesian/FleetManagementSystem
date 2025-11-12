# 🔄 Como Reiniciar o Backend

## ⚠️ IMPORTANTE: Verifique se o Backend Está Rodando!

Antes de reiniciar, verifique se o backend está realmente rodando:

```powershell
# PowerShell
Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
```

- **Se retornar algo:** Backend está rodando ✅
- **Se não retornar nada:** Backend NÃO está rodando ❌ (veja seção "Iniciar Backend")

---

## 🚀 Iniciar Backend (Primeira Vez ou Quando Não Está Rodando)

### Opção 1: Script Automático (Recomendado)
```bash
start-all.cmd
```

Este script inicia TUDO automaticamente:
- Docker (SQL Server, Redis, Seq)
- Aplica migrações
- Inicia Backend em nova janela
- Inicia Frontend em nova janela

### Opção 2: Manual
```bash
# 1. Iniciar infraestrutura
docker-compose up -d

# 2. Aguardar 15 segundos
timeout /t 15

# 3. Aplicar migrações
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api

# 4. Iniciar backend
dotnet run --project src/FleetManager.Api
```

---

## 🔄 Reiniciar Backend (Quando Já Está Rodando)

### Quando Reiniciar?

Você precisa reiniciar o backend quando:
- ✅ Fizer alterações no código C# (controllers, services, etc)
- ✅ Modificar configurações (appsettings.json, Program.cs)
- ✅ Adicionar ou alterar dependências (packages)
- ✅ Aplicar novas migrações do banco de dados
- ❌ **NÃO precisa** reiniciar para mudanças no frontend (Angular)

### Método Rápido (Recomendado)
```bash
restart-backend.cmd
```

### Método Manual

**Passo 1:** Parar o Backend
No terminal onde o backend está rodando, pressione:
```
Ctrl + C
```

**Passo 2:** Reiniciar
```bash
dotnet run --project src/FleetManager.Api
```

**Passo 3:** Aguardar Inicialização
Espere até ver:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

---

## 🐛 Solução de Problemas

### ❌ Erro de CORS no Frontend

**Sintoma:**
```
Access to XMLHttpRequest at 'http://localhost:5000/api/vehicles' 
from origin 'http://localhost:4200' has been blocked by CORS policy
```

**Causa:** Backend NÃO está rodando!

**Solução:**
```bash
# Verificar se backend está rodando
Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue

# Se não estiver, iniciar:
dotnet run --project src/FleetManager.Api
```

### ❌ Erro: "Port 5000 is already in use"

**Causa:** Outra instância do backend ainda está rodando

**Solução:**
```powershell
# PowerShell - Matar processos dotnet
Get-Process -Name dotnet | Stop-Process -Force

# Aguardar 5 segundos
timeout /t 5

# Reiniciar
dotnet run --project src/FleetManager.Api
```

### ❌ Erro: "Database connection failed"

**Causa:** SQL Server não está rodando

**Solução:**
```bash
# Iniciar Docker Compose
docker-compose up -d

# Aguardar 15 segundos
timeout /t 15

# Aplicar migrações
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api

# Reiniciar backend
dotnet run --project src/FleetManager.Api
```

### ❌ Erro: "Pending migrations"

**Solução:**
```bash
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

---

## 📊 Verificar Status

### Backend está rodando?
```powershell
# PowerShell
Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
```

### Testar API
```powershell
# PowerShell
Invoke-WebRequest -Uri http://localhost:5000/api/vehicles -Method GET
```

Ou abra no navegador:
- http://localhost:5000/swagger

---

## 🎯 Checklist Completo

### Para Iniciar (Primeira Vez):
- [ ] Docker está instalado e rodando
- [ ] Executei `docker-compose up -d`
- [ ] Aguardei 15 segundos
- [ ] Apliquei migrações
- [ ] Iniciei o backend
- [ ] Vi "Now listening on: http://localhost:5000"
- [ ] Testei no Swagger ou frontend

### Para Reiniciar:
- [ ] Parei o backend (Ctrl+C)
- [ ] Apliquei migrações (se necessário)
- [ ] Reiniciei o backend
- [ ] Vi "Now listening on: http://localhost:5000"
- [ ] Testei uma requisição
- [ ] Não há erros no console

---

## 💡 Dicas

### Hot Reload (Experimental)
Para não precisar reiniciar sempre:
```bash
dotnet watch run --project src/FleetManager.Api
```

### Múltiplos Terminais
Mantenha 2 terminais abertos:
- **Terminal 1:** Backend (`dotnet run --project src/FleetManager.Api`)
- **Terminal 2:** Frontend (`cd frontend && npm start`)

### Logs em Tempo Real
Acesse o Seq para ver logs detalhados:
```
http://localhost:5341
```

---

## 🔗 Links Úteis

- **API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger
- **Frontend:** http://localhost:4200
- **Seq (Logs):** http://localhost:5341

---

## 📝 Resumo Rápido

**Backend não está rodando?**
```bash
dotnet run --project src/FleetManager.Api
```

**Backend está rodando e precisa reiniciar?**
```
Ctrl+C (parar) → dotnet run --project src/FleetManager.Api (iniciar)
```

**Erro de CORS?**
```
Backend provavelmente não está rodando! Inicie-o primeiro.
```
