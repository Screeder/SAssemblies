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
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.UI.Values;
using SAssemblies.Trackers;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Tracker = new MenuItemSettings();
        public static MenuItemSettings GankTracker = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { GankTracker, () => new Gank() },
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

    class MainMenu2 : Menu2
    {
        public static MenuItemSettings Tracker = new MenuItemSettings();
        public static MenuItemSettings GankTracker = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { GankTracker, () => new Gank() },
            };
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
            LeagueSharp.SDK.Core.Events.Load.OnLoad += Game_OnGameLoad;
        }

        public static Program Instance()
        {
            return instance;
        }

        private async void Game_OnGameLoad(Object obj, EventArgs args)
        {
            CreateMenu();
            Common.ShowNotification("SGankTracker loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                LeagueSharp.SDK.Core.UI.Menu menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Tracker = Tracker.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.GankTracker, Gank.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.CloneTracker, Clone.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.DestinationTracker, Destination.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.KillableTracker, Killable.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SsCallerTracker, SsCaller.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.UiTracker, Ui.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.UimTracker, Uim.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.WaypointTracker, Waypoint.SetupMenu(MainMenu.Tracker.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.JunglerTracker, Jungler.SetupMenu(MainMenu.Tracker.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.CrowdControlTracker, Trackers.CrowdControl.SetupMenu(MainMenu.Tracker.Menu));

                Menu2.MenuItemSettings GankTracker = new Menu2.MenuItemSettings(typeof(Gank));

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesTrackersGank", Language.GetString("TRACKERS_GANK_MAIN")));
                GankTracker.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesTrackersGank"];
                GankTracker.Menu.Add(new MenuItem<MenuSlider>("SAssembliesTrackersGankTrackRange", Language.GetString("TRACKERS_GANK_RANGE")) { Value = new MenuSlider(1, 1, 20000) });
                GankTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersGankKillable", Language.GetString("TRACKERS_GANK_KILLABLE")) { Value = new MenuBool() });
                GankTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersGankDraw", Language.GetString("TRACKERS_GANK_LINES")) { Value = new MenuBool() });
                GankTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersGankPing", Language.GetString("TRACKERS_GANK_PING")) { Value = new MenuBool() });
                GankTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersGankVoice", Language.GetString("GLOBAL_VOICE")) { Value = new MenuBool() });
                GankTracker.CreateActiveMenuItem("SAssembliesTrackersGankActive");

                MainMenu2.GankTracker = GankTracker;
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
