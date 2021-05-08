using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IPrimeOnly : IGrainWithIntegerKey, IReactiveCacheTo<int>, IConsumer
  {
    Task UpdateAsync(int number);
  }
}
