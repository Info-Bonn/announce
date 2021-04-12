using announce.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace announce
{
    internal class BotService : IHostedService
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly CommandHandlingService _commandHandlingService;
        private readonly CommandService _commandService;
        private readonly IOptions<BotOptions> _botOptions;
        private readonly ILogger<BotService> _logger;

        public BotService(
            DiscordSocketClient discorClient, 
            CommandHandlingService commandHandlingService, 
            CommandService commandService,
            IOptions<BotOptions> botOptions, 
            ILogger<BotService> logger)
        {
            _commandHandlingService = commandHandlingService;
            _commandService = commandService;
            _discordClient = discorClient;
            _botOptions = botOptions;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _discordClient.Log += LogDiscord;
            _commandService.Log += LogDiscord;

            await _discordClient.LoginAsync(TokenType.Bot, _botOptions.Value.Token);
            await _discordClient.StartAsync();

            await _commandHandlingService.InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordClient.StopAsync();
        }

        private Task LogDiscord(LogMessage arg)
        {
            var level = arg.Severity switch
            {
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                _ => throw new System.NotImplementedException()
            };

            _logger.Log(level, arg.Exception, arg.Message, arg.Source);

            return Task.CompletedTask;
        }
    }
}
