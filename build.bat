@echo off
REM ============================================
REM RimWorld Pawn AI Multithread - Build Script
REM ============================================
REM Этот скрипт автоматически компилирует мод

setlocal enabledelayedexpansion

echo.
echo ===================================="
echo  RimWorld Pawn AI Multithread Mod
echo  Автоматическая сборка v1.1.0
echo ===================================="
echo.

REM Проверяем наличие MSBuild
echo [1/5] Ищу MSBuild...
where msbuild >nul 2>&1
if %errorlevel% neq 0 (
    echo.
    echo ERROR: MSBuild не найден!
    echo.
    echo Решение:
    echo 1. Откройте Visual Studio Installer
    echo 2. Нажмите "Modify"
    echo 3. Выберите ".NET desktop development"
    echo 4. Нажмите "Modify"
    echo 5. Перезагрузитесь и запустите этот скрипт заново
    echo.
    pause
    exit /b 1
)
echo [OK] MSBuild найден!

REM Проверяем наличие проекта
echo [2/5] Проверяю файлы проекта...
if not exist "Source\PawnAIMultithread.csproj" (
    echo.
    echo ERROR: Файл PawnAIMultithread.csproj не найден!
    echo.
    echo Убедитесь, что вы запустили этот скрипт из папки проекта:
    echo rimworld-multithreaded-ai
    echo.
    pause
    exit /b 1
)
echo [OK] Проект найден!

REM Очищаем старые файлы
echo [3/5] Очищаю старые файлы...
if exist "Source\bin\Release\PawnAIMultithread.dll" (
    del "Source\bin\Release\PawnAIMultithread.dll"
    echo [OK] Старые файлы удалены
) else (
    echo [OK] Нечего чистить
)

REM Выполняем сборку
echo [4/5] Компилирую код (это займёт 30-60 секунд)...
echo.
msbuild Source\PawnAIMultithread.csproj /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal

if %errorlevel% neq 0 (
    echo.
    echo ===================================="
    echo  ОШИБКА ПРИ КОМПИЛЯЦИИ!
    echo ===================================="
    echo.
    echo Возможные причины:
    echo 1. .NET Framework 4.7.2 не установлена
    echo 2. RimWorld не установлена в стандартную папку
    echo 3. Отсутствуют файлы проекта
    echo.
    pause
    exit /b 1
)

REM Проверяем результат
echo.
echo [5/5] Проверяю результаты...
if not exist "Source\bin\Release\PawnAIMultithread.dll" (
    echo.
    echo ERROR: DLL файл не был создан!
    echo.
    pause
    exit /b 1
)

echo.
echo ===================================="
echo  УСПЕШНО!
echo ===================================="
echo.
echo Готовый файл находится:
echo Source\bin\Release\PawnAIMultithread.dll
echo.
echo Дальше:
echo 1. Создайте папку: RimWorld\Mods\PawnAIMultithread\
echo 2. Создайте подпапки: Assemblies\ и About\
echo 3. Скопируйте DLL в Assemblies\
echo 4. Скопируйте About.xml и Manifest.xml в About\
echo 5. Запустите RimWorld
echo 6. Mods -^> Manage Mods -^> Включите PawnAIMultithread
echo.
echo Готово! Нажмите любую клавишу для выхода.
pause
exit /b 0