using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreBasicAuthentication.Tests
{
    public class TestServerUtilities
    {
        private static TestServer _testServer;
        private static readonly object _initializationLock = new object();

        public static TestServer GetTestServer()
        {
            if (_testServer == null)
            {
                InitializeTestServer();
            }
            return _testServer;
        }

        public static HttpClient Client => GetTestServer().CreateClient();

        private static void InitializeTestServer()
        {
            lock (_initializationLock)
            {
                if (_testServer != null)
                {
                    return;
                }
                var webHostBuilder = new WebHostBuilder()
                    .UseStartup<Startup>();
                _testServer = new TestServer(webHostBuilder);
                InitializeDatabase();
            }
        }

        private static void InitializeDatabase()
        {
            using (var serviceScope = _testServer.Host.Services.CreateScope())
            {
                DatabaseInitializer.InitializeDatabase(serviceScope)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
