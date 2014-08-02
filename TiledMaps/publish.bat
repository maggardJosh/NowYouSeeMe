@ECHO OFF
IF NOT "%1"=="" GOTO setFilename
SET /P filename="Enter map name(don't include extension): "
GOTO finish

:setFilename
SET filename=%1 
for %%x in (%filename:\= %) do (set actualFilename=%%x)
set actualFilename = %actualFilename:~0,-4%.txt
echo 
echo %actualFilename%


:finish
copy %1 ..\Assets\Resources\Maps\%actualFilename:~0,-4%.txt && echo "Copied %actualFilename:~0,-4%.tmx to ..\Assets\Resources\Maps\%actualFilename:~0,-4%.txt" || echo "Nothing copied"

pause