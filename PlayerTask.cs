using Obsidian;
using Obsidian.Boss;
using Obsidian.Chat;
using Obsidian.Entities;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace NbsPlayerPlugin
{
    public class PlayerTask
    {
        public PlayerTask(NbsFile nbs, Player player, int tickStart)
        {
            this.NBS = nbs ?? throw new ArgumentNullException(nameof(nbs));
            this.Player = player ?? throw new ArgumentNullException(nameof(player));
            this.TickStart = tickStart;

            InitializeBossBar();
        }

        public PlayerTask(NbsFile nbs, Player player, DateTime startTime)
        {
            this.NBS = nbs ?? throw new ArgumentNullException(nameof(nbs));
            this.Player = player ?? throw new ArgumentNullException(nameof(player));
            this.StartTime = startTime;

            InitializeBossBar();
        }

        public async Task InitializeBossBar()
        {
            await this.Player.SendBossBarAsync(NbsPlayerPluginClass.BossBarId, new BossBarAddAction()
            {
                Color = BossBarColor.Green,
                Title = ChatMessage.Simple("§a" + this.NBS.GetLabel()),
                Health = 0f,
                Division = BossBarDivisionType.None,
                Flags = BossBarFlags.None
            });
        }

        /// <summary>
        /// Countdown until next Boss Bar update
        /// </summary>
        public int BossBarUpdate { get; set; }

        public Player Player { get; }
        public NbsFile NBS { get; }
        public int Tick { get; set; } = 0;
        public int LastTick { get; set; } = 0;
        public int TickStart { get; }
        public DateTime StartTime { get; }

        public Timer Timer { get; set; }
    }
}