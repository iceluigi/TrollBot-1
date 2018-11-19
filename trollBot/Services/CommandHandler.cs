using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Audio;

namespace TrollBot.Services
{
    /// <summary>
    /// Represents a service to handle commands for the TrollBot
    /// </summary>
    public class CommandHandler
    {
        /// <summary>
        /// A reference shortcut to the CommandService singleton
        /// </summary>
        private readonly CommandService _commands;

        /// <summary>
        /// A reference shortcut to the DiscordSocketClient singleton
        /// </summary>
        private readonly DiscordSocketClient _discord;

        /// <summary>
        /// A reference shortcut to the RoastService singleton
        /// </summary>
        private readonly RoastService _roasts;

        /// <summary>
        /// A reference shortcut to the AudioService singleton
        /// </summary>
        private readonly AudioService _audio;

        // TODO: Move this to config
        /// <summary>
        /// The prefix for the bot to use for commands.
        /// </summary>
        private const char prefix = '~';

        // TODO: Move this to config
        /// <summary>
        /// The full bot-name prefix to use for commands
        /// </summary>
        private const string longPrefix = "troll ";

        /// <summary>
        /// Initializes a new instance of the CommandHandler class
        /// </summary>
        public CommandHandler()
        {
            _commands = Service.Current.GetRequiredService<CommandService>();
            _discord = Service.Current.GetRequiredService<DiscordSocketClient>();
            _roasts = Service.Current.GetRequiredService<RoastService>();
            _audio = Service.Current.GetRequiredService<AudioService>();
            _discord.MessageReceived += MessageReceivedAsync;
            _discord.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
            _discord.GuildMemberUpdated += GuildMemberUpdatedAsync;
        }

        /// <summary>
        /// Initializes the CommandHandler
        /// </summary>
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// A callback method to be used whenever the bot detects a user joining a voice channel
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldVoiceState"></param>
        /// <param name="newVoiceState"></param>
        /// <returns></returns>
        public async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            
            if (user.Id == _audio.GetStalkee())
            {
                var guild = oldVoiceState.VoiceChannel.Guild;

                //Will have to double check this. I'm assuming VoiceChannel will be null when user is not in a voice channel
                if (newVoiceState.VoiceChannel != null)
                {
                    await Service.Current.GetService<AudioService>()
                        .JoinAudioChannelTask(guild, newVoiceState.VoiceChannel);
                }
                else
                    await Service.Current.GetService<AudioService>().LeaveAudioChannelTask(guild);
                
            }
        }

        public async Task GuildMemberUpdatedAsync(SocketGuildUser userOld, SocketGuildUser userNew)
        {
            var client = userNew.Guild.AudioClient;
            userNew.Guild
            client.SpeakingUpdated += OnClientSpeaking;
        }

        

        private async Task OnClientSpeaking(ulong userID, bool speaking)
        {
            await 
        }

        /// <summary>
        /// A callback method to be used whenever the bot detects a message has been received
        /// </summary>
        /// <param name="rawMessage">The message to process</param>
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage))
                return;
            var message = rawMessage as SocketUserMessage;
            if (message.Source != MessageSource.User)
                return;

            // Try to get reply to message as a command
            string reply = await TryMessageAsCommand(message);
            if (reply == String.Empty) // Try to reply to the message anyways, as a TrollBot would
            {
                if (_roasts.RollRoast())
                {
                    string userToRoast = message.Author.Username;

                    if (message.Author is IGuildUser && ((message.Author as IGuildUser).Nickname != null))
                    {
                        userToRoast = (message.Author as IGuildUser).Nickname;
                    }
                    reply = _roasts.GetRoast(userToRoast);
                }
            }

            if (reply != String.Empty)
            {
                await message.Channel.SendMessageAsync(reply);
            }
        }

        /// <summary>
        /// Parses the message and executes it as a command, if applicable
        /// </summary>
        /// <param name="message">The message to parse</param>
        /// <returns>The reply to the message, if the message was a command</returns>
        private async Task<string> TryMessageAsCommand(SocketUserMessage message)
        {
            var argPos = 0; // This value holds the offset where the prefix ends

            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) &&
                !message.HasCharPrefix(prefix, ref argPos) &&
                !message.HasStringPrefix(longPrefix, ref argPos))
            {
                return String.Empty;
            }

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, Service.Current);

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand)
            {
                return result.ToString();
            }

            return String.Empty;
        }
    }
}
