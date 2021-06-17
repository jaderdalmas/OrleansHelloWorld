using Orleans.TestingHost;

namespace Tests.Extension
{
  public static class TestClusterExtension
  {
    public static T GetService<T>(this TestCluster cluster) => (T)cluster.ServiceProvider.GetService(typeof(T));
  }
}
