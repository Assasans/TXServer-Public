robocopy "%~1StateServer" "%~2StateServer" /mir /xo /nfl /ndl /njs /nc /ns /np /xf "tankix.tgz"
if %ERRORLEVEL% EQU 1 goto do_tgz
if %ERRORLEVEL% EQU 3 goto do_tgz
if %ERRORLEVEL% EQU 5 goto do_tgz
if %ERRORLEVEL% EQU 7 goto do_tgz
if %ERRORLEVEL% GEQ 8 exit %ERRORLEVEL%
exit 0
:do_tgz
del /s /q "%~2StateServer\resources\StandaloneWindows\tankix.tgz"
"%~17-Zip\7z.exe" a -ttar -so -an "%~2StateServer\client\*" | "%~17-Zip\7z.exe" a -si "%~2StateServer\resources\StandaloneWindows\tankix.tgz"