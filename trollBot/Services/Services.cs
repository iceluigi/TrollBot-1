using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;
using TrollBot.Services;

namespace TrollBot
{
    /// <summary>
    /// Represents the singleton hub of all Services for the application
    /// </summary>
    class Service
    {
        /// <summary>
        /// Initializes a new instance of the Services class. Private as a singleton.
        /// </summary>
        private Service() { }

        /// <summary>
        /// The backing field for the singleton instance of Services.
        /// </summary>
        private static IServiceProvider _current = null;

        /// <summary>
        /// The singleton instance of services
        /// </summary>
        public static IServiceProvider Current { get { return _current; } }

        /// <summary>
        /// Initializes the singelton of Services and all contained services.
        /// </summary>
        public static void ConfigureServices()
        {
            if (_current != null)
            {
                return;
            }

            _current = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<RoastService>()
                .AddSingleton<AudioService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
