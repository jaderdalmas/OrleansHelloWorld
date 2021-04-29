using Orleans;

namespace Interfaces
{
  public interface IPrimeOnly : IGrainWithIntegerKey, IReactiveCacheTo<int>, IConsumer
  {
  }
}
