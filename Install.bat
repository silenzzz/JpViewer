@echo off

set dir=C:\Users\%USERNAME%\DeMmAgeSoft

if exist %dir% ( goto Exists ) ELSE ( goto NotExists )

:NotExists
echo Creating directories...
MKDIR %dir%

:Exists

echo Copying files...
copy /Y JpViewer.exe %dir%\JpViewer.exe

echo Adding to PATH...
Powershell.exe -File AddToPath.ps1 %dir%

if %ERRORLEVEL% NEQ 0 ( goto Error ) ELSE ( goto Success )

:Error
echo ERROR OCCURRED DURING INSTALLATION
pause
exit /b 1

:Success
echo INSTALLATION SUCCESSFUL
pause
exit /b 0