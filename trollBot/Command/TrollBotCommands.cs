using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TrollBot
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
    }
}
