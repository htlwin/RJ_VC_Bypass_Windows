@echo off
echo ========================================
echo RJ VC Bypass - Windows Edition Publisher
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

echo Publishing application...
echo.

REM Create publish directory
if not exist "publish" mkdir "publish"

REM Publish as single executable with self-contained runtime
dotnet publish -c Release -r win-x64 --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:PublishReadyToRun=true ^
    -o "publish"

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Publishing failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Publishing completed successfully!
echo ========================================
echo.
echo Output directory: publish\
echo.
echo Files created:
dir /b "publish"
echo.
echo You can distribute the contents of the 'publish' folder.
echo The executable is approximately 70-80MB due to included .NET runtime.
echo.
pause
