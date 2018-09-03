# This script assumes sqlite3.exe is in your PATH!

 param (
    [string]$dblocation = "$env:TEMP\Stormshrike.db"
 )


if (Test-Path $dblocation) {
  echo "Removing $dblocation" 
  Remove-Item $dblocation
}

echo "Loading Projects.sql"
sqlite3 $dblocation ".read Projects.sql"
echo "Loading Tasks"
sqlite3 $dblocation ".read Tasks.sql"
echo "Loading Contexts.sql"
sqlite3 $dblocation ".read Contexts.sql"

echo "Loading Contexts-Defaults.sql"
sqlite3 $dblocation ".read Contexts-Defaults.sql"