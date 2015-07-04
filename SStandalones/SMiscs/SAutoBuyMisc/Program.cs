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
        public static MenuItemSettings AutoBuyMisc = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoBuyMisc, () => new AutoBuy() },
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
        public static MenuItemSettings AutoBuyMisc = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoBuyMisc, () => new AutoBuy() },
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
            Common.ShowNotification("SAutoBuyMisc loaded!", Color.LawnGreen, 5000);

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
                ////mainMenu.UpdateDirEntry(ref MainMenu.AutoBuy, Miscs.AutoBuy.SetupMenu(MainMenu.Misc.Menu));

                Menu2.MenuItemSettings AutoBuyMisc = new Menu2.MenuItemSettings(typeof(AutoBuy));

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesMiscsAutoBuy", Language.GetString("MISCS_AUTOBUY_MAIN")));
                AutoBuyMisc.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesMiscsAutoBuy"];
                AutoBuyMisc.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesMiscsAutoBuyLoadChoice", Language.GetString("MISCS_AUTOBUY_BUILD_CHOICE")) { Value = new MenuList<String>(new[] { "dummy" }) /*GetBuildNames()*/ });
                AutoBuyMisc.Menu.Add(new MenuItem<MenuBool>("SAssembliesMiscsAutoBuyShowBuild", Language.GetString("MISCS_AUTOBUY_BUILD_LOAD")) { Value = new MenuBool() });
                AutoBuyMisc.Menu.Add(new MenuItem<MenuBool>("SAssembliesMiscsAutoBuyNewBuild", Language.GetString("MISCS_AUTOBUY_CREATE_BUILD")) { Value = new MenuBool() });
                AutoBuyMisc.Menu.Add(new MenuItem<MenuBool>("SAssembliesMiscsAutoBuyDeleteBuild", Language.GetString("MISCS_AUTOBUY_DELETE_BUILD")) { Value = new MenuBool() });
                AutoBuyMisc.CreateActiveMenuItem("SAssembliesMiscsAutoBuyActive");

                MainMenu2.AutoBuyMisc = AutoBuyMisc;
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
