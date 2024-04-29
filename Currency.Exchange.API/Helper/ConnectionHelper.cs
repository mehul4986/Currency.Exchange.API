using StackExchange.Redis;

namespace Currency.Exchange.API.Helper
{
    public class ConnectionHelper
    {
        public static string RedisURL => System.Environment.GetEnvironmentVariable("RedisURL");

        private static Lazy<ConnectionMultiplexer> lazyConnMultiplexer;

        static ConnectionHelper()
        {
            ConnectionHelper.lazyConnMultiplexer = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(RedisURL);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnMultiplexer.Value;
            }
        }
    }
}