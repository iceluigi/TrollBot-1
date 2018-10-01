using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TrollBot
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

        // TODO: Move this to config
        /// <summary>
        /// The prefix for the bot to use for commands.
        /// </summary>
        private const char prefix = '~';

        // TODO: Move this to config
        /// <summary>
        /// The full bot-name prefix to use for commands
        /// </summary>
        private const string longPrefix = "troll";

        /// <summary>
        /// Initiailizes a new instance of the CommandHandler class
        /// </summary>
        public CommandHandler()
        {
            _commands = Services.Current.GetRequiredService<CommandService>();
            _discord = Services.Current.GetRequiredService<DiscordSocketClient>();
            _discord.MessageReceived += MessageReceivedAsync;
        }

        /// <summary>
        /// Initializes the CommandHandler
        /// </summary>
        /// <returns>Async task</returns>
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// A callback method to be used whenever the bot detects a message has been received
        /// </summary>
        /// <param name="rawMessage">The message to process</param>
        /// <returns>Async tasak</returns>
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
                // TODO: Implement 
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
            var result = await _commands.ExecuteAsync(context, argPos, Services.Current);

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand)
            {
                return result.ToString();
            }

            return String.Empty;
        }
    }
}
