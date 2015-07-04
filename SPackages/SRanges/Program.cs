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

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Range = new MenuItemSettings();
        public static MenuItemSettings TurretRange = new MenuItemSettings();
        public static MenuItemSettings ShopRange = new MenuItemSettings();
        public static MenuItemSettings VisionRange = new MenuItemSettings();
        public static MenuItemSettings ExperienceRange = new MenuItemSettings();
        public static MenuItemSettings AttackRange = new MenuItemSettings();
        public static MenuItemSettings SpellQRange = new MenuItemSettings();
        public static MenuItemSettings SpellWRange = new MenuItemSettings();
        public static MenuItemSettings SpellERange = new MenuItemSettings();
        public static MenuItemSettings SpellRRange = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { TurretRange, () => new Turret() },
                { ShopRange, () => new Shop() },
                { VisionRange, () => new Vision() },
                { ExperienceRange, () => new Experience() },
                { AttackRange, () => new Attack() },
                { SpellQRange, () => new SpellQ() },
                { SpellWRange, () => new SpellW() },
                { SpellERange, () => new SpellE() },
                { SpellRRange, () => new SpellR() },
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
        public static MenuItemSettings TurretRange = new MenuItemSettings();
        public static MenuItemSettings ShopRange = new MenuItemSettings();
        public static MenuItemSettings VisionRange = new MenuItemSettings();
        public static MenuItemSettings ExperienceRange = new MenuItemSettings();
        public static MenuItemSettings AttackRange = new MenuItemSettings();
        public static MenuItemSettings SpellQRange = new MenuItemSettings();
        public static MenuItemSettings SpellWRange = new MenuItemSettings();
        public static MenuItemSettings SpellERange = new MenuItemSettings();
        public static MenuItemSettings SpellRRange = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { TurretRange, () => new Turret() },
                { ShopRange, () => new Shop() },
                { VisionRange, () => new Vision() },
                { ExperienceRange, () => new Experience() },
                { AttackRange, () => new Attack() },
                { SpellQRange, () => new SpellQ() },
                { SpellWRange, () => new SpellW() },
                { SpellERange, () => new SpellE() },
                { SpellRRange, () => new SpellR() },
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
            Common.ShowNotification("SRanges loaded!", Color.LawnGreen, 5000);

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
                //mainMenu.UpdateDirEntry(ref MainMenu.SpellQRange, SpellQ.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SpellWRange, SpellW.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SpellERange, SpellE.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SpellRRange, SpellR.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.ShopRange, Shop.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.VisionRange, Vision.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.ExperienceRange, Experience.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AttackRange, Attack.SetupMenu(MainMenu.Range.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.TurretRange, Turret.SetupMenu(MainMenu.Range.Menu));

                Menu2.MenuItemSettings Ranges = new Menu2.MenuItemSettings();

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAwarenessRanges", Language.GetString("RANGES_RANGE_MAIN")));
                Ranges.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAwarenessRanges"];
                Ranges.CreateActiveMenuItem("SAwarenessRangesActive");

                MainMenu2.Range = Ranges;
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
