# This script assumes sqlite3.exe is in your PATH!

$FileName = "$env:TEMP\Stormshrike.db"
if (Test-Path $FileName) {
  echo "Removing $FileName" 
  Remove-Item $FileName
}

echo "Loading Projects.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Projects.sql"
echo "Turning on Foreign Keys"
sqlite3 $env:TEMP\Stormshrike.db ".read Tasks.sql"
echo "Loading Contexts.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Contexts.sql"



echo "Loading Contexts-Defaults.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Contexts-Defaults.sql"

echo "Loading Projects-testdata.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Projects-testdata.sql"
echo "Loading Tasks-testdata.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Tasks-testdata.sql"
