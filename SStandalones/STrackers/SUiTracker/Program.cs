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
                LeagueSharp.SDK.Core.UI.Menu menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Tracker = Tracker.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.UiTracker, Ui.SetupMenu(MainMenu.Tracker.Menu));

                Menu2.MenuItemSettings UiTracker = new Menu2.MenuItemSettings(typeof(Ui));

                Menu2.MenuItemSettings tempSettings = new Menu2.MenuItemSettings();
                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesTrackersUi", Language.GetString("TRACKERS_UI_MAIN")));
                UiTracker.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesTrackersUi"];
                //UiTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesItemPanelActive", Language.GetString("TRACKERS_UI_ITEMPANEL")) { Value = new MenuBool() });
                UiTracker.Menu.Add(new MenuItem<MenuSlider>("SAssembliesUITrackerScale", Language.GetString("TRACKERS_UI_SCALE")) { Value = new MenuSlider(100, 0, 100) });

                UiTracker.Menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesUITrackerEnemyTracker", Language.GetString("TRACKERS_UI_ENEMY")));
                tempSettings.Menu = ((LeagueSharp.SDK.Core.UI.Menu)UiTracker.Menu["SAssembliesUITrackerEnemyTracker"]);
                tempSettings.Menu.Add(new MenuItem<MenuSlider>("SAssembliesUITrackerEnemyTrackerXPos", Language.GetString("TRACKERS_UI_GLOBAL_POSITION_X")) { Value = new MenuSlider((int)_screen.X, 0, Drawing.Width) });
                tempSettings.Menu.Add(new MenuItem<MenuSlider>("SAssembliesUITrackerEnemyTrackerYPos", Language.GetString("TRACKERS_UI_GLOBAL_POSITION_Y")) { Value = new MenuSlider((int)_screen.Y, 0, Drawing.Height) });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerEnemyTrackerMode", Language.GetString("GLOBAL_MODE"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_BOTH"),  
                })
                });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerEnemyTrackerSideDisplayMode", Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE_DISPLAY"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_LEAGUE"),  
                })
                });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerEnemyTrackerHeadMode", Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_MODE"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_SMALL"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_BIG"), 
                })
                });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerEnemyTrackerHeadDisplayMode", Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_DISPLAY"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                })
                });
                tempSettings.CreateActiveMenuItem("SAssembliesUITrackerEnemyTrackerActive");

                UiTracker.Menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesUITrackerAllyTracker", Language.GetString("TRACKERS_UI_ALLY")));
                tempSettings.Menu = ((LeagueSharp.SDK.Core.UI.Menu)UiTracker.Menu["SAssembliesUITrackerAllyTracker"]);
                tempSettings.Menu.Add(new MenuItem<MenuSlider>("SAssembliesUITrackerAllyTrackerXPos", Language.GetString("TRACKERS_UI_GLOBAL_POSITION_X")) { Value = new MenuSlider((int)_screen.X, 0, Drawing.Width) });
                tempSettings.Menu.Add(new MenuItem<MenuSlider>("SAssembliesUITrackerAllyTrackerYPos", Language.GetString("TRACKERS_UI_GLOBAL_POSITION_Y")) { Value = new MenuSlider((int)_screen.Y, 0, Drawing.Height) });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerAllyTrackerMode", Language.GetString("GLOBAL_MODE"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_BOTH"),  
                })
                });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerAllyTrackerSideDisplayMode", Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDE_DISPLAY"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_LEAGUE"),  
                })
                });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerAllyTrackerHeadMode", Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_MODE"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_SMALL"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_HEAD_BIG"), 
                })
                });
                tempSettings.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesUITrackerAllyTrackerHeadDisplayMode", Language.GetString("TRACKERS_UI_GLOBAL_MODE_UNIT_DISPLAY"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_DEFAULT"), 
                    Language.GetString("TRACKERS_UI_GLOBAL_MODE_SIDEHEAD_SIMPLE"), 
                })
                });
                tempSettings.CreateActiveMenuItem("SAssembliesUITrackerAllyTrackerActive");

                UiTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesUITrackerPingActive", Language.GetString("TRACKERS_UI_PING")) { Value = new MenuBool() });
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
