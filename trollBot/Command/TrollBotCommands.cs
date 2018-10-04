using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using TrollBot.Services;

namespace TrollBot.Commands
{
    /// <summary>
    /// Represents the repository for all commands used by TrollBot.
    /// </summary>
    public class TrollBotCommands : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// The ping command. Does stuff. Except not really.
        /// </summary>
        /// <returns>Async Task</returns>
        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
        {
            return ReplyAsync("pong!");
        }

        /// <summary>
        /// Adds a roast to the roast list in the roast service, and saves it to the disc.
        /// </summary>
        /// <param name="roast">The roast to add.</param>
        /// <returns>Async Task</returns>
        [Command("addroast", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddRoastAsync([Remainder] string roast)
        {
            bool result = await Service.Current.GetRequiredService<RoastService>().AddRoast(roast);
            if (result)
            {
                await ReplyAsync("Roast added!");
            }
            else
            {
                await ReplyAsync("Issue saving roast; roast is added but has not been saved to disc.");
            }
        }
    }
}
