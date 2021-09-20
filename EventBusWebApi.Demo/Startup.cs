using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EventBusWebApi.Demo.Repositories;
using EventBusWebApi.Demo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Retry;

namespace EventBusWebApi.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // IoC
            services.AddTransient<IStupidService, StupidService>();
            services.AddTransient<IMockRepo, MockRepo>();
            // Resiliency Policies
            services.GetRetryPolicy(LoggerFactory.CreateLogger("GetRetryPolicyLogger"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventBusWebApi.Demo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventBusWebApi.Demo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }


    public static class StupidPolicyBuilder
    {
        public static void GetRetryPolicy(this IServiceCollection services, ILogger logger)
        {
            var policy = GetPolicy(logger);
            var registry = new PolicyRegistry
            {
                { "stupid_policy", policy }
            };

            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
        }

        public static AsyncRetryPolicy GetPolicy(ILogger logger)
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        OnHttpRetry(null, timeSpan, retryCount, context, logger);
                    });

            return policy;
        }

        private static void OnHttpRetry(DelegateResult<HttpResponseMessage> result, TimeSpan timeSpan, int retryCount,
            Context context, ILogger logger)
        {
            if (result?.Result != null)
            {
                var msg =
                    $"Request failed with {result.Result.StatusCode}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}";
                Console.Error.WriteLine(msg);
                logger.LogWarning(msg,
                    result.Result.StatusCode, timeSpan, retryCount);
            }
            else
            {
                var msg =
                    $"Request failed because network failure. Waiting {timeSpan} before next retry. Retry attempt {retryCount}";
                Console.Error.WriteLine(msg);
                logger.LogWarning(msg,
                    timeSpan, retryCount);
            }
        }

        private static TimeSpan ComputeDuration(int input)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, input)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100));
        }
    }
}