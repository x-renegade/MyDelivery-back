using Application.Common.Contracts;

namespace Api.Services
{
    public class AppConfiguration(IConfiguration configuration) : IAppConfiguration
    {
        public string GetValue(string key)
        {
            return configuration[key]!;
        }
    }
}
