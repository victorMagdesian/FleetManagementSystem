@echo off
echo ========================================
echo   FleetManager - Iniciar TUDO
echo ========================================
echo.

echo [1/5] Iniciando servicos de infraestrutura (Docker)...
docker-compose up -d
if %errorlevel% neq 0 (
    echo ERRO: Falha ao iniciar Docker Compose
    pause
    exit /b 1
)

echo.
echo [2/5] Aguardando servicos iniciarem...
timeout /t 15 /nobreak

echo.
echo [3/5] Aplicando migracoes do banco de dados...
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
if %errorlevel% neq 0 (
    echo ERRO: Falha ao aplicar migracoes
    pause
    exit /b 1
)

echo.
echo [4/5] Iniciando Backend em nova janela...
start "FleetManager Backend" cmd /k "dotnet run --project src/FleetManager.Api"

echo.
echo [5/5] Aguardando backend iniciar (10 segundos)...
timeout /t 10 /nobreak

echo.
echo Iniciando Frontend em nova janela...
start "FleetManager Frontend" cmd /k "cd frontend && npm start"

echo.
echo ========================================
echo   TUDO INICIADO COM SUCESSO!
echo ========================================
echo.
echo URLs de acesso:
echo   - Frontend: http://localhost:4200
echo   - API: http://localhost:5000
echo   - Swagger: http://localhost:5000/swagger
echo   - Seq (Logs): http://localhost:5341
echo.
echo Duas novas janelas foram abertas:
echo   1. Backend (porta 5000)
echo   2. Frontend (porta 4200)
echo.
echo Para parar tudo:
echo   - Feche as janelas do Backend e Frontend
echo   - Execute: docker-compose down
echo.
pause
