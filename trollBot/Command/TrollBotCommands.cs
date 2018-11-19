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
        [Command("ping"), Alias("pong", "hello")]
        public Task PingAsync()
        {
            return ReplyAsync("pong!");
        }

        /// <summary>
        /// Adds a roast to the roast list in the roast service, and saves it to the disc.
        /// </summary>
        /// <param name="roast">The roast to add.</param>
        [Command("addroast", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
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
        [Command("roast", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task RoastAsync([Remainder] string username)
        {
            await ReplyAsync(Service.Current.GetRequiredService<RoastService>().GetRoast(username));
        }

        /// <summary>
        /// Joins a voice channel
        /// </summary>
        /// <returns></returns>
        [Command("join", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task JoinVoiceChannel()
        {
            var channel = Context.Guild.CurrentUser.VoiceChannel;
            await Service.Current.GetService<AudioService>().JoinAudioChannelTask(Context.Guild, channel);
        }

        /// <summary>
        /// Leaves voice channel
        /// </summary>
        /// <returns></returns>
        [Command("leave", RunMode = RunMode.Async),RequireContext(ContextType.Guild),
        RequireUserPermission(GuildPermission.Administrator)]
        public async Task LeaveVoiceChannel()
        {
            await Service.Current.GetService<AudioService>().LeaveAudioChannelTask(Context.Guild);
        }

        /*
        /// <summary>
        /// Plays an audio file in voice  channel
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [Command("play", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task PlayCmd()
        {
            await Service.Current.GetService<AudioService>().SendAudioAsync(Context.Guild, Context.Channel);
        }
        */

        /// <summary>
        /// Selects a user to stalk
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Command("Follow", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task StalkUser([Remainder] ulong userID)
        {
            if (Context.Guild.GetUser(userID) != null)
            {
                await Service.Current.GetService<AudioService>().SetStalkee(Context.Guild, userID);
                await ReplyAsync("Huehuehuehue");
            }
        }

        [Command("Stahp", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task StopStalking()
        {
            await Service.Current.GetService<AudioService>().SetStalkee(Context.Guild, 0);
        }
    }
}
