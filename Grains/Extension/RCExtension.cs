using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
  public static class RCExtension
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="grain">Grain to add RC</param>
    /// <param name="initialize">initialize method (grain from)</param>
    /// <param name="poll">poll method (grain from)</param>
    /// <param name="validate">validate poll response</param>
    /// <param name="apply">apply method (grain to)</param>
    /// <param name="failed">failed method (grain to)</param>
    /// <returns></returns>
    public static async Task RegisterRCPoolAsync<T>(this Grain grain, Func<Task<T>> initialize, Func<Task<T>> poll, Func<T, bool> validate, Func<T, Task> apply, Func<T, Task> failed = null)
    {
      if (initialize != null)
      {
        var init = await initialize();
        await apply(init);
      }

      await ExecuteRCPoolAsync(poll, validate, apply, failed);
    }

    private static async Task ExecuteRCPoolAsync<T>(Func<Task<T>> poll, Func<T, bool> validate, Func<T, Task> apply, Func<T, Task> failed = null)
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
