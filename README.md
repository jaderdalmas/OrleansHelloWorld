# Orleans HelloWorld
It all started with a Hello World, but then the learning process begins ...

1) Hello World
2) Primes

To better understand Orleans, check out this link --> https://github.com/dotnet/orleans

## Server Side
- Silo

Command --> dotnet run --project Silo/Silo.csproj

## Client Side
- Client

Command --> dotnet run --project Client/Client.csproj

## Structure
- Interfaces are implemented on grains
- Interfaces are called from the client view
- Grains are sustained on the silos

Note: As a silo add assemblies, it does not need to add every grain ;)

Command --> '.\build-and-run.ps1' (PowerShell command for the full solution)