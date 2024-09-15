using TestingFixtures;
using TestUtilities.DatabaseContexts;

namespace Tests.PostgresDockerTestFixture;

public abstract class SimplePostgresDockerTestFixture : PostgresDockerTestFixture<SimpleDbContext>;