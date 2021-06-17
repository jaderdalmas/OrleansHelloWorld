using System.Threading.Tasks;

namespace Interfaces
{
  public interface IAggregate
  {
    Task<bool> Apply(IEvent @event);

    Task Emit(IEvent @event);
  }
}
