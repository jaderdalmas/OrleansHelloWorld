using Xunit;

namespace Tests
{
  [CollectionDefinition(Name)]
  public class ClusterCollection : ICollectionFixture<ClusterFixture>
  {
    public const string Name = "ClusterCollection";
  }
}
