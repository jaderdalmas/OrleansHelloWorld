using System.Threading.Tasks;

namespace Interfaces
{
  public interface IReactiveCacheTo<T>
  {
    //VersionedValue<T> Cache { get; set; }
    //IDisposable Poll { get; set; }

    Task<T> GetAsync();
  }
}
