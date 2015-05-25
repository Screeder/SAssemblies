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
using SAssemblies.Timers;
using Spell = SAssemblies.Timers.Spell;
using Timer = SAssemblies.Timers.Timer;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Timers = new MenuItemSettings();
        public static MenuItemSettings JungleTimer = new MenuItemSettings();
        public static MenuItemSettings RelictTimer = new MenuItemSettings();
        public static MenuItemSettings HealthTimer = new MenuItemSettings();
        public static MenuItemSettings InhibitorTimer = new MenuItemSettings();
        public static MenuItemSettings SummonerTimer = new MenuItemSettings();
        public static MenuItemSettings ImmuneTimer = new MenuItemSettings();
        public static MenuItemSettings AltarTimer = new MenuItemSettings();
        public static MenuItemSettings SpellTimer = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { JungleTimer, () => new Jungle() },
                { RelictTimer, () => new Relic() },
                { HealthTimer, () => new Health() },
                { InhibitorTimer, () => new Inhibitor() },
                { SummonerTimer, () => new Summoner() },
                { ImmuneTimer, () => new Immune() },
                { AltarTimer, () => new Altar() },
                { SpellTimer, () => new Timers.Spell() },
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
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static Program Instance()
        {
            return instance;
        }

        private async void Game_OnGameLoad(EventArgs args)
        {
            CreateMenu();
            Common.ShowNotification("STimers loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("STimers", "STimers", true);

                MainMenu.Timers = Timer.SetupMenu(menu);
                mainMenu.UpdateDirEntry(ref MainMenu.AltarTimer, Altar.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.HealthTimer, Health.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.ImmuneTimer, Immune.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.InhibitorTimer, Inhibitor.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.JungleTimer, Jungle.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.RelictTimer, Relic.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.SummonerTimer, Summoner.SetupMenu(MainMenu.Timers.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.SpellTimer, Spell.SetupMenu(MainMenu.Timers.Menu));

                Menu.GlobalSettings.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("Global Settings", "SAssembliesGlobalSettings"));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsServerChatPingActive", "Server Chat/Ping").SetValue(false)));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsVoiceVolume", "Voice Volume").SetValue(new Slider(100, 0, 100))));

                menu.AddItem(new MenuItem("By Screeder", "By Screeder V" + Assembly.GetExecutingAssembly().GetName().Version));
                menu.AddToMainMenu();
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
