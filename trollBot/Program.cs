using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using TrollBot.Services;

namespace TrollBot
{
    /// <summary>
    /// The main class an entry point
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point of the program.
        /// </summary>
        /// <param name="args">Arguments passed in via command line. Currently not supported.</param>
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        /// <summary>
        /// Represents the path to the config json file.
        /// </summary>
        private const string configPath = "./config.json";

        /// <summary>
        /// The main entry point of the program, as an Async task.
        /// </summary>
        public async Task MainAsync()
        {
            try
            {
                Service.ConfigureServices();
                var services = Service.Current;
                var config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(configPath));

                // Check config to make sure token exists...
                if (string.IsNullOrEmpty(config.Token))
                {
                    throw new Exception("String retrieved from config is empty or null.");
                }

                var client = services.GetRequiredService<DiscordSocketClient>();
                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                await client.LoginAsync(TokenType.Bot, config.Token);
                await client.StartAsync();
                await services.GetRequiredService<Services.CommandHandler>().InitializeAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something went wrong when attempting to login!\n{0}", ex.ToString());
                Console.WriteLine("\nExiting program. Press any key to continue.");
                return;
            }

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        /// <summary>
        /// Method to call any time something needs to be logged.
        /// </summary>
        /// <param name="log">The message to be logged</param>
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
