# Orleans HelloWorld
It all started with a Hello World, but then the learning process begins ...

1) Hello World
2) Primes

### Tools:
This project was developed using: 
- **.Net Core 3.1**: to write the code; -> https://dotnet.microsoft.com/download/dotnet/3.1
- **Orleans 3.4**: to support the code; -> https://dotnet.github.io/orleans/index.html
- **xUnit 2.4 & Moq 4.16**: to test the code;

To better understand Orleans, check out this link --> https://github.com/dotnet/orleans

### Nugets:
- [Microsoft.Extensions.Logging.Console]
- [Microsoft.Extensions.Logging.Abstractions]
- [Microsoft.Orleans.Client]
- [Microsoft.Orleans.CodeGenerator.MSBuild]
- [Microsoft.Orleans.Core]
- [Microsoft.Orleans.Core.Abstractions]
- [Microsoft.Orleans.Runtime.Abstractions]
- [Microsoft.Orleans.Server]
- [Microsoft.Orleans.TestingHost]

-----

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

## Structure
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
PS> '.\build-and-run.ps1'
```