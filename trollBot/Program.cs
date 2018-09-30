using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace TrollBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // Try to retrieve config
            Config config;
            try
            {
                config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("./config.json"));

                // Check config to make sure token exists...
                if (config.Token == String.Empty || config.Token == null)
                {
                    throw new Exception("String retrieved from config is empty or null.");
                }

                var client = new DiscordSocketClient();
                client.Log += LogAsync;
                await client.LoginAsync(TokenType.Bot, config.Token);
                await client.StartAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something went wrong when attempting to login!\n{0}", ex.ToString());
                Console.WriteLine("\nExiting program. Press any key to continue.");
                Console.ReadKey();
                return;
            }

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
