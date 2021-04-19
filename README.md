# Orleans HelloWorld
It all started with a Hello World, but then the learning process begins ...

1) Hello World
2) Primes

### Tools:
- **.Net Core 3.1**: to write the code; -> https://dotnet.microsoft.com/download/dotnet/3.1
- **Orleans 3.4**: to support the code; -> https://dotnet.github.io/orleans/index.html
- **xUnit 2.4 & Moq 4.16**: to test the code;

To better understand Orleans, check out this link --> https://github.com/dotnet/orleans

### Nugets: external dependencies
--> https://www.nuget.org/packages?q=Microsoft.Extensions.Logging
- [Microsoft.Extensions.Logging.Console] -> Client & Silo
- [Microsoft.Extensions.Logging.Abstractions] -> Grain

--> https://www.nuget.org/packages?q=Microsoft.Orleans
- [Microsoft.Orleans.Client] -> Client
- [Microsoft.Orleans.CodeGenerator.MSBuild] -> Interface & Grain
- [Microsoft.Orleans.Core] -> Grain
- [Microsoft.Orleans.Core.Abstractions] -> Interface & Grain
- [Microsoft.Orleans.Runtime.Abstractions] -> Interface
- [Microsoft.Orleans.Server] -> Silo
- [Microsoft.Orleans.TestingHost] -> Test

# Structure
```
Project
├── Client
│   └── Samples
├── Grains
│   └── Storage
├── Interfaces
│   └── Consts
├── Silo
├── Tests
│   └── Fixture
├── .gitignore
├── build-and-run.ps1
└── README.md
```

- Interfaces are implemented on grains and called from the client view
- Interfaces consts are used to configure grains cluster
- Grains are sustained on the silos
- Grains storage is used to persist
- Test fixture is used to emulate a silo container

Note: As a silo add assemblies, it does not need to add every grain ;)

```
PS> .\build-and-run.ps1
```

### Server Side: - Silo
```
$ dotnet run --project Silo/Silo.csproj
```

### Client Side: - Client
```
$ dotnet run --project Client/Client.csproj
```

### Tests: - Tests
```
$ dotnet test
```