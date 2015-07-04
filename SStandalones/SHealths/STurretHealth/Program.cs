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
using LeagueSharp.SDK.Core.UI.Abstracts;
using LeagueSharp.SDK.Core.UI.Values;
using SAssemblies;
using SAssemblies.Healths;
using Menu = SAssemblies.Menu;
using MenuItem = LeagueSharp.SDK.Core.UI.MenuItem;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Health = new MenuItemSettings();
        public static MenuItemSettings TurretHealth = new MenuItemSettings();
        public static MenuItemSettings InhibitorHealth = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { TurretHealth, () => new Turret() },
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
        public static MenuItemSettings Health = new MenuItemSettings();
        public static MenuItemSettings TurretHealth = new MenuItemSettings();
        public static MenuItemSettings InhibitorHealth = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { TurretHealth, () => new Turret() },
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
            Common.ShowNotification("STurretHealth loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                LeagueSharp.SDK.Core.UI.Menu menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Health = Healths.Health.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.TurretHealth, Turret.SetupMenu(MainMenu.Health.Menu));

                Menu2.MenuItemSettings TurretHealth = new Menu2.MenuItemSettings();

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesHealthsTurret", Language.GetString("HEALTHS_TURRET_MAIN")));
                TurretHealth.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesHealthsTurret"];
                TurretHealth.CreateActiveMenuItem("SAssembliesHealthsTurretActive");

                MainMenu2.TurretHealth = TurretHealth;
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
