using Obsidian;
using Obsidian.Boss;
using Obsidian.Chat;
using Obsidian.Entities;
using Obsidian.Events.EventArgs;
using Obsidian.Plugins;
using Obsidian.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace NbsPlayerPlugin
{
    public class NbsPlayerPluginClass : IPluginClass
    {
        public static NbsPlayerConfig Config;

        public static Guid BossBarId { get; } = Guid.NewGuid();

        private Server server;

        public static List<PlayerTask> Tasks = new List<PlayerTask>();

        public async Task<PluginInfo> InitializeAsync(Server server)
        {
            this.server = server;

            server.Commands.AddModule<NbsPlayerCommands>();
            server.Events.PlayerLeave += this.Events_PlayerLeave;

            Config = server.LoadConfig<NbsPlayerConfig>(this);

            if (Config == null)
            {
                Config = new NbsPlayerConfig();
            }

            if (Config.UseServerTicks)
            {
                server.Events.ServerTick += async () => await StepAsync();
            }

            return new PluginInfo(
                "NBS Player Plugin",
                "Craftplacer (Obsidian Team)",
                "0.1",
                "Plays back .NBS files stored on this server",
                "https://github.com/Craftplacer/NbsPlayerPlugin"
            );
        }

        public static async Task StopTaskAsync(Player player)
        {
            PlayerTask task = Tasks.FirstOrDefault(t => t.Player == player);

            if (task == null)
            {
                //server.Logger.LogWarning("");
                return;
            }

            if (player.Connected)
            {
                await RemoveBossBarAsync(player);
            }

            Tasks.Remove(task);
        }

        private async Task Events_PlayerLeave(PlayerLeaveEventArgs e) => await StopTaskAsync(e.WhoLeft);

        public void SaveConfig() => server.SaveConfig(this, Config);

        public async Task StepAsync()
        {
            //Credit: https://stackoverflow.com/questions/17767161/possible-to-modify-a-list-while-iterating-through-it
            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                PlayerTask task = Tasks[i];

                if (task == null)
                {
                    continue;
                }

                if (task.Timer != null)
                {
                    continue;
                }

                await StepAsync(task);
            }
        }

        public async Task StepAsync(PlayerTask task)
        {
            int GetTick(PlayerTask t)
            {
                float secondsPassed;

                if (Config.UseServerTicks)
                {
                    secondsPassed = (server.TotalTicks - (float)t.TickStart) / 20f;
                }
                else
                {
                    TimeSpan timeSpan = DateTime.Now - t.StartTime;
                    secondsPassed = (float)timeSpan.TotalSeconds;
                }

                float ticksPassed = secondsPassed * t.NBS.Tempo;
                return (int)ticksPassed;
            }

            if (!Config.UseServerTicks && task.Timer == null)
            {
                task.Timer = new Timer(1000 / task.NBS.Tempo);
                task.Timer.Elapsed += async (_, __) => await StepAsync(task);
                task.Timer.Start();
                return;
            }

            if (!task.Player.Connected)
            {
                await StopTaskAsync(task.Player);
                return;
            }

            try
            {
                task.LastTick = task.Tick;
                task.Tick = GetTick(task);

                //if (task.Tick >= task.NBS.Length)
                //{
                //    await StopTaskAsync(task.Player);
                //    continue;
                //}

                if (task.Tick == task.LastTick)
                {
                    return;
                }

                var noteBlocks = new List<NoteBlock>();
                foreach (List<NoteBlock> noteBlocks1 in task.NBS.Layers.Select(l => l.NoteBlocks))
                {
                    noteBlocks.AddRange(noteBlocks1.FindAll(nb => nb.Tick > task.LastTick && nb.Tick <= task.Tick));
                }

                string msg = "";

                //if (noteBlocks.Count == 0)
                //{
                //    msg += $"§0---";
                //}

                Position position = task.Player.Transform.Position;
                foreach (NoteBlock noteBlock in noteBlocks)
                {
                    float volume = task.NBS.Layers[noteBlock.Layer].Volume / 100;
                    float pitch = Constants.PitchValues[noteBlock.Key - 33];
                    string instrument = Constants.InstrumentValues[noteBlock.Instrument];

                    await task.Player.SendNamedSoundAsync(instrument, position, category: SoundCategory.Records, volume: volume, pitch: pitch);

                    msg += $"§{Constants.ColorValues[noteBlock.Instrument]}{Constants.NoteValues[noteBlock.Key - 33]} ";
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    await task.Player.SendMessageAsync(msg, 2);
                }

                await UpdateBossBarAsync(task);
            }
            catch
            {
                await StopTaskAsync(task.Player);
            }
        }

        public static async Task UpdateBossBarAsync(PlayerTask task)
        {
            if (task.BossBarUpdate <= 0)
            {
                await task.Player.SendBossBarAsync(BossBarId, new BossBarUpdateHealthAction()
                {
                    Health = (float)((float)task.Tick / (float)task.NBS.Length)
                });

                task.BossBarUpdate = 10;
            }
            else
            {
                task.BossBarUpdate--;
            }
        }

        public static async Task RemoveBossBarAsync(Player player) => await player.SendBossBarAsync(BossBarId, new BossBarRemoveAction());
    }
}