using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public static MenuItemSettings AutoLevlerMisc = new MenuItemSettings();
        public static MenuItemSettings SkinChangerMisc = new MenuItemSettings();
        public static MenuItemSettings SafeMovementMisc = new MenuItemSettings();
        public static MenuItemSettings MoveToMouseMisc = new MenuItemSettings();
        public static MenuItemSettings SurrenderVoteMisc = new MenuItemSettings();
        public static MenuItemSettings AutoLaternMisc = new MenuItemSettings();
        public static MenuItemSettings AutoJumpMisc = new MenuItemSettings();
        public static MenuItemSettings TurnAroundMisc = new MenuItemSettings();
        public static MenuItemSettings MinionBarsMisc = new MenuItemSettings();
        public static MenuItemSettings MinionLocationMisc = new MenuItemSettings();
        public static MenuItemSettings FlashJukeMisc = new MenuItemSettings();
        public static MenuItemSettings EasyRangedJungleMisc = new MenuItemSettings();
        public static MenuItemSettings RealTimeMisc = new MenuItemSettings();
        public static MenuItemSettings ShowPingMisc = new MenuItemSettings();
        public static MenuItemSettings PingerNameMisc = new MenuItemSettings();
        public static MenuItemSettings AntiVisualScreenStealthMisc = new MenuItemSettings();
        public static MenuItemSettings EloDisplayerMisc = new MenuItemSettings();
        public static MenuItemSettings SmartPingImproveMisc = new MenuItemSettings();
        public static MenuItemSettings WallJumpMisc = new MenuItemSettings();
        public static MenuItemSettings AntiNexusTurretMisc = new MenuItemSettings();
        public static MenuItemSettings AntiLaternMisc = new MenuItemSettings();
        public static MenuItemSettings AutoBuyMisc = new MenuItemSettings();
        public static MenuItemSettings AntiJumpMisc = new MenuItemSettings();

        public MainMenu()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoLevlerMisc, () => new AutoLevler() },
                { SkinChangerMisc, () => new SkinChanger() },
                { SafeMovementMisc, () => new SafeMovement() },
                { MoveToMouseMisc, () => new MoveToMouse() },
                { SurrenderVoteMisc, () => new SurrenderVote() },
                { AutoLaternMisc, () => new AutoLatern() },
                { AutoJumpMisc, () => new AutoJump() },
                { TurnAroundMisc, () => new TurnAround() },
                { MinionBarsMisc, () => new MinionBars() },
                { MinionLocationMisc, () => new MinionLocation() },
                { FlashJukeMisc, () => new FlashJuke() },
                { EasyRangedJungleMisc, () => new EasyRangedJungle() },
                { RealTimeMisc, () => new RealTime() },
                { ShowPingMisc, () => new ShowPing() },
                { PingerNameMisc, () => new PingerName() },
                { AntiVisualScreenStealthMisc, () => new AntiVisualScreenStealth() },
                { EloDisplayerMisc, () => new EloDisplayer() },
                { SmartPingImproveMisc, () => new SmartPingImprove() },
                { WallJumpMisc, () => new WallJump() },
                { AntiNexusTurretMisc, () => new AntiNexusTurret() },
                { AntiLaternMisc, () => new AntiLatern() },   
                { AutoBuyMisc, () => new AutoBuy() },
                { AntiJumpMisc, () => new AntiJump() },
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
        public static MenuItemSettings Misc = new MenuItemSettings();
        public static MenuItemSettings AutoLevlerMisc = new MenuItemSettings();
        public static MenuItemSettings SkinChangerMisc = new MenuItemSettings();
        public static MenuItemSettings SafeMovementMisc = new MenuItemSettings();
        public static MenuItemSettings MoveToMouseMisc = new MenuItemSettings();
        public static MenuItemSettings SurrenderVoteMisc = new MenuItemSettings();
        public static MenuItemSettings AutoLaternMisc = new MenuItemSettings();
        public static MenuItemSettings AutoJumpMisc = new MenuItemSettings();
        public static MenuItemSettings TurnAroundMisc = new MenuItemSettings();
        public static MenuItemSettings MinionBarsMisc = new MenuItemSettings();
        public static MenuItemSettings MinionLocationMisc = new MenuItemSettings();
        public static MenuItemSettings FlashJukeMisc = new MenuItemSettings();
        public static MenuItemSettings EasyRangedJungleMisc = new MenuItemSettings();
        public static MenuItemSettings RealTimeMisc = new MenuItemSettings();
        public static MenuItemSettings ShowPingMisc = new MenuItemSettings();
        public static MenuItemSettings PingerNameMisc = new MenuItemSettings();
        public static MenuItemSettings AntiVisualScreenStealthMisc = new MenuItemSettings();
        public static MenuItemSettings EloDisplayerMisc = new MenuItemSettings();
        public static MenuItemSettings SmartPingImproveMisc = new MenuItemSettings();
        public static MenuItemSettings WallJumpMisc = new MenuItemSettings();
        public static MenuItemSettings AntiNexusTurretMisc = new MenuItemSettings();
        public static MenuItemSettings AntiLaternMisc = new MenuItemSettings();
        public static MenuItemSettings AutoBuyMisc = new MenuItemSettings();
        public static MenuItemSettings AntiJumpMisc = new MenuItemSettings();

        public MainMenu2()
        {
            MenuEntries =
            new Dictionary<MenuItemSettings, Func<dynamic>>
            {
                { AutoLevlerMisc, () => new AutoLevler() },
                { SkinChangerMisc, () => new SkinChanger() },
                { SafeMovementMisc, () => new SafeMovement() },
                { MoveToMouseMisc, () => new MoveToMouse() },
                { SurrenderVoteMisc, () => new SurrenderVote() },
                { AutoLaternMisc, () => new AutoLatern() },
                { AutoJumpMisc, () => new AutoJump() },
                { TurnAroundMisc, () => new TurnAround() },
                { MinionBarsMisc, () => new MinionBars() },
                { MinionLocationMisc, () => new MinionLocation() },
                { FlashJukeMisc, () => new FlashJuke() },
                { EasyRangedJungleMisc, () => new EasyRangedJungle() },
                { RealTimeMisc, () => new RealTime() },
                { ShowPingMisc, () => new ShowPing() },
                { PingerNameMisc, () => new PingerName() },
                { AntiVisualScreenStealthMisc, () => new AntiVisualScreenStealth() },
                { EloDisplayerMisc, () => new EloDisplayer() },
                { SmartPingImproveMisc, () => new SmartPingImprove() },
                { WallJumpMisc, () => new WallJump() },
                { AntiNexusTurretMisc, () => new AntiNexusTurret() },
                { AntiLaternMisc, () => new AntiLatern() },   
                { AutoBuyMisc, () => new AutoBuy() },
                { AntiJumpMisc, () => new AntiJump() },
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
            new WoodenPc();
            LeagueSharp.SDK.Core.Events.Load.OnLoad += Game_OnGameLoad;
        }

        public static Program Instance()
        {
            return instance;
        }

        private async void Game_OnGameLoad(Object obj, EventArgs args)
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
                var menu = Menu2.CreateMainMenu();
                Menu2.CreateGlobalMenuItems(menu);

                //MainMenu.Misc = Misc.SetupMenu(menu);
                //mainMenu.UpdateDirEntry(ref MainMenu.AntiVisualScreenStealth, AntiVisualScreenStealth.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AntiNexusTurret, AntiNexusTurret.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AntiLatern, AntiLatern.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AntiJump, AntiJump.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.AutoBuy, Miscs.AutoBuy.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.AutoJump, AutoJump.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.AutoLatern, Miscs.AutoLatern.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.AutoLevler, Miscs.AutoLevler.SetupMenu(MainMenu.Misc.Menu)); //Hängt bei linkslick
                //mainMenu.UpdateDirEntry(ref MainMenu.EasyRangedJungle, EasyRangedJungle.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.EloDisplayer, Miscs.EloDisplayer.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.FlashJuke, FlashJuke.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.MinionBars, Miscs.MinionBars.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.MinionLocation, Miscs.MinionLocation.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.MoveToMouse, MoveToMouse.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.PingerName, PingerName.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.RealTime, RealTime.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.SafeMovement, Miscs.SafeMovement.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.ShowPing, ShowPing.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.SkinChanger, SkinChanger.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.SmartPingImprove, Miscs.SmartPingImprove.SetupMenu(MainMenu.Misc.Menu));
                ////mainMenu.UpdateDirEntry(ref MainMenu.SurrenderVote, Miscs.SurrenderVote.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.TurnAround, TurnAround.SetupMenu(MainMenu.Misc.Menu));
                //mainMenu.UpdateDirEntry(ref MainMenu.WallJump, WallJump.SetupMenu(MainMenu.Misc.Menu));

                Menu2.MenuItemSettings Miscs = new Menu2.MenuItemSettings();

                Miscs.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesMiscs", Language.GetString("MISCS_MISC_MAIN")));
                Miscs.CreateActiveMenuItem("SAssembliesMiscsActive");

                MainMenu2.Misc = Miscs;
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
