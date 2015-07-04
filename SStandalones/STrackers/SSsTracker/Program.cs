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
        public static MenuItemSettings SsCallerTracker = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { SsCallerTracker, () => new SsCaller() },
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
        public static MenuItemSettings SsCallerTracker = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { SsCallerTracker, () => new SsCaller() },
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
            Common.ShowNotification("SSsCallerTracker loaded!", Color.LawnGreen, 5000);

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
                //mainMenu.UpdateDirEntry(ref MainMenu.SsCallerTracker, SsCaller.SetupMenu(MainMenu.Tracker.Menu));

                Menu2.MenuItemSettings SsCallerTracker = new Menu2.MenuItemSettings(typeof(SsCaller));

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesTrackersSsCaller", Language.GetString("TRACKERS_SSCALLER_MAIN")));
                SsCallerTracker.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesTrackersSsCaller"];
                SsCallerTracker.Menu.Add(new MenuItem<MenuSlider>("SAssembliesTrackersSsCallerPingTimes", Language.GetString("GLOBAL_PING_TIMES")) { Value = new MenuSlider(0, 0, 5) });
                SsCallerTracker.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesTrackersSsCallerPingType", Language.GetString("GLOBAL_PING_TIMES"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("GLOBAL_PING_TYPE_NORMAL"), 
                    Language.GetString("GLOBAL_PING_TYPE_DANGER"), 
                    Language.GetString("GLOBAL_PING_TYPE_ENEMYMISSING"), 
                    Language.GetString("GLOBAL_PING_TYPE_ONMYWAY"), 
                    Language.GetString("GLOBAL_PING_TYPE_FALLBACK"), 
                    Language.GetString("GLOBAL_PING_ASSISTME") 
                })
                });
                SsCallerTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersSsCallerLocalPing", Language.GetString("GLOBAL_PING_LOCAL")) { Value = new MenuBool() });
                SsCallerTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersSsCallerChatChoice", Language.GetString("GLOBAL_CHAT")) { Value = new MenuBool() });
                SsCallerTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersSsCallerNotification", Language.GetString("GLOBAL_NOTIFICATION")) { Value = new MenuBool() });
                SsCallerTracker.Menu.Add(new MenuItem<MenuSlider>("SAssembliesTrackersSsCallerDisableTime", Language.GetString("TRACKERS_SSCALLER_DISABLETIME")) { Value = new MenuSlider(20, 1, 180) });
                SsCallerTracker.Menu.Add(new MenuItem<MenuSlider>("SAssembliesTrackersSsCallerCircleRange", Language.GetString("TRACKERS_SSCALLER_CIRCLE_RANGE")) { Value = new MenuSlider(2000, 100, 15000) });
                SsCallerTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersSsCallerCircleActive", Language.GetString("TRACKERS_SSCALLER_CIRCLE_ACTIVE")) { Value = new MenuBool() });
                SsCallerTracker.Menu.Add(new MenuItem<MenuBool>("SAssembliesTrackersSsCallerChatChoice", Language.GetString("GLOBAL_CHAT")) { Value = new MenuBool() });
                SsCallerTracker.CreateActiveMenuItem("SAssembliesTrackersSsCallerActive");

                MainMenu2.SsCallerTracker = SsCallerTracker;
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
