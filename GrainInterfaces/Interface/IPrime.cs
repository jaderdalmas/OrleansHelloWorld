using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IPrime : Orleans.IGrainWithIntegerKey
  {
    Task<bool> IsPrime(int number);
  }
}
