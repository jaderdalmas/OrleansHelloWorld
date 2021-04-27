using GrainInterfaces.Model;
using System;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IReactiveCacheTo<T>
  {
    VersionedValue<T> Cache { get; set; }
    IDisposable Pool { get; set; }

    Task<T> GetAsync() => Task.FromResult(Cache.Value);
  }
}
