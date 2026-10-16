@echo off
chcp 65001 > nul
echo ============================================================
echo   Installation av Cabbe Areakontroll (C# Revit Add-in)
echo ============================================================
echo.

set "SCRIPT_DIR=%~dp0"
set "ADDIN_SRC=%SCRIPT_DIR%CabbeAreaAddin.addin"
set "DLL_SRC=%SCRIPT_DIR%CabbeAreaAddin.dll"

if not exist "%DLL_SRC%" (
    echo Fel: Hittade inte CabbeAreaAddin.dll i installationsmappen!
    pause
    exit /b 1
)

for %%V in (2025 2026) do (
    set "DEST_DIR=%APPDATA%\Autodesk\Revit\Addins\%%V"
    if exist "%APPDATA%\Autodesk\Revit\Addins\%%V" (
        echo Installerar för Revit %%V...
        copy /Y "%ADDIN_SRC%" "%DEST_DIR%\" > nul
        copy /Y "%DLL_SRC%" "%DEST_DIR%\" > nul
        echo   - Klart för Revit %%V.
    ) else (
        echo   - Revit %%V Addins-mapp hittades inte (hoppar över).
    )
)

echo.
echo ============================================================
echo   Klar! Starta Revit 2025 eller 2026.
echo   Fliken "Cabbe" med panelen "Areor" ska nu finnas i Ribbon.
echo ============================================================
echo.
pause
