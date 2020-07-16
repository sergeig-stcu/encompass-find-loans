# README

## Overview

Provides a way to load a set of Encompass loans satisfying a condition.

Current implementation loads that have in their starting chain previous state set to null.  That is important if we want to make sure that loan state is genuine.

## History

### version-1 Implementation

Gets a set of loans without previous state.  If Encompas has loan with previous state in its 1st status, then it means that some data has been removed.  When Encompass runs normally, all loans shall start without previous state.

Then take first 10 loans for each milestone group.

### version-2 implementation

Take loans without previous state, then sort by milestoneCurrentDateUtc DESC and take 10 first loans from each milestone.

## Examples

```
dotnet run -- user/local.json
dotnet run -- user/stage.json
dotnet run -- user/prod.json
```

## References

1. [Getting Started](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-get-started)