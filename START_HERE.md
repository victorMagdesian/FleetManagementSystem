# 🚀 Como Iniciar a Aplicação - Guia Passo a Passo

## ⚠️ IMPORTANTE: Execute os comandos na RAIZ do projeto!

Certifique-se de estar na pasta `FleetManagementSystem` (raiz), NÃO na pasta `frontend`.

```bash
# Verificar se está na pasta correta
pwd
# Deve mostrar: D:\dev\portifolio\FleetManagementSystem

# Se estiver em frontend, volte para a raiz:
cd ..
```

---

## 📋 Passo 1: Iniciar Backend

### 1.1 Iniciar Infraestrutura (Docker)

```bash
docker-compose up -d
```

Aguarde até ver:
```
✔ Container fleetmanager-sqlserver    Healthy
✔ Container fleetmanager-redis        Healthy
✔ Container fleetmanager-seq          Started
✔ Container fleetmanager-api          Started
```

### 1.2 Aplicar Migrações do Banco de Dados

**⚠️ Execute da RAIZ do projeto:**

```bash
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

Deve ver:
```
Build started...
Build succeeded.
Done.
```

### 1.3 Iniciar a API

```bash
dotnet run --project src/FleetManager.Api
```

Aguarde até ver:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

✅ **Backend está rodando!**

---

## 📋 Passo 2: Iniciar Frontend

### Abra um NOVO terminal (deixe o backend rodando)

### 2.1 Navegar para a pasta frontend

```bash
cd frontend
```

### 2.2 Instalar dependências (apenas primeira vez)

```bash
npm install
```

### 2.3 Iniciar servidor de desenvolvimento

```bash
npm start
```

Aguarde até ver:
```
✔ Browser application bundle generation complete.
** Angular Live Development Server is listening on localhost:4200 **
```

✅ **Frontend está rodando!**

---

## 🌐 Acessar a Aplicação

Abra seu navegador em:

- **Frontend**: http://localhost:4200
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Logs (Seq)**: http://localhost:5341

---

## 🧪 Testar a Aplicação

1. Acesse http://localhost:4200
2. Veja o Dashboard
3. Crie alguns veículos em "Veículos"
4. Crie alguns condutores em "Condutores"
5. Inicie uma viagem em "Viagens"
6. Registre uma manutenção em "Manutenções"

---

## 🛑 Parar a Aplicação

### Parar Frontend
No terminal do frontend, pressione: `Ctrl + C`

### Parar Backend
No terminal do backend, pressione: `Ctrl + C`

### Parar Docker
```bash
docker-compose down
```

---

## 🔧 Troubleshooting

### Erro: "Arquivo de projeto não existe"
**Causa**: Você está na pasta errada (provavelmente em `frontend/`)

**Solução**:
```bash
cd ..
# Agora execute os comandos novamente
```

### Erro: "Port 5000 is already in use"
**Solução**:
```bash
# Parar o processo que está usando a porta
netstat -ano | findstr :5000
# Anote o PID e mate o processo:
taskkill /PID <numero_do_pid> /F
```

### Erro: "Port 4200 is already in use"
**Solução**:
```bash
# Parar o processo que está usando a porta
netstat -ano | findstr :4200
# Anote o PID e mate o processo:
taskkill /PID <numero_do_pid> /F
```

### Erro: Docker não inicia
**Solução**:
```bash
# Verificar se Docker Desktop está rodando
# Reiniciar Docker Compose
docker-compose down
docker-compose up -d
```

### Erro de compilação no Angular
**Solução**:
```bash
cd frontend
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
npm start
```

---

## 📝 Resumo dos Comandos

**Terminal 1 (Backend) - Execute da RAIZ:**
```bash
docker-compose up -d
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
dotnet run --project src/FleetManager.Api
```

**Terminal 2 (Frontend) - Execute da RAIZ:**
```bash
cd frontend
npm install
npm start
```

---

## ✅ Checklist

- [ ] Estou na pasta RAIZ do projeto (não em `frontend/`)
- [ ] Docker Desktop está rodando
- [ ] Executei `docker-compose up -d`
- [ ] Executei as migrações do banco
- [ ] Backend está rodando (http://localhost:5000)
- [ ] Frontend está rodando (http://localhost:4200)
- [ ] Consigo acessar o Dashboard

---

**Dúvidas?** Consulte o [TESTING_GUIDE.md](TESTING_GUIDE.md) para mais detalhes.
