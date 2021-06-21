using EventSourcing.Aggregate;
using EventSourcing.Event;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing
{
  public static class ServiceExtension
  {
    public static IServiceCollection AddAggregateServices(this IServiceCollection service)
    {
      service.AddSingleton<IStoreAggregate<IsPrimeEvent>, EventStoreAggregate<IsPrimeEvent>>();

      service.AddSingleton<IEventAggregate<IsPrimeEvent>, PrimeAggregate>();
      service.AddSingleton(_ => (PrimeAggregate)_.GetService<IEventAggregate<IsPrimeEvent>>());

      return service;
    }
  }
}
