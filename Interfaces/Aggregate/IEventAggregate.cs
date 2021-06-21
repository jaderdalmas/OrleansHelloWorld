using System.Threading.Tasks;

namespace Interfaces
{
  public interface IEventAggregate<T> where T : IEvent
  {
    Task<bool> Apply(T @event);
  }
}
