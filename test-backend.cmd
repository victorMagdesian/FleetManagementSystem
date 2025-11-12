@echo off
echo ========================================
echo   Testando Backend - FleetManager
echo ========================================
echo.

echo [1] Testando se o backend esta rodando...
curl -s http://localhost:5000/api/vehicles
if %errorlevel% neq 0 (
    echo.
    echo ERRO: Backend nao esta respondendo em http://localhost:5000
    echo.
    echo Verifique se o backend esta rodando:
    echo   dotnet run --project src/FleetManager.Api
    echo.
    pause
    exit /b 1
)

echo.
echo.
echo [2] Backend esta respondendo! ✓
echo.
echo [3] Testando CORS...
echo Abra o navegador em http://localhost:4200
echo Abra o Console (F12) e execute:
echo.
echo fetch('http://localhost:5000/api/vehicles').then(r =^> r.json()).then(console.log)
echo.
echo Se funcionar, o CORS esta OK!
echo.
pause
