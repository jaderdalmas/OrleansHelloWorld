namespace Interfaces
{
  public interface IAggregate<T> : IEventAggregate<T>, IStoreAggregate<T>
    where T : IEvent
  {
  }
}
