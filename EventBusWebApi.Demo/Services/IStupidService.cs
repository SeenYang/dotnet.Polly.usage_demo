using System.Threading.Tasks;

namespace EventBusWebApi.Demo.Services
{
    public interface IStupidService
    {
        Task<string> HereIsDoingStupidWork(string inputStr);
    }
}