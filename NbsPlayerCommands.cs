﻿using Obsidian.Commands;
using Obsidian.Entities;
using Qmmands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NbsPlayerPlugin
{
    [Group("nbspp")]
    public class NbsPlayerCommands : ModuleBase<CommandContext>
    {
        public CommandService Service { get; set; }

        [Command("play")]
        [Description("Plays back the specified song.")]
        public async Task PlayAsync([Remainder]string song)
        {
            if (!song.EndsWith(".nbs", StringComparison.InvariantCultureIgnoreCase))
            {
                song += ".nbs";
            }

            string path = Path.Combine(Context.Server.Path, "songs", song);
            NbsFile nbsFile = NbsFileReader.ReadNbsFile(path);

            bool songNameSpecified = !string.IsNullOrWhiteSpace(nbsFile.SongName);
            bool songAuthorSpecified = !string.IsNullOrWhiteSpace(nbsFile.OriginalSongAuthor);

            if (songNameSpecified || songAuthorSpecified)
            {
                string songName = songNameSpecified ? nbsFile.SongName : "Unknown";
                string songAuthor = songAuthorSpecified ? nbsFile.OriginalSongAuthor : "Unknown";
                await Context.Client.SendChatAsync($"{Constants.Prefix}Playing {songAuthor} - {songName}");
            }
            else
            {
                await Context.Client.SendChatAsync($"{Constants.Prefix}Playing {song}");
            }

            PlayerTask task;

            if (NbsPlayerPluginClass.Config.UseServerTicks)
            {
                task = new PlayerTask(nbsFile, Context.Client, Context.Server.TotalTicks);
            }
            else
            {
                task = new PlayerTask(nbsFile, Context.Client, DateTime.Now);
            }

            NbsPlayerPluginClass.Tasks.Add(task);
        }

        [Command("stop")]
        [Description("Stops the currently playing song")]
        public async Task StopAsync()
        {
            try
            {
                await NbsPlayerPluginClass.StopTaskAsync(Context.Player.Username);
                await Context.Client.SendChatAsync($"{Constants.Prefix}Stopped playing");
            }
            catch
            {
                await Context.Client.SendChatAsync($"{Constants.Prefix}You aren't playing a song right now.");
            }
        }

        [Command("mode")]
        [RequireOperator]
        public async Task SetModeAsync(string mode)
        {
            switch (mode.ToLowerInvariant())
            {
                default:
                    await Context.Client.SendChatAsync("§c/nbspp mode [tick|timer]");
                    break;

                case "tick":
                case "ticks":
                    NbsPlayerPluginClass.Config.UseServerTicks = true;
                    await Context.Client.SendChatAsync("§aTiming method set to server ticks");
                    break;

                case "timer":
                    NbsPlayerPluginClass.Config.UseServerTicks = false;
                    await Context.Client.SendChatAsync("§aTiming method set to timers");
                    break;
            }
        }
    }
}