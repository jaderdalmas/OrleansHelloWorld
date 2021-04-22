using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class PrimeController : ControllerBase
  {
    private readonly IGrainFactory _client;
    public PrimeController(IGrainFactory client)
    {
      _client = client;
    }

    [HttpGet("{number}")]
    public async Task<bool> Get(int number = 101)
    {
      var grain = _client.GetGrain<IPrime>(0);
      return await grain.IsPrime(number);
    }
  }
}
