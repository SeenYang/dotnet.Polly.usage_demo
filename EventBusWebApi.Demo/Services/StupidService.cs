using System;
using System.Threading.Tasks;
using EventBusWebApi.Demo.Repositories;
using Microsoft.Extensions.Logging;
using Polly;

namespace EventBusWebApi.Demo.Services
{
    public class StupidService:IStupidService
    {
        private readonly ILogger<StupidService> _logger;
        private readonly IsPolicy _policy;
        private readonly IMockRepo _repo;

        public StupidService(ILogger<StupidService> logger, IsPolicy policy, IMockRepo repo)
        {
            _logger = logger;
            _policy = policy;
            _repo = repo;
        }


        public async Task<string> HereIsDoingStupidWork(string inputStr)
        {
            var result = await Policy.Handle<Exception>()
                .RetryAsync()
                .ExecuteAndCaptureAsync(() => _repo.ThrowingHttpException());

            return result.Result;
        }
    }
}