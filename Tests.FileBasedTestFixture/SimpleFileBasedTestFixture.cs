using TestingFixtures;
using TestUtilities.DatabaseContexts;

namespace Tests.FileBasedTestFixture;

public abstract class SimpleFileBasedTestFixture : FileBasedTestFixture<SimpleDbContext>
{
}