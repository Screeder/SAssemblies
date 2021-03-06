﻿using System;
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
        public static MenuItemSettings FoWSpellEnemyDetector = new MenuItemSettings();
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public MainMenu()
        {
            MenuEntries = new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { FoWSpellEnemyDetector, () => new FoWSpellEnemy() }
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
        public static MenuItemSettings FoWSpellEnemyDetector = new MenuItemSettings();
        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { FoWSpellEnemyDetector, () => new FoWSpellEnemy() },
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
            Common.ShowNotification("SFoWSpellEnemyDetector loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            try
            {
                //Menu.MenuItemSettings tempSettings;
                //var menu = new LeagueSharp.Common.Menu("SFoWSpellEnemyDetector", "SFoWSpellEnemyDetector", true);

                //MainMenu.Detector = Detector.SetupMenu(menu, true);
                //mainMenu.UpdateDirEntry(
                //    ref MainMenu.FoWSpellEnemyDetector, FoWSpellEnemy.SetupMenu(MainMenu.Detector.Menu));

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
                //mainMenu.UpdateDirEntry(ref MainMenu.DisconnectDetector, DisReconnect.SetupMenu(MainMenu.Detector.Menu));

                Menu2.MenuItemSettings FoWSpellEnemyDetector = new Menu2.MenuItemSettings(typeof(FoWSpellEnemy));

                FoWSpellEnemyDetector.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesDetectorsFoWSpellEnemy", Language.GetString("DETECTORS_FOWSPELLENEMY_MAIN")));
                FoWSpellEnemyDetector.CreateActiveMenuItem("SAssembliesDetectorsFoWSpellEnemyActive");

                MainMenu2.FoWSpellEnemyDetector = FoWSpellEnemyDetector;
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