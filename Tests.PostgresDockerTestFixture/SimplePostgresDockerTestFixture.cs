using TestingFixtures;
using TestUtilities.DatabaseContexts;

namespace Tests.PostgresDockerTestFixture;

public class SimplePostgresDockerTestFixture : PostgresDockerTestFixture<SimpleDbContext>;