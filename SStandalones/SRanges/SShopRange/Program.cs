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
using SAssemblies.Ranges;
using Menu = SAssemblies.Menu;
using System.Drawing;
using LeagueSharp.SDK.Core.Events;
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.UI.Values;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Range = new MenuItemSettings();
        public static MenuItemSettings ShopRange = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { ShopRange, () => new Shop() },
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
        public static MenuItemSettings Range = new MenuItemSettings();
        public static MenuItemSettings ShopRange = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { ShopRange, () => new Shop() },
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
            Common.ShowNotification("SShopRange loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                LeagueSharp.SDK.Core.UI.Menu menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Range = Range.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.ShopRange, Shop.SetupMenu(MainMenu.Range.Menu));

                Menu2.MenuItemSettings ShopRange = new Menu2.MenuItemSettings(typeof(Shop));

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesRangesShop", Language.GetString("RANGES_SHOP_MAIN")));
                ShopRange.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesRangesShop"];
                ShopRange.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesRangesShopMode", Language.GetString("RANGES_ALL_MODE"))
                {
                    Value = new MenuList<string>(new[]
                {
                    Language.GetString("RANGES_ALL_MODE_ME"), 
                    Language.GetString("RANGES_ALL_MODE_ENEMY"), 
                    Language.GetString("RANGES_ALL_MODE_BOTH"),  
                })
                });
                ShopRange.Menu.Add(new MenuItem<MenuColor>("SAssembliesRangesShopColorMe", Language.GetString("RANGES_ALL_COLORME")) { Value = new MenuColor(SharpDX.Color.LawnGreen) });
                ShopRange.Menu.Add(new MenuItem<MenuColor>("SAssembliesRangesShopColorEnemy", Language.GetString("RANGES_ALL_COLORENEMY")) { Value = new MenuColor(SharpDX.Color.IndianRed) });
                ShopRange.CreateActiveMenuItem("SAssembliesRangesShopActive");

                MainMenu2.ShopRange = ShopRange;
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
                Console.WriteLine("SAssemblies: " + e);
                threadActive = false;
            }
        }
    }
}
