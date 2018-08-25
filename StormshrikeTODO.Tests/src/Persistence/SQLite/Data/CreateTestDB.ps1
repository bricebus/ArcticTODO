# This script assumes sqlite3.exe is in your PATH!

$FileName = "$env:TEMP\Stormshrike.db"
if (Test-Path $FileName) {
  echo "Removing $FileName" 
  Remove-Item $FileName
}

echo "Loading Projects-testdata.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Projects-testdata.sql"
echo "Loading Tasks-testdata.sql"
sqlite3 $env:TEMP\Stormshrike.db ".read Tasks-testdata.sql"
