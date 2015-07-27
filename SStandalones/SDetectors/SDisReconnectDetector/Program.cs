using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using LeagueSharp.SDK.Core.UI;
using SAssemblies.Detectors;

namespace SAssemblies
{
    internal class MainMenu : Menu
    {
        public static MenuItemSettings Detector = new MenuItemSettings();
        public static MenuItemSettings DisconnectDetector = new MenuItemSettings();
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public MainMenu()
        {
            MenuEntries = new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { DisconnectDetector, () => new DisReconnect() }
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
        public static MenuItemSettings DisconnectDetector = new MenuItemSettings();
        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { DisconnectDetector, () => new DisReconnect() }
            };
        }

        public void UpdateDirEntry(ref MenuItemSettings oldMenuItem, MenuItemSettings newMenuItem)
        {
            var save = MenuEntries[oldMenuItem];
            MenuEntries.Remove(oldMenuItem);
            MenuEntries.Add(newMenuItem, save);
            oldMenuItem = newMenuItem;
        }
    }

    internal class Program
    {
        private static bool threadActive = true;
        private static float lastDebugTime = 0;
        private static readonly Program instance = new Program();
        private MainMenu2 mainMenu;

        public static void Main(string[] args)
        {
            AssemblyResolver.Init();
            AppDomain.CurrentDomain.DomainUnload += delegate { threadActive = false; };
            AppDomain.CurrentDomain.ProcessExit += delegate { threadActive = false; };
            Instance().Load();
        }

        public void Load()
        {
            mainMenu = new MainMenu2();
            LeagueSharp.SDK.Core.Events.Load.OnLoad += Game_OnGameLoad;
        }

        public static Program Instance()
        {
            return instance;
        }

        private async void Game_OnGameLoad(Object obj, EventArgs args)
        {
            CreateMenu();
            Common.ShowNotification("SDisReconnectDetector loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            try
            {
                var menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                MainMenu2.Detector = Detector.SetupMenu(menu, true);
                mainMenu.UpdateDirEntry(ref MainMenu2.DisconnectDetector, DisReconnect.SetupMenu(MainMenu2.Detector.Menu));
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
                        catch (Exception e) {}
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