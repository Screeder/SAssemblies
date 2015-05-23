using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        public static MenuItemSettings Timers;
        public static MenuItemSettings JungleTimer;
        public static MenuItemSettings RelictTimer;
        public static MenuItemSettings HealthTimer;
        public static MenuItemSettings InhibitorTimer;
        public static MenuItemSettings SummonerTimer;
        public static MenuItemSettings ImmuneTimer;
        public static MenuItemSettings AltarTimer;
        public static MenuItemSettings SpellTimer;
    }

    class Program
    {

        private static bool threadActive = true;
        static void Main(string[] args)
        {
            AssemblyResolver.Init();
            AppDomain.CurrentDomain.DomainUnload += delegate { threadActive = false; };
            AppDomain.CurrentDomain.ProcessExit += delegate { threadActive = false; };
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private async static void Game_OnGameLoad(EventArgs args)
        {
            CreateMenu();
            Game.PrintChat("STimers loaded!");
            new Thread(GameOnOnGameUpdate).Start();
        }

        private static void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("STimers", "STimers", true);

                MainMenu.Timers = Timers.Timer.SetupMenu(menu);
                MainMenu.AltarTimer = Timers.Altar.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.HealthTimer = Timers.Health.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.ImmuneTimer = Timers.Immune.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.InhibitorTimer = Timers.Inhibitor.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.JungleTimer = Timers.Jungle.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.RelictTimer = Timers.Relic.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.SummonerTimer = Timers.Summoner.SetupMenu(MainMenu.Timers.Menu);
                MainMenu.SpellTimer = Timers.Spell.SetupMenu(MainMenu.Timers.Menu);

                Menu.GlobalSettings.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("Global Settings", "SAssembliesGlobalSettings"));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsServerChatPingActive", "Server Chat/Ping").SetValue(false)));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsVoiceVolume", "Voice Volume").SetValue(new Slider(100, 0, 100))));

                menu.AddItem(new MenuItem("By Screeder", "By Screeder V0.8.0.4"));
                menu.AddToMainMenu();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GameOnOnGameUpdate(/*EventArgs args*/)
        {
            try
            {
                while (threadActive)
                {
                    Thread.Sleep(100);
                    Type classType = typeof(MainMenu);
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
                    FieldInfo[] fields = classType.GetFields(flags);
                    foreach (FieldInfo p in fields.ToList())
                    {
                        try
                        {
                            var item = (Menu.MenuItemSettings)p.GetValue(null);
                            if (item == null)
                            {
                                continue;
                            }
                            if (item.GetActive() == false && item.Item != null)
                            {
                                item.Item = null;
                                //GC.Collect();
                            }
                            else if (item.GetActive() && item.Item == null && !item.ForceDisable && item.Type != null)
                            {
                                try
                                {
                                    item.Item = System.Activator.CreateInstance(item.Type);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    threadActive = false;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("SAssemblies: " + e + "\n" + p.ToString());
                            threadActive = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SAssemblies: " + e);
                threadActive = false;
            }
        }
    }
}
