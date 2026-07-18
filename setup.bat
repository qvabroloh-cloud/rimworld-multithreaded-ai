@echo off
REM ============================================
REM RimWorld Pawn AI Multithread - Full Setup
REM ============================================
REM Этот скрипт автоматически выполняет ВСЁ

setlocal enabledelayedexpansion

echo.
echo ===================================="
echo  RimWorld Pawn AI Multithread Mod
echo  Полная установка v1.1.0
echo ===================================="
echo.
echo Этот скрипт:
echo 1. Скомпилирует код
echo 2. Установит мод в RimWorld
echo.
pause
echo.

REM Запускаем сборку
echo ШАГИ 1-5: КОМПИЛЯЦИЯ
echo.
call build.bat
if %errorlevel% neq 0 exit /b 1

echo.
echo.
echo ШАГИ 6-9: УСТАНОВКА
echo.
call install.bat
if %errorlevel% neq 0 exit /b 1

echo.
echo ===================================="
echo  ВСЁ ГОТОВО!
echo ===================================="
echo.
echo Теперь откройте RimWorld и наслаждайтесь модом!
echo.
pause