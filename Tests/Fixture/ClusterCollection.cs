using Xunit;

namespace Tests
{
  [CollectionDefinition(nameof(ClusterCollection))]
  public class ClusterCollection : ICollectionFixture<ClusterFixture> { }
}
