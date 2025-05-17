using Microsoft.Extensions.Configuration;

namespace ToDoListClient.Helpers
{
    public static class Config
    {
        private static IConfigurationRoot? _config;

        public static IConfigurationRoot Load()
        {
            if (_config == null)
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            }

            return _config;
        }

        public static string GetApiBaseUrl() => Load()["Api:BaseUrl"]!;
        public static string GetSignalRHubUrl() => Load()["SignalR:HubUrl"]!;
    }
}
