using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoFF
{
    internal class Program
    {
        static Menu Menu;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            (Menu = new Menu("Auto FF", "Auto FF", true)).AddToMainMenu();
            Menu.AddItem(new MenuItem("FF20", "Surrender at 20").SetValue(false));
            Menu.AddItem(new MenuItem("AFF", "Auto Accept Surrender").SetValue(false));
            Menu.AddItem(new MenuItem("KFF", "Surrender at kills behind").SetValue(false));
            Menu.AddItem(new MenuItem("KNFF", "Number of kills behind").SetValue(new Slider(10, 50, 5)));
            
            Game.PrintChat("Auto FF by Emunator loaded.");
            
            Game.OnUpdate += OnGameUpdate;

        }

        static void OnGameUpdate(EventArgs arg)
        {
            if (Menu.Item("FF20").IsActive() && LeagueSharp.Game.ClockTime > 1200)
            {
                LeagueSharp.Game.Say("/ff");
            }

            if (Loosing() && LeagueSharp.Game.ClockTime > 1200)
            {
                LeagueSharp.Game.Say("/ff");
            }
        }

        private void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            if (string.Equals(args.EventId.ToString(), "OnSurrenderVote") || args.EventId == GameEventId.OnSurrenderVote)
            {
                if (Menu.Item("AFF").IsActive())
                {
                    LeagueSharp.Game.Say("/ff");
                }

                else
                {
                    LeagueSharp.Game.Say("/noff");
                }
            }
        }

        private static bool Loosing()
        {
            var enemyStats = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy).Sum(enemy => enemy.ChampionsKilled);
            var allyStats = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly).Sum(ally => ally.ChampionsKilled);

            return enemyStats - Menu.Item("KNFF").GetValue<Slider>().Value > allyStats;
        }
    }
}