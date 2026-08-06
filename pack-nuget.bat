@echo off
setlocal

REM ============================================================================
REM  Gera o pacote NuGet da biblioteca NfseNacional.PdfGenerator.Lib
REM  Uso:  pack-nuget.bat            -> usa a versao do .csproj
REM        pack-nuget.bat 1.1.0      -> sobrescreve a versao do pacote
REM ============================================================================

cd /d "%~dp0"

set "PROJETO=src\NfseNacional.PdfGenerator.Lib\NfseNacional.PdfGenerator.Lib.csproj"
set "SAIDA=nupkg"
set "CONFIG=Release"
set "VERSAO=%~1"

echo.
echo === NfseNacional.PdfGenerator - Empacotamento NuGet ===
echo.

REM Verifica se o dotnet esta disponivel
where dotnet >nul 2>nul
if errorlevel 1 (
    echo [ERRO] O .NET SDK ^(dotnet^) nao foi encontrado no PATH.
    echo         Instale em https://dotnet.microsoft.com/download
    goto :fim
)

echo [1/3] Restaurando e compilando em %CONFIG%...
dotnet build "%PROJETO%" -c %CONFIG%
if errorlevel 1 (
    echo.
    echo [ERRO] Falha na compilacao. Pacote NAO gerado.
    goto :fim
)

echo.
echo [2/3] Gerando o pacote .nupkg em "%SAIDA%"...
if defined VERSAO (
    echo        Versao sobrescrita: %VERSAO%
    dotnet pack "%PROJETO%" -c %CONFIG% -o "%SAIDA%" --no-build -p:PackageVersion=%VERSAO%
) else (
    dotnet pack "%PROJETO%" -c %CONFIG% -o "%SAIDA%" --no-build
)
if errorlevel 1 (
    echo.
    echo [ERRO] Falha ao gerar o pacote.
    goto :fim
)

echo.
echo [3/3] Concluido. Pacotes em "%CD%\%SAIDA%":
dir /b "%SAIDA%\*.nupkg" 2>nul

echo.
echo === Pacote gerado com sucesso ===

:fim
echo.
pause
endlocal
