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
        public static MenuItemSettings Tracker;
        public static MenuItemSettings UiTracker;
        public static MenuItemSettings UimTracker;
        public static MenuItemSettings SsCallerTracker;
        public static MenuItemSettings WaypointTracker;
        public static MenuItemSettings CloneTracker;
        public static MenuItemSettings GankTracker;
        public static MenuItemSettings DestinationTracker;
        public static MenuItemSettings KillableTracker;
        public static MenuItemSettings JunglerTracker;
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
            Game.PrintChat("STrackers loaded!");
            new Thread(GameOnOnGameUpdate).Start();
        }

        private static void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("STrackers", "STrackers", true);

                MainMenu.Tracker = Trackers.Tracker.SetupMenu(menu);
                MainMenu.GankTracker = Trackers.Gank.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.CloneTracker = Trackers.Clone.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.DestinationTracker = Trackers.Destination.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.KillableTracker = Trackers.Killable.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.SsCallerTracker = Trackers.SsCaller.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.UiTracker = Trackers.Ui.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.UimTracker = Trackers.Uim.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.WaypointTracker = Trackers.Waypoint.SetupMenu(MainMenu.Tracker.Menu);
                MainMenu.JunglerTracker = Trackers.Jungler.SetupMenu(MainMenu.Tracker.Menu);
                //MainMenu.CrowdControlTracker = Trackers.CrowdControl.SetupMenu(MainMenu.Tracker.Menu);

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
