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

            if (!File.Exists(path))
            {
                await Context.Player.SendMessageAsync($"§cCan't play {song} as the file doesn't exists.");
                return;
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

        [Command("list")]
        [Description("Lists all .NBS files available to play on this server")]
        public async Task ListAsync()
        {
            await Context.Player.SendMessageAsync($"§2--- Showing available songs ---");

            foreach (string nbsPath in Directory.GetFiles(Path.Combine(Context.Server.Path, "songs"), "*.nbs"))
            {
                NbsFile nbsFile = NbsFileReader.ReadNbsFile(nbsPath);
                await Context.Player.SendMessageAsync($"§a{nbsFile.GetLabel()} ({nbsFile.FileName})");
            }

            await Context.Player.SendMessageAsync($"§2-------------------------------");
        }

        [Command("stop")]
        [Description("Stops the currently playing song")]
        public async Task StopAsync()
        {
            try
            {
                await NbsPlayerPluginClass.StopTaskAsync(Context.Player);
                await Context.Player.SendMessageAsync($"§aStopped playing");
            }
            catch
            {
                await Context.Player.SendMessageAsync($"§aYou aren't playing a song right now.");
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