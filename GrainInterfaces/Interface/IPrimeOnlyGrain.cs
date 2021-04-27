using Orleans;

namespace GrainInterfaces
{
  public interface IPrimeOnlyGrain : IGrainWithIntegerKey, IReactiveCacheTo<int>
  {
  }
}
