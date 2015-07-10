using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using LeagueSharp.Common;
using LeagueSharp.SDK.Core.UI;
using SAssemblies.Detectors;

namespace SAssemblies
{
    internal class MainMenu : Menu
    {
        public static MenuItemSettings Detector = new MenuItemSettings();
        public static MenuItemSettings GankDetector = new MenuItemSettings();
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public MainMenu()
        {
            MenuEntries = new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { GankDetector, () => new Gank() },
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
            var save = MenuEntries[oldMenuItem];
            MenuEntries.Remove(oldMenuItem);
            MenuEntries.Add(newMenuItem, save);
            oldMenuItem = newMenuItem;
        }
    }

    class MainMenu2 : Menu2
    {
        public static MenuItemSettings Detector = new MenuItemSettings();
        public static MenuItemSettings GankDetector = new MenuItemSettings();
        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { GankDetector, () => new Gank() },
            };
        }
    }

    internal class Program
    {
        private static bool threadActive = true;
        private static float lastDebugTime = 0;
        private static readonly Program instance = new Program();
        private MainMenu mainMenu;

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
            Common.ShowNotification("SGankDetector loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            try
            {
                //Menu.MenuItemSettings tempSettings;
                //var menu = new LeagueSharp.Common.Menu("SGankDetector", "SGankDetector", true);

                //MainMenu.Detector = Detector.SetupMenu(menu, true);
                //mainMenu.UpdateDirEntry(ref MainMenu.GankDetector, Gank.SetupMenu(MainMenu.Detector.Menu));

                //Menu.GlobalSettings.Menu =
                //    menu.AddSubMenu(new LeagueSharp.Common.Menu("Global Settings", "SAssembliesGlobalSettings"));
                //Menu.GlobalSettings.MenuItems.Add(
                //    Menu.GlobalSettings.Menu.AddItem(
                //        new MenuItem("SAssembliesGlobalSettingsServerChatPingActive", "Server Chat/Ping").SetValue(
                //            false)));
                //Menu.GlobalSettings.MenuItems.Add(
                //    Menu.GlobalSettings.Menu.AddItem(
                //        new MenuItem("SAssembliesGlobalSettingsVoiceVolume", "Voice Volume").SetValue(
                //            new Slider(100, 0, 100))));

                //menu.AddItem(
                //    new MenuItem("By Screeder", "By Screeder V" + Assembly.GetExecutingAssembly().GetName().Version));
                //menu.AddToMainMenu();

                var menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Detector = Detector.SetupMenu(menu, true);
                //mainMenu.UpdateDirEntry(ref MainMenu.GankDetector, Gank.SetupMenu(MainMenu.Detector.Menu));

                Menu2.MenuItemSettings GankDetector = new Menu2.MenuItemSettings(typeof(Gank));

                GankDetector.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesDetectorsGank", Language.GetString("DETECTORS_GANK_MAIN")));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesDetectorsGankPingTimes", Language.GetString("GLOBAL_PING_TIMES"), 0, 0, 5));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuList<String>("SAssembliesDetectorsGankPingType", Language.GetString("GLOBAL_PING_TIMES"), new[]
                {
                    Language.GetString("GLOBAL_PING_TYPE_NORMAL"), 
                    Language.GetString("GLOBAL_PING_TYPE_DANGER"), 
                    Language.GetString("GLOBAL_PING_TYPE_ENEMYMISSING"), 
                    Language.GetString("GLOBAL_PING_TYPE_ONMYWAY"), 
                    Language.GetString("GLOBAL_PING_TYPE_FALLBACK"), 
                    Language.GetString("GLOBAL_PING_ASSISTME") 
                }));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsGankLocalPing", Language.GetString("GLOBAL_PING_LOCAL"), true));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsGankChat", Language.GetString("GLOBAL_CHAT")));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsGankNotification", Language.GetString("GLOBAL_NOTIFICATION")));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesDetectorsGankTrackRangeMin", Language.GetString("DETECTORS_GANK_RANGE_MIN"), 1, 1, 10000));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesDetectorsGankTrackRangeMax", Language.GetString("DETECTORS_GANK_RANGE_MAX"), 1, 1, 10000));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuSlider("SAssembliesDetectorsGankDisableTime", Language.GetString("DETECTORS_GANK_DISABLETIME"), 20, 1, 180));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsGankShowJungler", Language.GetString("DETECTORS_GANK_SHOWJUNGLER")));
                Menu2.AddComponent(ref GankDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsGankVoice", Language.GetString("GLOBAL_VOICE")));
                GankDetector.CreateActiveMenuItem("SAssembliesDetectorsGankActive");

                MainMenu2.GankDetector = GankDetector;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GameOnOnGameUpdate()
        {
            try
            {
                while (threadActive)
                {
                    Thread.Sleep(1000);

                    if (mainMenu == null)
                    {
                        continue;
                    }

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
                        catch (Exception e) { }
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