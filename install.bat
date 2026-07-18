@echo off
REM ============================================
REM RimWorld Pawn AI Multithread - Install Script
REM ============================================
REM Этот скрипт помогает установить мод в RimWorld

setlocal enabledelayedexpansion

echo.
echo ===================================="
echo  RimWorld Pawn AI Multithread Mod
echo  Установщик v1.1.0
echo ===================================="
echo.

REM Проверяем наличие DLL
echo [1/4] Проверяю скомпилированный файл...
if not exist "Source\bin\Release\PawnAIMultithread.dll" (
    echo.
    echo ERROR: DLL файл не найден!
    echo.
    echo Сначала запустите: build.bat
    echo.
    pause
    exit /b 1
)
echo [OK] DLL файл найден!

REM Ищем RimWorld
echo [2/4] Ищу папку RimWorld...
set RIMWORLD_PATH=C:\Program Files\Steam\steamapps\common\RimWorld

if not exist "%RIMWORLD_PATH%" (
    echo.
    echo ERROR: RimWorld не найдена в стандартной папке!
    echo.
    echo Ожидаемая папка: %RIMWORLD_PATH%
    echo.
    echo Если RimWorld установлена в другом месте, отредактируйте этот скрипт:
    echo set RIMWORLD_PATH=ВАШ_ПУТЬ
    echo.
    pause
    exit /b 1
)
echo [OK] RimWorld найдена!

REM Создаём папки
echo [3/4] Создаю папки мода...
if not exist "%RIMWORLD_PATH%\Mods\PawnAIMultithread\Assemblies" (
    mkdir "%RIMWORLD_PATH%\Mods\PawnAIMultithread\Assemblies"
    echo [OK] Папка Assemblies создана
)
if not exist "%RIMWORLD_PATH%\Mods\PawnAIMultithread\About" (
    mkdir "%RIMWORLD_PATH%\Mods\PawnAIMultithread\About"
    echo [OK] Папка About создана
)

REM Копируем файлы
echo [4/4] Копирую файлы мода...
copy "Source\bin\Release\PawnAIMultithread.dll" "%RIMWORLD_PATH%\Mods\PawnAIMultithread\Assemblies\" >nul
if %errorlevel% equ 0 (
    echo [OK] DLL скопирован
) else (
    echo [ERROR] Не удалось скопировать DLL
    pause
    exit /b 1
)

copy "About\About.xml" "%RIMWORLD_PATH%\Mods\PawnAIMultithread\About\" >nul
if %errorlevel% equ 0 (
    echo [OK] About.xml скопирован
) else (
    echo [ERROR] Не удалось скопировать About.xml
    pause
    exit /b 1
)

copy "About\Manifest.xml" "%RIMWORLD_PATH%\Mods\PawnAIMultithread\About\" >nul
if %errorlevel% equ 0 (
    echo [OK] Manifest.xml скопирован
) else (
    echo [ERROR] Не удалось скопировать Manifest.xml
    pause
    exit /b 1
)

echo.
echo ===================================="
echo  УСТАНОВКА ЗАВЕРШЕНА!
echo ===================================="
echo.
echo Мод установлен в:
echo %RIMWORLD_PATH%\Mods\PawnAIMultithread\
echo.
echo Дальше:
echo 1. Откройте RimWorld
echo 2. Главное меню -^> Mods -^> Manage Mods
echo 3. Найдите "Pawn AI Multithread"
echo 4. Поставьте галочку
echo 5. Закройте меню и перезагрузитесь
echo 6. Создайте новую игру
echo.
echo Готово! Нажмите любую клавишу для выхода.
pause
exit /b 0