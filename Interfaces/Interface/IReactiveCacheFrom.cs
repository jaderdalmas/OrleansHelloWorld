using Interfaces.Model;
using System.Threading.Tasks;

namespace Interfaces
{
  public interface IReactiveCacheFrom<T>
  {
    //VersionedValue<T> Value { get; set; }
    //TaskCompletionSource<VersionedValue<T>> Wait { get; set; }

    /// <summary>
    /// Returns the current state without polling.
    /// </summary>
    Task<VersionedValue<T>> GetAsync();

    /// <summary>
    /// Update the current state and release Long Poll Wait
    /// </summary>
    Task UpdateAsync(T value);

    /// <summary>
    /// If the given version is the same as the current version then resolves when a new version of data is available.
    /// If no new data become available within the orleans response timeout minus some margin, then resolves with a "no data" response.
    /// Otherwise returns the current data immediately.
    /// </summary>
    Task<VersionedValue<T>> LongPollAsync(VersionToken knownVersion);
  }
}
