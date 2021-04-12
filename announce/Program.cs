using announce.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Threading.Tasks;

namespace announce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .RunConsoleAsync();
        }

        public static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<BotOptions>(hostContext.Configuration.GetSection("Bot"));

            services.AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>();

            services.AddHttpClient();

            services.AddHostedService<BotService>();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
#if DEBUG
                    builder.AddUserSecrets<Program>();
#endif
                    builder.AddEnvironmentVariables("announce!");
                })
                .ConfigureServices(ConfigureServices);
    }
}
