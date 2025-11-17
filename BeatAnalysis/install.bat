@echo off
echo ===================================
echo Magic Tiles AI - Audio Analysis Setup
echo ===================================
echo.

echo Checking Python installation...
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Python not found!
    echo Please install Python from: https://www.python.org/downloads/
    echo Make sure to check "Add Python to PATH" during installation.
    pause
    exit /b 1
)

echo Python found!
echo.

echo Installing required packages...
echo This may take a few minutes...
echo.

python -m pip install --upgrade pip
python -m pip install -r requirements.txt

if %errorlevel% equ 0 (
    echo.
    echo ===================================
    echo Installation completed successfully!
    echo ===================================
    echo.
    echo You can now use Audio Analysis in Magic Tiles AI.
    echo The app will automatically analyze your music files.
) else (
    echo.
    echo ===================================
    echo Installation failed!
    echo ===================================
    echo.
    echo Please check the error messages above.
    echo You may need to install Visual C++ from:
    echo https://aka.ms/vs/17/release/vc_redist.x64.exe
)

echo.
pause
