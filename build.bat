@echo off
echo ========================================
echo RJ VC Bypass - Windows Edition Builder
echo ========================================
echo.

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed!
    echo Please download and install .NET 6.0 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

echo Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages!
    pause
    exit /b 1
)

echo.
echo Building application...
dotnet build --configuration Release --no-restore
if %errorlevel% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build completed successfully!
echo ========================================
echo.
echo Output directory:
echo   bin\Release\net6.0-windows\
echo.
echo To publish as single executable:
echo   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
echo.
pause
