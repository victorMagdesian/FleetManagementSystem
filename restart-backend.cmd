@echo off
echo ========================================
echo   Reiniciando Backend - FleetManager
echo ========================================
echo.

echo [1] Parando processos na porta 5000...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5000 ^| findstr LISTENING') do (
    echo Matando processo %%a
    taskkill /F /PID %%a 2>nul
)

echo.
echo [2] Aguardando 2 segundos...
timeout /t 2 /nobreak >nul

echo.
echo [3] Iniciando backend...
echo.
start "FleetManager Backend" cmd /k "cd /d %~dp0 && dotnet run --project src/FleetManager.Api"

echo.
echo ========================================
echo Backend esta iniciando em nova janela!
echo Aguarde ver: "Now listening on: http://localhost:5000"
echo ========================================
echo.
pause
