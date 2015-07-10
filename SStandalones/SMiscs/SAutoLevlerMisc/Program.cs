using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SAssemblies;
using SAssemblies.Miscs;
using Menu = SAssemblies.Menu;
using System.Drawing;
using LeagueSharp.SDK.Core.UI;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Misc = new MenuItemSettings();
        public static MenuItemSettings AutoLevlerMisc = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoLevlerMisc, () => new AutoLevler() },
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
        public static MenuItemSettings Misc = new MenuItemSettings();
        public static MenuItemSettings AutoLevlerMisc = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoLevlerMisc, () => new AutoLevler() },
            };
        }
    }

    class Program
    {

        private static bool threadActive = true;
        private static float lastDebugTime = 0;
        private MainMenu mainMenu;
        private static readonly Program instance = new Program();

        public static void Main(string[] args)
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
            Common.ShowNotification("SAutoLevlerMisc loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                var menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Misc = Misc.SetupMenu(menu);
                ////mainMenu.UpdateDirEntry(ref MainMenu.AutoLevler, Miscs.AutoLevler.SetupMenu(MainMenu.Misc.Menu)); //Hängt bei linkslick

                Menu2.MenuItemSettings AutoLevlerMisc = new Menu2.MenuItemSettings(typeof(AutoLevler));

                Menu2.MenuItemSettings tempSettings = new Menu2.MenuItemSettings();
                AutoLevlerMisc.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesMiscsAutoLevler", Language.GetString("MISCS_AUTOLEVLER_MAIN")));
                Menu2.AddComponent(ref AutoLevlerMisc.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesTimersPingTimes", Language.GetString("GLOBAL_PING_TIMES"), 0, 0, 5));

                tempSettings.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesMiscsAutoLevlerPriority", Language.GetString("MISCS_AUTOLEVLER_PRIORITY_MAIN")));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesMiscsAutoLevlerPrioritySliderQ", "Q", 0, 0, 3));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesMiscsAutoLevlerPrioritySliderW", "W", 0, 0, 3));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesMiscsAutoLevlerPrioritySliderE", "E", 0, 0, 3));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesMiscsAutoLevlerPrioritySliderR", "R", 0, 0, 3));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesMiscsAutoLevlerPriorityFirstSpells", 
                    Language.GetString("MISCS_AUTOLEVLER_PRIORITY_MODE"), new[]
                    {
                        "Q W E", 
                        "Q E W", 
                        "W Q E", 
                        "W E Q", 
                        "E Q W", 
                        "E W Q"
                    }));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesMiscsAutoLevlerPriorityFirstSpellsActive", 
                    Language.GetString("MISCS_AUTOLEVLER_PRIORITY_MODE_ACTIVE")));
                tempSettings.CreateActiveMenuItem("SAssembliesMiscsAutoLevlerPriorityActive");

                tempSettings.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesMiscsAutoLevlerSequence", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_MAIN")));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesMiscsAutoLevlerSequenceLoadChoice", 
                    Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_BUILD_CHOICE"), new[] { "dummy" /*GetBuildNames()*/}));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesMiscsAutoLevlerSequenceShowBuild", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_BUILD_LOAD")));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesMiscsAutoLevlerSequenceNewBuild", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_CREATE_CHOICE")));
                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesMiscsAutoLevlerSequenceDeleteBuild", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_DELETE_CHOICE")));
                tempSettings.CreateActiveMenuItem("SAssembliesMiscsAutoLevlerSequenceActive");

                Menu2.AddComponent(ref tempSettings.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesMiscsAutoLevlerSMode", Language.GetString("GLOBAL_MODE"), new[]
                    {
                        Language.GetString("MISCS_AUTOLEVLER_MODE_PRIORITY"), 
                        Language.GetString("MISCS_AUTOLEVLER_MODE_SEQUENCE"), 
                        Language.GetString("MISCS_AUTOLEVLER_MODE_R")
                    }));
                AutoLevlerMisc.CreateActiveMenuItem("SAssembliesMiscsAutoLevlerActive");

                MainMenu2.AutoLevlerMisc = AutoLevlerMisc;
            }
            catch (Exception)
            {
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
