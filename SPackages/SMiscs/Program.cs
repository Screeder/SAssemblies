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

namespace SAssemblies
{
    class MainMenu : Menu
    {
        private readonly Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;

        public static MenuItemSettings Misc = new MenuItemSettings();
        public static MenuItemSettings AutoLevler = new MenuItemSettings();
        public static MenuItemSettings SkinChanger = new MenuItemSettings();
        public static MenuItemSettings SafeMovement = new MenuItemSettings();
        public static MenuItemSettings MoveToMouse = new MenuItemSettings();
        public static MenuItemSettings SurrenderVote = new MenuItemSettings();
        public static MenuItemSettings AutoLatern = new MenuItemSettings();
        public static MenuItemSettings AutoJump = new MenuItemSettings();
        public static MenuItemSettings TurnAround = new MenuItemSettings();
        public static MenuItemSettings MinionBars = new MenuItemSettings();
        public static MenuItemSettings MinionLocation = new MenuItemSettings();
        public static MenuItemSettings FlashJuke = new MenuItemSettings();
        public static MenuItemSettings EasyRangedJungle = new MenuItemSettings();
        public static MenuItemSettings RealTime = new MenuItemSettings();
        public static MenuItemSettings ShowPing = new MenuItemSettings();
        public static MenuItemSettings PingerName = new MenuItemSettings();
        public static MenuItemSettings AntiVisualScreenStealth = new MenuItemSettings();
        public static MenuItemSettings EloDisplayer = new MenuItemSettings();
        public static MenuItemSettings SmartPingImprove = new MenuItemSettings();
        public static MenuItemSettings WallJump = new MenuItemSettings();
        public static MenuItemSettings AntiNexusTurret = new MenuItemSettings();
        public static MenuItemSettings AntiLatern = new MenuItemSettings();
        public static MenuItemSettings AutoBuy = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoLevler, () => new AutoLevler() },
                { SkinChanger, () => new SkinChanger() },
                { SafeMovement, () => new SafeMovement() },
                { MoveToMouse, () => new MoveToMouse() },
                { SurrenderVote, () => new SurrenderVote() },
                { AutoLatern, () => new AutoLatern() },
                { AutoJump, () => new AutoJump() },
                { TurnAround, () => new TurnAround() },
                { MinionBars, () => new MinionBars() },
                { MinionLocation, () => new MinionLocation() },
                { FlashJuke, () => new FlashJuke() },
                { EasyRangedJungle, () => new EasyRangedJungle() },
                { RealTime, () => new RealTime() },
                { ShowPing, () => new ShowPing() },
                { PingerName, () => new PingerName() },
                { AntiVisualScreenStealth, () => new AntiVisualScreenStealth() },
                { EloDisplayer, () => new EloDisplayer() },
                { SmartPingImprove, () => new SmartPingImprove() },
                { WallJump, () => new WallJump() },
                { AntiNexusTurret, () => new AntiNexusTurret() },
                { AntiLatern, () => new AntiLatern() },   
                { AutoBuy, () => new AutoBuy() },
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
            Common.ShowNotification("SMiscs loaded!", Color.LawnGreen, 5000);

            new Thread(GameOnOnGameUpdate).Start();
        }

        private void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("SMiscs", "SMiscs", true);

                MainMenu.Misc = Misc.SetupMenu(menu);
                mainMenu.UpdateDirEntry(ref MainMenu.AntiVisualScreenStealth, AntiVisualScreenStealth.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.AntiNexusTurret, AntiNexusTurret.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.AntiLatern, AntiLatern.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AutoBuy, Miscs.AutoBuy.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.AutoJump, AutoJump.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AutoLatern, Miscs.AutoLatern.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AutoLevler, Miscs.AutoLevler.SetupMenu(MainMenu.Misc.Menu)); //Hängt bei linkslick
                mainMenu.UpdateDirEntry(ref MainMenu.EasyRangedJungle, EasyRangedJungle.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.EloDisplayer, Miscs.EloDisplayer.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.FlashJuke, FlashJuke.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.MinionBars, Miscs.MinionBars.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.MinionLocation, Miscs.MinionLocation.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.MoveToMouse, MoveToMouse.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.PingerName, PingerName.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.RealTime, RealTime.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SafeMovement, Miscs.SafeMovement.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.ShowPing, ShowPing.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.SkinChanger, SkinChanger.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SmartPingImprove, Miscs.SmartPingImprove.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SurrenderVote, Miscs.SurrenderVote.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.TurnAround, TurnAround.SetupMenu(MainMenu.Misc.Menu));
                mainMenu.UpdateDirEntry(ref MainMenu.WallJump, WallJump.SetupMenu(MainMenu.Misc.Menu));

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
