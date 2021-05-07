using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Grains
{
  public static class ServiceExtension
  {
    public static IServiceCollection AddGrainServices(this IServiceCollection service)
    {
      service.AddScoped<IESService<int>, ESService<int>>();

      return service;
    }
  }
}
