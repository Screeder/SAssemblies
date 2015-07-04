using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using LeagueSharp.Common;
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.UI.Values;
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

                LeagueSharp.SDK.Core.UI.Menu menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Detector = Detector.SetupMenu(menu, true);
                //mainMenu.UpdateDirEntry(ref MainMenu.GankDetector, Gank.SetupMenu(MainMenu.Detector.Menu));

                Menu2.MenuItemSettings GankDetector = new Menu2.MenuItemSettings(typeof(Gank));

                menu.Add(new LeagueSharp.SDK.Core.UI.Menu("SAssembliesDetectorsGank", Language.GetString("DETECTORS_GANK_MAIN")));
                GankDetector.Menu = (LeagueSharp.SDK.Core.UI.Menu)menu["SAssembliesDetectorsGank"];
                GankDetector.Menu.Add(new MenuItem<MenuSlider>("SAssembliesDetectorsGankPingTimes", Language.GetString("GLOBAL_PING_TIMES")) { Value = new MenuSlider(0, 0, 5) });
                GankDetector.Menu.Add(new MenuItem<MenuList<String>>("SAssembliesDetectorsGankPingType", Language.GetString("GLOBAL_PING_TIMES")) { Value = new MenuList<string>(new[]
                {
                    Language.GetString("GLOBAL_PING_TYPE_NORMAL"), 
                    Language.GetString("GLOBAL_PING_TYPE_DANGER"), 
                    Language.GetString("GLOBAL_PING_TYPE_ENEMYMISSING"), 
                    Language.GetString("GLOBAL_PING_TYPE_ONMYWAY"), 
                    Language.GetString("GLOBAL_PING_TYPE_FALLBACK"), 
                    Language.GetString("GLOBAL_PING_ASSISTME") 
                })});
                GankDetector.Menu.Add(new MenuItem<MenuBool>("SAssembliesDetectorsGankLocalPing", Language.GetString("GLOBAL_PING_LOCAL")) { Value = new MenuBool(true) });
                GankDetector.Menu.Add(new MenuItem<MenuBool>("SAssembliesDetectorsGankChat", Language.GetString("GLOBAL_CHAT")) { Value = new MenuBool() });
                GankDetector.Menu.Add(new MenuItem<MenuBool>("SAssembliesDetectorsGankNotification", Language.GetString("GLOBAL_NOTIFICATION")) { Value = new MenuBool() });
                GankDetector.Menu.Add(new MenuItem<MenuSlider>("SAssembliesDetectorsGankTrackRangeMin", Language.GetString("DETECTORS_GANK_RANGE_MIN")) { Value = new MenuSlider(1, 1, 10000) });
                GankDetector.Menu.Add(new MenuItem<MenuSlider>("SAssembliesDetectorsGankTrackRangeMax", Language.GetString("DETECTORS_GANK_RANGE_MAX")) { Value = new MenuSlider(1, 1, 10000) });
                GankDetector.Menu.Add(new MenuItem<MenuSlider>("SAssembliesDetectorsGankDisableTime", Language.GetString("DETECTORS_GANK_DISABLETIME")) { Value = new MenuSlider(20, 1, 180) });
                GankDetector.Menu.Add(new MenuItem<MenuBool>("SAssembliesDetectorsGankShowJungler", Language.GetString("DETECTORS_GANK_SHOWJUNGLER")) { Value = new MenuBool() });
                GankDetector.Menu.Add(new MenuItem<MenuBool>("SAssembliesDetectorsGankVoice", Language.GetString("GLOBAL_VOICE")) { Value = new MenuBool() });
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