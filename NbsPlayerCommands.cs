using Obsidian.Commands;
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

            {
                await Context.Player.SendMessageAsync($"{Constants.Prefix}Playing {song}");
            }

            NbsFile nbsFile = NbsFileReader.ReadNbsFile(path);

            await Context.Player.SendMessageAsync($"§aPlaying {nbsFile.GetLabel()}");

            PlayerTask task;

            if (NbsPlayerPluginClass.Config.UseServerTicks)
            {
                task = new PlayerTask(nbsFile, Context.Player, Context.Server.TotalTicks);
            }
            else
            {
                task = new PlayerTask(nbsFile, Context.Player, DateTime.Now);
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
                await Context.Player.SendMessageAsync($"{Constants.Prefix}Stopped playing");
            }
            catch
            {
                await Context.Player.SendMessageAsync($"{Constants.Prefix}You aren't playing a song right now.");
            }
        }

        [Command("mode")]
        [RequireOperator]
        public async Task SetModeAsync(string mode)
        {
            switch (mode.ToLowerInvariant())
            {
                default:
                    await Context.Player.SendMessageAsync("§c/nbspp mode [tick|timer]");
                    break;

                case "tick":
                case "ticks":
                    NbsPlayerPluginClass.Config.UseServerTicks = true;
                    await Context.Player.SendMessageAsync("§aTiming method set to server ticks");
                    break;

                case "timer":
                    NbsPlayerPluginClass.Config.UseServerTicks = false;
                    await Context.Player.SendMessageAsync("§aTiming method set to timers");
                    break;
            }
        }
    }
}