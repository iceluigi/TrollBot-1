using System;
using System.Collections.Generic;

namespace TrollBot.Services
{
    /// <summary>
    /// A repository for storing and retrieving roasts.
    /// </summary>
    class RoastService
    {
        /// <summary>
        /// Represents the path to the roasts text file.
        /// </summary>
        private string roastsPath = "./roasts.txt";

        /// <summary>
        /// The list of roasts,
        /// </summary>
        private List<string> _roasts;

        /// <summary>
        /// Initializes a new instance of the RoastService class.
        /// </summary>
        public RoastService()
        {
            try
            {
                _roasts = new List<string>(System.IO.File.ReadAllText(roastsPath).Split("~\n"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying to parse roasts file:\n\n{0}\n\nUnable to roast members!", ex.ToString());
                _roasts = new List<string>();
            }
        }

        /// <summary>
        /// Determines the probablility of the function RollRoast() succeeding.
        /// </summary>
        private const double roastProbability = 0.2;

        /// <summary>
        /// The random number generator for the roasts class
        /// </summary>
        private Random _rng = new Random();

        /// <summary>
        /// Determines, per message, if the TrollBot should roast a member.
        /// </summary>
        /// <returns>True if the bot should roast the member.</returns>
        public bool RollRoast()
        {
            if (_roasts.Count == 0)
            {
                return false;
            }

            double roll = _rng.Next(0, 100) / 100.0;
            return (roll < roastProbability);
        }

        /// <summary>
        /// Represents the placeholder for the username to raost.
        /// </summary>
        private const string usernamePlaceholder = "%USERNAME%";

        /// <summary>
        /// Gets a random roast from the roasts list.
        /// </summary>
        /// <param name="username">The name of the user to roast.</param>
        /// <returns>A random roast from the roasts list.</returns>
        public string GetRoast(string username)
        {
            if (_roasts.Count == 0)
            {
                return String.Empty;
            }

            int roll = _rng.Next(0, _roasts.Count);
            return _roasts[roll].Replace(usernamePlaceholder, username);
        }
    }
}
