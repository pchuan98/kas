@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

where bun >nul 2>nul
if %errorlevel% neq 0 (
    powershell -c "irm bun.sh/install.ps1|iex"
    pause
    exit /b 1
)

bun run tiny.ts %*
pause