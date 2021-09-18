using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventBusWebApi.Demo.Repositories
{
    public class MockRepo : IMockRepo
    {
        public async Task<string> DoThings()
        {
            return "Nothing Happens, Here is the string msg.";
        }

        public async Task<string> ThrowingHttpException()
        {
            throw new HttpRequestException(
                "Something happen from down stream services.",
                new Exception("Down Stream Server stupid Exception."),
                HttpStatusCode.InternalServerError);
        }

        public async Task<string> ThrowingCustomiseException()
        {
            throw new MockRepoException();
        }
    }

    public class MockRepoException : Exception
    {
        public string ErrorMessage = "Mock Repo Exception.";
    }
}