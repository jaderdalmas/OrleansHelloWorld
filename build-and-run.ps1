$mode = 'Release'

dotnet build -c $mode
if ($LastExitCode -ne 0) { return; }

Start-Process "dotnet" -ArgumentList "run --project Silo/Silo.csproj --no-build -c $mode"
Start-Process "dotnet" -ArgumentList "run --project Client/Client.csproj --no-build -c $mode"