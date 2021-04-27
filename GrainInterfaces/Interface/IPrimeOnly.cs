using Orleans;

namespace GrainInterfaces
{
  public interface IPrimeOnly : IGrainWithIntegerKey, IReactiveCacheTo<int>, IConsumer
  {
  }
}
