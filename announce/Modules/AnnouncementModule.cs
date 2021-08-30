using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace announce.Modules
{
    [Group("announce")]
    public sealed class AnnouncementModule : ModuleBase<SocketCommandContext>
    {
        private readonly HttpClient _http;

        public AnnouncementModule(HttpClient http)
        {
            _http = http;
        }

        [Command("new")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreateAnnouncement(ITextChannel channel, [Remainder] string message = null)
        {
            var text = Context.Message.Attachments
                .Where(a => a.Filename.EndsWith(".txt"))
                .FirstOrDefault();

            if (text != null)
            {
                message = await _http.GetStringAsync(text.Url);
            }

            var msg = await channel.SendMessageAsync(message);
        }

        [Command("edit")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task EditAnnouncement(string link, [Remainder] string content = null)
        {
            var text = Context.Message.Attachments
                .Where(a => a.Filename.EndsWith(".txt"))
                .FirstOrDefault();

            if (text != null)
            {
                content = await _http.GetStringAsync(text.Url);
            }

            var uri = new Uri(link);
            var parts = uri.AbsolutePath.Split("/")
                .Select(i => (ulong.TryParse(i, out var id), id))
                .Where(x => x.Item1)
                .ToList();

            if (parts.Count != 3)
            {
                throw new UriFormatException("Not a valid Message Uri");
            }

            var channel = Context.Client.GetChannel(parts[1].id);

            if (channel is ITextChannel textChannel)
            {
                var message = await textChannel.GetMessageAsync(parts[2].id);

                if (message is IUserMessage userMessage)
                {
                    await userMessage.ModifyAsync(m => m.Content = Optional.Create(content));
                    return;
                }

                throw new UriFormatException("Not a message the bot wrote");
            }

            throw new UriFormatException("Invalid channel");
        }
    }
}
