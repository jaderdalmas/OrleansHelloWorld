$mode = 'Release'

dotnet restore
if ($LastExitCode -ne 0) { return; }

dotnet build --no-restore -c $mode
if ($LastExitCode -ne 0) { return; }

Start-Process "dotnet" -ArgumentList "run --project Silo/Silo.csproj --no-build -c $mode"
Start-Sleep 10
Start-Process "dotnet" -ArgumentList "run --project Client/Client.csproj --no-build -c $mode"