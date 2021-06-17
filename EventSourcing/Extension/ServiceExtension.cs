using EventSourcing.Aggregate;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing
{
  public static class ServiceExtension
  {
    public static IServiceCollection AddAggregateServices(this IServiceCollection service)
    {
      service.AddSingleton<PrimeAggregate>();

      return service;
    }
  }
}
