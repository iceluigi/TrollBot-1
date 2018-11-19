using Discord;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord.Rest;


namespace TrollBot.Services
{
    public class AudioService
    {

        public AudioService()
        {
            ReadAudioList();
        }

        /// <summary>
        /// Path to audio files
        /// </summary>
        private string _audioPath = @"C:\Users\montj\Desktop\LTBot\TrollBot\trollBot\Files";

        /// <summary>
        /// The list of audio files to play
        /// </summary>
        private List<string> _audioList;
        
        /// <summary>
        /// The random number generator for the roasts class
        /// </summary>
        private Random _rngeezus = new Random();

        private ulong _stalkee;



        /// <summary>
        /// Keeps Track of which audio channel the bot is currently connected to
        /// </summary>
        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        /// <summary>
        /// Asynchronous Task for attempting to join an Audio Channel. 
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task JoinAudioChannelTask(IGuild guild, IVoiceChannel target)
        {
            //make sure guild and target exist
            if (guild == null || target == null)
            {
                return;
            }
            
            //Check current Audio Client. If it's in _connectedChannels, then the bot has already joined Audio Channel
            if (_connectedChannels.TryGetValue(guild.Id, out var client))
            {
                return;
            }

            //If voice channel's guild ID does not match our guild.
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            //Finally. Attempt to connect to the audio channel
            var audioClient = await target.ConnectAsync();

            //Update _connectedChannels with channel bot has joined
            if (_connectedChannels.TryAdd(guild.Id, audioClient))
            {
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

        /// <summary>
        /// Asynchronous Task for attempting to leave Audio Channel
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task LeaveAudioChannelTask(IGuild guild)
        {
            // make sure guild exists
            if (guild == null)
            {
                return; 
            }
            //TODO: Check if issues can arise from leaving an audio channel while audio is playing. Might have to stop audio file before disconnecting.

            //Attempt to remove audio client from dictionary. If successful then proceed to leave audio channel.
            if (_connectedChannels.TryRemove(guild.Id, out var client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }

        /// <summary>
        /// Asynchronous task for attempting to play and audio file.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channel"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel)
        {
           var path = GetAudioFile();

           if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }

            if (_connectedChannels.TryGetValue(guild.Id, out var client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                var output = CreateStream(path).StandardOutput.BaseStream;
                var stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        /// <summary>
        /// Creates a List of strings with all the .mp3 files in _audioPath
        /// </summary>
        /// <returns></returns>
        private bool ReadAudioList()
        {
            try
            {
                _audioList = new List<string>(System.IO.Directory.GetFiles(_audioPath, ".mp3"));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying to parse folder for .MP3 files:\n\n{0}\n\nUnable to infuriate members!", ex.ToString());
                _audioList = new List<string>();
                return false;
            }
        }

        /// <summary>
        /// Returns the path of the audio file to be played.
        /// </summary>
        /// <returns></returns>
        public string GetAudioFile()
        {
            var numberOfFiles = _audioList.Count;

            if (numberOfFiles == 0)
            {
                return string.Empty;
            }
            var roll = _rngeezus.Next(0, numberOfFiles);
            return _audioList[roll];


        }


        public async Task SetStalkee(IGuild guild, ulong userId)
        {
            //make sure guild exists
            if (guild == null)
            {
                return;
            }
            _stalkee = userId;
        }

        public ulong GetStalkee()
        {
            return _stalkee;
        }
    }
}