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
using LeagueSharp.SDK.Core.UI.Values;

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
                LeagueSharp.SDK.Core.UI.Menu menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Misc = Misc.SetupMenu(menu);
                ////mainMenu.UpdateDirEntry(ref MainMenu.AutoLevler, Miscs.AutoLevler.SetupMenu(MainMenu.Misc.Menu)); //Hängt bei linkslick

                Menu2.MenuItemSettings AutoLevlerMisc = new Menu2.MenuItemSettings(typeof(AutoLevler));

                Menu2.MenuItemSettings tempSettings = new Menu2.MenuItemSettings();
                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesMiscsAutoLevler", Language.GetString("MISCS_AUTOLEVLER_MAIN")));
                AutoLevlerMisc.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesMiscsAutoLevler"];

                AutoLevlerMisc.Menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesMiscsAutoLevlerPriority", Language.GetString("MISCS_AUTOLEVLER_PRIORITY_MAIN")));
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]).Add(
                    new MenuItem<MenuSlider>("SAssembliesMiscsAutoLevlerPrioritySliderQ", "Q") { Value = new MenuSlider(0, 0, 3) });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]).Add(
                    new MenuItem<MenuSlider>("SAssembliesMiscsAutoLevlerPrioritySliderW", "W") { Value = new MenuSlider(0, 0, 3) });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]).Add(
                    new MenuItem<MenuSlider>("SAssembliesMiscsAutoLevlerPrioritySliderE", "E") { Value = new MenuSlider(0, 0, 3) });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]).Add(
                    new MenuItem<MenuSlider>("SAssembliesMiscsAutoLevlerPrioritySliderR", "R") { Value = new MenuSlider(0, 0, 3) });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]).Add(
                    new MenuItem<MenuList<String>>("SAssembliesMiscsAutoLevlerPriorityFirstSpells", Language.GetString("MISCS_AUTOLEVLER_PRIORITY_MODE")) { Value = new MenuList<string>(new[]
                    {
                        "Q W E", 
                        "Q E W", 
                        "W Q E", 
                        "W E Q", 
                        "E Q W", 
                        "E W Q"
                    })});
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]).Add(
                    new MenuItem<MenuBool>("SAssembliesMiscsAutoLevlerPriorityFirstSpellsActive", Language.GetString("MISCS_AUTOLEVLER_PRIORITY_MODE_ACTIVE")) { Value = new MenuBool() });
                tempSettings.Menu = ((LeagueSharp.SDK.Core.UI.Menu) AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerPriority"]);
                tempSettings.CreateActiveMenuItem("SAssembliesMiscsAutoLevlerPriorityActive");

                AutoLevlerMisc.Menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesMiscsAutoLevlerSequence", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_MAIN")));
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerSequence"]).Add(
                    new MenuItem<MenuList<String>>("SAssembliesMiscsAutoLevlerSequenceLoadChoice", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_BUILD_CHOICE")) { Value = new MenuList<String>(new[]{"dummy"}) /*GetBuildNames()*/ });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerSequence"]).Add(
                    new MenuItem<MenuBool>("SAssembliesMiscsAutoLevlerSequenceShowBuild", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_BUILD_LOAD")) { Value = new MenuBool() });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerSequence"]).Add(
                    new MenuItem<MenuBool>("SAssembliesMiscsAutoLevlerSequenceNewBuild", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_CREATE_CHOICE")) { Value = new MenuBool() });
                ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerSequence"]).Add(
                    new MenuItem<MenuBool>("SAssembliesMiscsAutoLevlerSequenceDeleteBuild", Language.GetString("MISCS_AUTOLEVLER_SEQUENCE_DELETE_CHOICE")) { Value = new MenuBool() });
                tempSettings.Menu = ((LeagueSharp.SDK.Core.UI.Menu)AutoLevlerMisc.Menu["SAssembliesMiscsAutoLevlerSequence"]);
                tempSettings.CreateActiveMenuItem("SAssembliesMiscsAutoLevlerSequenceActive");

                AutoLevlerMisc.Menu.Add(
                    new MenuItem<MenuList<String>>("SAssembliesMiscsAutoLevlerSMode", Language.GetString("GLOBAL_MODE")) { Value = new MenuList<string>(new[]
                    {
                        Language.GetString("MISCS_AUTOLEVLER_MODE_PRIORITY"), 
                        Language.GetString("MISCS_AUTOLEVLER_MODE_SEQUENCE"), 
                        Language.GetString("MISCS_AUTOLEVLER_MODE_R")
                    })});
                AutoLevlerMisc.CreateActiveMenuItem("SAssembliesMiscsAutoLevlerActive");

                MainMenu2.AutoLevlerMisc = AutoLevlerMisc;
            }
            catch (Exception e)
            {
                Console.WriteLine( e);
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
