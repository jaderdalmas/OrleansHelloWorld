using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IHello : Orleans.IGrainWithIntegerKey, IStreamSubscriptionObserver
  {
    Task<string> SayHello(string greeting);
  }
}
