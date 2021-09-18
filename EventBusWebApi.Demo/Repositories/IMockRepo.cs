using System.Threading.Tasks;

namespace EventBusWebApi.Demo.Repositories
{
    public interface IMockRepo
    {
        Task<string> DoThings();
        Task<string> ThrowingHttpException();
        Task<string> ThrowingCustomiseException();
    }
}