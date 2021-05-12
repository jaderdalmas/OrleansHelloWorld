using Interfaces.Model;
using System;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IRXService<T>
  {
    Task<VersionedValue<T>> GetAsync();

    Task UpdateAsync(T value);

    Task Subscribe(IObserver<T> observer);
  }
}
