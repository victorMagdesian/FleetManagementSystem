@echo off
echo ========================================
echo   FleetManager - Iniciar Desenvolvimento
echo ========================================
echo.

echo [1/4] Iniciando servicos de infraestrutura (Docker)...
docker-compose up -d
if %errorlevel% neq 0 (
    echo ERRO: Falha ao iniciar Docker Compose
    pause
    exit /b 1
)

echo.
echo [2/4] Aguardando servicos iniciarem...
timeout /t 15 /nobreak

echo.
echo [3/4] Aplicando migracoes do banco de dados...
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
if %errorlevel% neq 0 (
    echo ERRO: Falha ao aplicar migracoes
    pause
    exit /b 1
)

echo.
echo [4/4] Pronto! Agora execute os comandos abaixo em terminais separados:
echo.
echo Terminal 1 - Backend:
echo   dotnet run --project src/FleetManager.Api
echo.
echo Terminal 2 - Frontend:
echo   cd frontend
echo   npm install
echo   npm start
echo.
echo URLs de acesso:
echo   - Frontend: http://localhost:4200
echo   - API: http://localhost:5000
echo   - Swagger: http://localhost:5000/swagger
echo   - Seq (Logs): http://localhost:5341
echo.
pause
