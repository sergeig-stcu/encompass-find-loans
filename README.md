# README

## Overview

Provides a way to load a set of Encompass loans satisfying a condition.

Current implementation loads that have in their starting chain previous state set to null.  That is important if we want to make sure that loan state is genuine.

## Examples

```
dotnet run -- user/local.json
dotnet run -- user/stage.json
dotnet run -- user/prod.json
```

## References

1. [Getting Started](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-get-started)