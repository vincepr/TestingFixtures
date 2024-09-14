# TestingFixture
Quickly setup tests for your .NET-EntityFrameWorkCore classes that use DbContext directly or IDbContextFactory<DbContext> directly.

## Benefits
* Access real databases in your UnitTests by using Sqlite-via-Files or Postgresql-via-Docker.
* Both Sqlite and TestContainers will run in CI-CD-Pipelines (as tests-step in this project's workflow should show)
* The InMemoryDb keeps Items and References in context longer than in production and might introduce sublte Bugs in your Unit Tests.
* Sqlite is still really fast, even compared to the InMemoryDb-Variant
* Postgres via Docker will create a real Database as used in production. So even extra logic like triggers, custom-database-functions etc. can be tested.
