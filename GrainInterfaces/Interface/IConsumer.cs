using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IConsumer
  {
    Task Consume();
  }
}
