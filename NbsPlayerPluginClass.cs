using Obsidian;
using Obsidian.Boss;
using Obsidian.Entities;
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

        public static Timer Timer;

        public static List<PlayerTask> Tasks = new List<PlayerTask>();

        public static async Task StopTaskAsync(string username)
        {
            PlayerTask task = Tasks.First(t => t.Player.Username == username);

            try
            {
                await RemoveBossBarAsync(task);
            }
            catch
            {
            }

            Tasks.Remove(task);
        }

        public async Task<PluginInfo> InitializeAsync(Server server)
        {
            this.server = server;

            server.Commands.AddModule<NbsPlayerCommands>();

            Config = server.LoadConfig<NbsPlayerConfig>(this);

            if (Config == null)
            {
                Config = new NbsPlayerConfig();
            }

            if (Config.UseServerTicks)
            {
                server.Events.ServerTick += async () => await StepAsync();
            }
            else
            {
                Timer = new Timer(10);
                Timer.Elapsed += async (s, e) => await StepAsync();
                Timer.Start();
            }

            return new PluginInfo(
                "NBS Player Plugin",
                "Craftplacer (Obsidian Team)",
                "0.1",
                "Plays back .NBS files stored on this server",
                "https://github.com/Craftplacer/NbsPlayerPlugin"
            );
        }

        public void SaveConfig() => server.SaveConfig(this, Config);

        public async Task StepAsync()
        {
            //Credit: https://stackoverflow.com/questions/17767161/possible-to-modify-a-list-while-iterating-through-it
            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                PlayerTask task = Tasks[i];

                try
                {
                    task.LastTick = task.Tick;

                    float secondsPassed;

                    if (Config.UseServerTicks)
                    {
                        secondsPassed = ((float)this.server.TotalTicks - (float)task.TickStart) / 20;
                    }
                    else
                    {
                        TimeSpan timeSpan = DateTime.Now - task.StartTime;
                        secondsPassed = (float)timeSpan.TotalSeconds;
                    }

                    float ticksPassed = secondsPassed * task.NBS.Tempo;
                    task.Tick = (int)ticksPassed;

                    if (task.Tick == task.LastTick)
                    {
                        continue;
                    }

                    var noteBlocks = new List<NoteBlock>();

                    foreach (NbsLayer layer in task.NBS.Layers)
                    {
                        noteBlocks.AddRange(layer.NoteBlocks.FindAll(nb => nb.Tick > task.LastTick && nb.Tick <= task.Tick));
                    }

                    Position position = task.Player.Transform.Position;
                    foreach (NoteBlock noteBlock in noteBlocks)
                    {
                        float pitch = Constants.PitchValues[noteBlock.Key - 33];
                        float volume = task.NBS.Layers[noteBlock.Layer].Volume / 100;
                        string instrument = Constants.InstrumentValues[noteBlock.Instrument];

                        //var playerPosition = new Position((int)position.X, (int)position.Y, (int)position.Z);

                        await task.Player.SendNamedSoundAsync(instrument, position, category: SoundCategory.Records, volume: volume, pitch: pitch);
                    }

                    await UpdateBossBarAsync(task);
                }
                catch
                {
                    await StopTaskAsync(task.Player.Username);
                }
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

        public static async Task RemoveBossBarAsync(PlayerTask task) => await task.Player.SendBossBarAsync(BossBarId, new BossBarRemoveAction());
    }
}