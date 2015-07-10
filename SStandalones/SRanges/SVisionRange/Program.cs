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

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Range = new MenuItemSettings();
        public static MenuItemSettings VisionRange = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { VisionRange, () => new Vision() },
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
        public static MenuItemSettings VisionRange = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { VisionRange, () => new Vision() },
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
            Common.ShowNotification("SVisionRange loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                var menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Range = Range.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.VisionRange, Vision.SetupMenu(MainMenu.Range.Menu));

                Menu2.MenuItemSettings VisionRange = new Menu2.MenuItemSettings(typeof(Vision));

                VisionRange.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesRangesVision", Language.GetString("RANGES_VISION_MAIN")));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesRangesVisionMode", Language.GetString("RANGES_ALL_MODE"), new[]
                {
                    Language.GetString("RANGES_ALL_MODE_ME"), 
                    Language.GetString("RANGES_ALL_MODE_ENEMY"), 
                    Language.GetString("RANGES_ALL_MODE_BOTH"),  
                }));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesRangesVisionDisplayMe", Language.GetString("RANGES_VISION_ME")));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesRangesVisionDisplayChampion", Language.GetString("RANGES_VISION_CHAMPION")));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesRangesVisionDisplayTurret", Language.GetString("RANGES_VISION_TURRET")));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesRangesVisionDisplayMinion", Language.GetString("RANGES_VISION_MINION")));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesRangesVisionDisplayWard", Language.GetString("RANGES_VISION_WARD")));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuColor("SAssembliesRangesVisionColorMe", Language.GetString("RANGES_ALL_COLORME"), SharpDX.Color.LawnGreen));
                Menu2.AddComponent(ref VisionRange.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuColor("SAssembliesRangesVisionColorEnemy", Language.GetString("RANGES_ALL_COLORENEMY"), SharpDX.Color.IndianRed));
                VisionRange.CreateActiveMenuItem("SAssembliesRangesVisionActive");

                MainMenu2.VisionRange = VisionRange;
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
