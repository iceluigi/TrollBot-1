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

        /// <summary>
        /// Roasts an user
        /// </summary>
        /// <param name="username">The name of the user to roast</param>
        [Command("roast", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RoastAsyc([Remainder] string username)
        {
            await ReplyAsync(Service.Current.GetRequiredService<RoastService>().GetRoast(username));
        }
    }
}
