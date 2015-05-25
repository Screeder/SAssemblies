using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SAssemblies.Trackers;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Tracker = new MenuItemSettings();
        public static MenuItemSettings UiTracker = new MenuItemSettings();
        public static MenuItemSettings UimTracker = new MenuItemSettings();
        public static MenuItemSettings SsCallerTracker = new MenuItemSettings();
        public static MenuItemSettings WaypointTracker = new MenuItemSettings();
        public static MenuItemSettings CloneTracker = new MenuItemSettings();
        public static MenuItemSettings GankTracker = new MenuItemSettings();
        public static MenuItemSettings DestinationTracker = new MenuItemSettings();
        public static MenuItemSettings KillableTracker = new MenuItemSettings();
        public static MenuItemSettings JunglerTracker = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { UiTracker, () => new Ui() },
                { UimTracker, () => new Uim() },
                { SsCallerTracker, () => new SsCaller() },
                { WaypointTracker, () => new Waypoint() },
                { CloneTracker, () => new Clone() },
                { GankTracker, () => new Gank() },
                { DestinationTracker, () => new Destination() },
                { KillableTracker, () => new Killable() },
                { JunglerTracker, () => new Jungler() },
            };
        }

        public Tuple<MenuItemSettings, Func<dynamic>> GetDirEntry(MenuItemSettings menuItem)
        {
            return new Tuple<MenuItemSettings, Func<dynamic>>(menuItem, MenuEntries[menuItem]);
        }

        public Dictionary<MenuItemSettings, Func<dynamic>> GetDirEntries()
        {
            return MenuEntries;
        }

        public void UpdateDirEntry(ref MenuItemSettings oldMenuItem, MenuItemSettings newMenuItem)
        {
            Func<dynamic> save = MenuEntries[oldMenuItem];
            MenuEntries.Remove(oldMenuItem);
            MenuEntries.Add(newMenuItem, save);
            oldMenuItem = newMenuItem;
        }
    }

    class Program
    {

        private static bool threadActive = true;
        private MainMenu mainMenu;
        private static readonly Program instance = new Program();
        static void Main(string[] args)
        {
            AssemblyResolver.Init();
            AppDomain.CurrentDomain.DomainUnload += delegate { threadActive = false; };
            AppDomain.CurrentDomain.ProcessExit += delegate { threadActive = false; };
            Instance().Load();
        }

        public void Load()
        {
            mainMenu = new MainMenu();
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static Program Instance()
        {
            return instance;
        }

        private async void Game_OnGameLoad(EventArgs args)
        {
            CreateMenu();
            Common.ShowNotification("STrackers loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("STrackers", "STrackers", true);

                MainMenu.Tracker = Tracker.SetupMenu(menu);
                mainMenu.UpdateDirEntry(ref MainMenu.GankTracker, Gank.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.CloneTracker, Clone.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.DestinationTracker, Destination.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.KillableTracker, Killable.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.SsCallerTracker, SsCaller.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.UiTracker, Ui.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.UimTracker, Uim.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.WaypointTracker, Waypoint.SetupMenu(MainMenu.Tracker.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.JunglerTracker, Jungler.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.CrowdControlTracker, Trackers.CrowdControl.SetupMenu(MainMenu.Tracker.Menu));

                Menu.GlobalSettings.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("Global Settings", "SAssembliesGlobalSettings"));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsServerChatPingActive", "Server Chat/Ping").SetValue(false)));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsVoiceVolume", "Voice Volume").SetValue(new Slider(100, 0, 100))));

                menu.AddItem(new MenuItem("By Screeder", "By Screeder V" + Assembly.GetExecutingAssembly().GetName().Version));
                menu.AddToMainMenu();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GameOnOnGameUpdate(/*EventArgs args*/)
        {
            try
            {
                while (threadActive)
                {
                    Thread.Sleep(1000);

                    if (mainMenu == null)
                        continue;

                    foreach (var entry in mainMenu.GetDirEntries())
                    {
                        var item = entry.Key;
                        if (item == null)
                        {
                            continue;
                        }
                        try
                        {
                            if (item.GetActive() == false && item.Item != null)
                            {
                                item.Item = null;
                            }
                            else if (item.GetActive() && item.Item == null && !item.ForceDisable && item.Type != null)
                            {
                                try
                                {
                                    item.Item = entry.Value();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SAwareness: " + e);
                threadActive = false;
            }
        }
    }
}
