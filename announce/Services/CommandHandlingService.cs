using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace announce.Services
{
    class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discordClient;
        private readonly IServiceProvider _services;

        public CommandHandlingService(CommandService commands, DiscordSocketClient discordClient, IServiceProvider services)
        {
            _commands = commands;
            _discordClient = discordClient;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            _discordClient.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message) return;

            int argPos = 0;

            if (!(message.HasStringPrefix("a!", ref argPos) ||
                message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_discordClient, message);

            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);
        }
    }
}
