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
using SAssemblies.Trackers;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Tracker = new MenuItemSettings();
        public static MenuItemSettings UiTracker = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { UiTracker, () => new Ui() },
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
        public static MenuItemSettings UiTracker = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { UiTracker, () => new Ui() },
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
            Common.ShowNotification("SUiTracker loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                var menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Tracker = Tracker.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.UiTracker, Ui.SetupMenu(MainMenu.Tracker.Menu));

                Menu2.MenuItemSettings UiTracker = new Menu2.MenuItemSettings(typeof(Ui));

                Menu2.MenuItemSettings tempSettings = new Menu2.MenuItemSettings();
                UiTracker.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesTrackersUi", Language.GetString("TRACKERS_UI_MAIN")));
                //Menu2.AddComponent(ref UiTracker.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesItemPanelActive", Language.GetString("TRACKERS_UI_ITEMPANEL")));
                Menu2.AddComponent(ref UiTracker.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesUITrackerScale", Language.GetString("TRACKERS_UI_SCALE"), 100));

                tempSettings.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesUITrackerEnemyTracker", Language.GetString("TRACKERS_UI_ENEMY")));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesUITrackerEnemyTrackerXPos", 
                    Language.GetString("TRACKERS_UI_GLOBAL_POSITION_X"), (int)_screen.X, 0, Drawing.Width));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesUITrackerEnemyTrackerYPos", 
                    Language.GetString("TRACKERS_UI_GLOBAL_POSITION_Y"), (int)_screen.Y, 0, Drawing.Height));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerEnemyTrackerMode",
                    Language.GetString("GLOBAL_MODE"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_BOTH"),  
                }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerEnemyTrackerSideDisplayMode",
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE_DISPLAY"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_LEAGUE"),  
                }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerEnemyTrackerHeadMode",
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_MODE"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_SMALL"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_BIG"), 
                }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerEnemyTrackerHeadDisplayMode",
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_DISPLAY"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                }));
                tempSettings.CreateActiveMenuItem("SAssembliesUITrackerEnemyTrackerActive");

                tempSettings.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesUITrackerAllyTracker", Language.GetString("TRACKERS_UI_ALLY")));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesUITrackerAllyTrackerXPos",
                    Language.GetString("TRACKERS_UI_GLOBAL_POSITION_X"), (int)_screen.X, 0, Drawing.Width));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesUITrackerAllyTrackerYPos",
                    Language.GetString("TRACKERS_UI_GLOBAL_POSITION_Y"), (int)_screen.Y, 0, Drawing.Height));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerAllyTrackerMode",
                    Language.GetString("GLOBAL_MODE"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_BOTH"),  
                }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerAllyTrackerSideDisplayMode",
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE_DISPLAY"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_LEAGUE"),  
                }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerAllyTrackerHeadMode",
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_MODE"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_SMALL"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_BIG"), 
                }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesUITrackerAllyTrackerHeadDisplayMode",
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_DISPLAY"), new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                }));
                tempSettings.CreateActiveMenuItem("SAssembliesUITrackerAllyTrackerActive");

                Menu2.AddComponent(ref UiTracker.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesUITrackerPingActive", Language.GetString("TRACKERS_UI_PING")));
                UiTracker.CreateActiveMenuItem("SAssembliesTrackersUiActive");

                MainMenu2.UiTracker = UiTracker;
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
