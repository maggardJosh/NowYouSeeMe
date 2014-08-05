@ECHO OFF
IF NOT "%1"=="" GOTO setFilename
echo Drag file onto this batch file to move it to the assets folder
GOTO end

:setFilename
SET filename=%1 
for %%x in (%filename:\= %) do (set actualFilename=%%x)
set actualFilename = %actualFilename:~0,-4%.txt
echo 
echo %actualFilename%


:finish
move %1 ..\Assets\Resources\Atlases\%actualFilename:~0,-4%.txt && echo "Copied %actualFilename:~0,-4%.fnt to ..\Assets\Resources\Atlases\%actualFilename:~0,-4%.txt" || echo "Nothing copied"

:end
pause