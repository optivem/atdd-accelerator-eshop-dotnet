# Monolith (.NET)

This is a sample monolithic application written in .NET.

## Instructions

Open up the 'monolith' folder

```shell
cd monolith
```

Check that you have Powershell 7

```shell
$PSVersionTable.PSVersion
```

Ensure you have .NET 8:

```shell
dotnet --version
```

To build the project:

```shell
dotnet build
```

To run:

```shell
dotnet run --urls "http://localhost:8080"
```

To restart:
```shell
dotnet build && dotnet run --urls "http://localhost:8080"
```

App should now be running on:
http://localhost:8080/