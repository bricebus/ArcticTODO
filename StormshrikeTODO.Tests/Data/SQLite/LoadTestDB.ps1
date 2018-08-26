# This script assumes sqlite3.exe is in your PATH!

 param (
    [string]$dblocation = "$env:TEMP\Stormshrike.db"
 )

echo "Loading Projects-testdata.sql"
sqlite3 $dblocation ".read Projects-testdata.sql"
echo "Loading Tasks-testdata.sql"
sqlite3 $dblocation ".read Tasks-testdata.sql"
