using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IHello : Orleans.IGrainWithIntegerKey, IStreamSubscriptionObserver, IConsumer
  {
    Task<string> SayHello(string greeting);
  }
}
