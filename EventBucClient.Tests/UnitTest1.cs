using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EventBusWebApi.Demo;
using EventBusWebApi.Demo.Repositories;
using EventBusWebApi.Demo.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Caching;
using Xunit;

namespace EventBucClient.Tests
{
    public class UnitTest1
    {
        private readonly Mock<IMockRepo> _repo;
        private readonly Mock<ILogger<StupidService>> _logger;
        private IStupidService _service;
        private IsPolicy _policy;

        public UnitTest1()
        {
            _repo = new Mock<IMockRepo>();
            _logger = new Mock<ILogger<StupidService>>();
        }


        [Fact]
        public async Task Test1()
        {
            _policy = StupidPolicyBuilder.GetPolicy(_logger.Object);
            _service = new StupidService(_logger.Object, _policy, _repo.Object);
            _repo.Setup(repo => repo.ThrowingHttpException()).ThrowsAsync(new HttpRequestException(
                "Something happen from down stream services.",
                new Exception("Down Stream Server stupid Exception."),
                HttpStatusCode.InternalServerError));

            var exception = await Record.ExceptionAsync(async () =>
            {
                await _service.HereIsDoingStupidWork("walalalalala;");
            });

            Assert.Null(exception);
        }
    }
}