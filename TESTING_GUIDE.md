# 🧪 Guia de Teste - FleetManager

Este guia mostra como testar a aplicação completa (Backend + Frontend).

## 📋 Pré-requisitos

Certifique-se de ter instalado:
- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ [Node.js 18+](https://nodejs.org/)
- ✅ [Docker Desktop](https://www.docker.com/products/docker-desktop)

## 🚀 Início Rápido

### Opção 1: Script Automático (Windows)

Execute o script de inicialização:

```cmd
start-dev.cmd
```

Depois abra 2 terminais e execute:

**Terminal 1 - Backend:**
```bash
dotnet run --project src/FleetManager.Api
```

**Terminal 2 - Frontend:**
```bash
cd frontend
npm install
npm start
```

### Opção 2: Manual

#### Passo 1: Iniciar Infraestrutura

```bash
# Iniciar Docker Compose (SQL Server, Redis, Seq)
docker-compose up -d

# Verificar se os serviços estão rodando
docker-compose ps

# Aguardar 15 segundos para os serviços iniciarem
```

#### Passo 2: Configurar Banco de Dados

```bash
# Aplicar migrações
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

#### Passo 3: Iniciar Backend

```bash
# Executar a API
dotnet run --project src/FleetManager.Api
```

Aguarde a mensagem: `Now listening on: http://localhost:5000`

#### Passo 4: Iniciar Frontend (Novo Terminal)

```bash
# Navegar para o frontend
cd frontend

# Instalar dependências (apenas primeira vez)
npm install

# Iniciar servidor de desenvolvimento
npm start
```

Aguarde a mensagem: `Angular Live Development Server is listening on localhost:4200`

## 🌐 URLs de Acesso

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **Frontend** | http://localhost:4200 | Interface Angular |
| **API** | http://localhost:5000 | Backend REST API |
| **Swagger** | http://localhost:5000/swagger | Documentação interativa |
| **Seq** | http://localhost:5341 | Visualização de logs |

## 🧪 Cenários de Teste

### 1. Gestão de Veículos

#### Criar Veículo
1. Acesse http://localhost:4200
2. Clique em "Veículos" no menu lateral
3. Clique em "Novo Veículo"
4. Preencha:
   - **Placa**: ABC1234
   - **Modelo**: Toyota Corolla
   - **Ano**: 2023
   - **Quilometragem**: 0
5. Clique em "Criar"
6. ✅ Veículo deve aparecer na lista com status "Disponível"

#### Editar Veículo
1. Clique no ícone de lápis (editar)
2. Altere a quilometragem para 1000
3. Clique em "Atualizar"
4. ✅ Veículo deve ser atualizado na lista

#### Excluir Veículo
1. Clique no ícone de lixeira (excluir)
2. Confirme a exclusão
3. ✅ Veículo deve ser removido da lista

**Crie pelo menos 3 veículos para os próximos testes**

### 2. Gestão de Condutores

#### Criar Condutor
1. Clique em "Condutores" no menu lateral
2. Clique em "Novo Condutor"
3. Preencha:
   - **Nome**: João Silva
   - **CNH**: 12345678900
   - **Telefone**: +55 (11) 99999-9999
4. Clique em "Criar"
5. ✅ Condutor deve aparecer na lista com status "Ativo"

#### Desativar Condutor
1. Clique no ícone de proibido (desativar)
2. Confirme a desativação
3. ✅ Status deve mudar para "Inativo"

#### Reativar Condutor
1. Clique no ícone de check (ativar)
2. Confirme a ativação
3. ✅ Status deve voltar para "Ativo"

**Crie pelo menos 2 condutores ativos para os próximos testes**

### 3. Gestão de Viagens

#### Iniciar Viagem
1. Clique em "Viagens" no menu lateral
2. Clique em "Iniciar Viagem"
3. Selecione:
   - **Veículo**: Escolha um veículo disponível
   - **Condutor**: Escolha um condutor ativo
   - **Rota**: São Paulo - Rio de Janeiro
4. Clique em "Iniciar Viagem"
5. ✅ Viagem deve aparecer na aba "Ativas"
6. ✅ Veículo deve mudar status para "Em Uso"

#### Verificar Dashboard
1. Clique em "Dashboard" no menu lateral
2. ✅ Veja os cards atualizados:
   - Veículos Disponíveis (diminuiu)
   - Veículos em Uso (aumentou)
   - Viagens Ativas (aumentou)
3. ✅ Viagem deve aparecer na lista "Viagens Ativas"

#### Finalizar Viagem
1. Volte para "Viagens"
2. Vá na aba "Ativas"
3. Clique em "Finalizar" na viagem ativa
4. Digite a distância: 450 km
5. Clique em "Finalizar"
6. ✅ Viagem deve aparecer na aba "Todas" com status "Finalizada"
7. ✅ Veículo deve voltar ao status "Disponível"
8. ✅ Quilometragem do veículo deve aumentar em 450 km

### 4. Gestão de Manutenções

#### Registrar Manutenção
1. Clique em "Manutenções" no menu lateral
2. Clique em "Nova Manutenção"
3. Preencha:
   - **Veículo**: Escolha um veículo
   - **Data**: Hoje
   - **Descrição**: Troca de óleo e filtros
   - **Custo**: R$ 350,00
4. Clique em "Registrar"
5. ✅ Manutenção deve aparecer na lista
6. ✅ Veículo deve mudar status para "Em Manutenção"

#### Ver Histórico de Manutenção
1. Na lista de manutenções, clique no ícone de histórico
2. ✅ Modal deve abrir mostrando todas as manutenções do veículo
3. ✅ Total de custos deve ser exibido no rodapé

#### Verificar Manutenções Próximas
1. Vá na aba "Próximas"
2. ✅ Veículos com manutenção nos próximos 7 dias devem aparecer
3. Clique no ícone de histórico para ver detalhes

### 5. Busca e Filtros

#### Buscar Veículos
1. Vá em "Veículos"
2. Digite uma placa no campo de busca (ex: ABC)
3. ✅ Lista deve filtrar em tempo real

#### Buscar Condutores
1. Vá em "Condutores"
2. Digite um nome ou CNH no campo de busca
3. ✅ Lista deve filtrar em tempo real

### 6. Validações

#### Testar Validações de Formulário

**Veículo:**
- Tente criar sem preencher campos → ✅ Deve mostrar "Campo obrigatório"
- Digite placa inválida (ex: 123) → ✅ Deve mostrar erro de formato
- Digite ano inválido (ex: 1800) → ✅ Deve mostrar erro de ano

**Condutor:**
- Tente criar com CNH de 10 dígitos → ✅ Deve mostrar "CNH deve conter 11 dígitos"
- Tente criar com telefone inválido → ✅ Deve mostrar erro de formato

**Viagem:**
- Tente iniciar sem selecionar veículo → ✅ Deve mostrar "Campo obrigatório"
- Tente finalizar com distância 0 → ✅ Deve mostrar "Distância deve ser maior que zero"

**Manutenção:**
- Tente registrar com custo negativo → ✅ Deve mostrar erro
- Tente registrar sem descrição → ✅ Deve mostrar "Campo obrigatório"

### 7. Regras de Negócio

#### Veículo em Uso não pode iniciar nova viagem
1. Inicie uma viagem com um veículo
2. Tente iniciar outra viagem com o mesmo veículo
3. ✅ Deve mostrar erro: "Vehicle is not available for trip"

#### Condutor inativo não aparece na lista
1. Desative um condutor
2. Tente iniciar uma viagem
3. ✅ Condutor desativado não deve aparecer no dropdown

## 🔍 Verificar Logs

### Seq (Logs Estruturados)
1. Acesse http://localhost:5341
2. Veja todos os logs da aplicação em tempo real
3. Filtre por nível (Information, Warning, Error)
4. Busque por eventos específicos

### Console da API
- Veja os logs no terminal onde a API está rodando
- Cada requisição é logada com detalhes

### Console do Frontend
- Abra o DevTools do navegador (F12)
- Vá na aba "Console"
- Veja logs de requisições HTTP

## 🐛 Troubleshooting

### Backend não inicia

```bash
# Verificar se as portas estão livres
netstat -ano | findstr :5000
netstat -ano | findstr :1433
netstat -ano | findstr :6379

# Reiniciar Docker Compose
docker-compose down
docker-compose up -d

# Verificar logs do Docker
docker-compose logs -f
```

### Frontend não inicia

```bash
# Limpar cache do npm
cd frontend
npm cache clean --force
rm -rf node_modules package-lock.json
npm install

# Verificar se a porta 4200 está livre
netstat -ano | findstr :4200
```

### Erro de CORS

Se você ver erros de CORS no console do navegador:
1. Verifique se a API está rodando em http://localhost:5000
2. Verifique se o frontend está rodando em http://localhost:4200
3. Reinicie ambos os serviços

### Banco de dados não conecta

```bash
# Verificar se o SQL Server está rodando
docker-compose ps

# Recriar o banco
docker-compose down -v
docker-compose up -d
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

## 🧹 Limpar Ambiente

### Parar tudo

```bash
# Parar frontend (Ctrl+C no terminal)
# Parar backend (Ctrl+C no terminal)

# Parar Docker Compose
docker-compose down
```

### Limpar dados (reset completo)

```bash
# Parar e remover volumes
docker-compose down -v

# Recriar banco de dados
docker-compose up -d
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

## 📊 Dados de Teste

Para facilitar os testes, você pode usar estes dados:

### Veículos
| Placa | Modelo | Ano | Quilometragem |
|-------|--------|-----|---------------|
| ABC1234 | Toyota Corolla | 2023 | 0 |
| DEF5678 | Honda Civic | 2022 | 5000 |
| GHI9012 | Volkswagen Gol | 2021 | 15000 |

### Condutores
| Nome | CNH | Telefone |
|------|-----|----------|
| João Silva | 12345678900 | +5511999999999 |
| Maria Santos | 98765432100 | +5511988888888 |
| Pedro Oliveira | 11122233344 | +5511977777777 |

### Rotas Comuns
- São Paulo - Rio de Janeiro (450 km)
- São Paulo - Campinas (100 km)
- Rio de Janeiro - Belo Horizonte (440 km)
- São Paulo - Curitiba (400 km)

## ✅ Checklist de Testes

- [ ] Backend iniciou sem erros
- [ ] Frontend iniciou sem erros
- [ ] Swagger está acessível
- [ ] Dashboard carrega corretamente
- [ ] Criar veículo funciona
- [ ] Editar veículo funciona
- [ ] Excluir veículo funciona
- [ ] Buscar veículo funciona
- [ ] Criar condutor funciona
- [ ] Ativar/desativar condutor funciona
- [ ] Iniciar viagem funciona
- [ ] Finalizar viagem funciona
- [ ] Registrar manutenção funciona
- [ ] Ver histórico de manutenção funciona
- [ ] Validações de formulário funcionam
- [ ] Mensagens de erro aparecem corretamente
- [ ] Mensagens de sucesso aparecem corretamente
- [ ] Navegação entre páginas funciona
- [ ] Dados persistem após refresh

## 🎯 Próximos Passos

Após testar a aplicação:
1. Explore a documentação da API no Swagger
2. Veja os logs estruturados no Seq
3. Execute os testes automatizados: `dotnet test`
4. Experimente diferentes cenários de uso
5. Reporte bugs ou sugestões

---

**Dúvidas?** Consulte o [README.md](README.md) principal ou a [documentação da API](docs/API.md).
