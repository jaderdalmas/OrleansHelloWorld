using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
  public static class RCExtension
  {
    /// <param name="grain">Grain to add RC</param>
    /// <param name="poll">poll method (grain from)</param>
    /// <param name="validate">validate poll response</param>
    /// <param name="apply">apply method (grain to)</param>
    /// <param name="failed">failed method (grain to)</param>
    private static async Task RegisterRCPoolAsync<T>(this Grain grain, Func<Task<T>> poll, Func<T, bool> validate, Func<T, Task> apply, Func<T, Task> failed = null)
    {
      try
      {
        var update = await poll();
        if (validate(update))
        {
          await apply(update);
        }
        else if (failed != null)
        {
          await failed(update);
        }
      }
      catch (TimeoutException) { }
    }
  }
}
