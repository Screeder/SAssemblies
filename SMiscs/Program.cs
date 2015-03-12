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
using Menu = SAssemblies.Menu;

namespace SAssemblies
{
    class MainMenu : Menu
    {
        public static MenuItemSettings Misc;
        public static MenuItemSettings AutoLevler;
        public static MenuItemSettings SkinChanger;
        public static MenuItemSettings SafeMovement;
        public static MenuItemSettings MoveToMouse;
        public static MenuItemSettings SurrenderVote;
        public static MenuItemSettings AutoLatern;
        public static MenuItemSettings AutoJump;
        public static MenuItemSettings TurnAround;
        public static MenuItemSettings MinionBars;
        public static MenuItemSettings MinionLocation;
        public static MenuItemSettings FlashJuke;
        public static MenuItemSettings EasyRangedJungle;
        public static MenuItemSettings RealTime;
        public static MenuItemSettings ShowPing;
        public static MenuItemSettings PingerName;
        public static MenuItemSettings AntiVisualScreenStealth;
        public static MenuItemSettings EloDisplayer;
        public static MenuItemSettings SmartPingImprove;
        public static MenuItemSettings WallJump;
        public static MenuItemSettings AntiNexusTurret;
        public static MenuItemSettings AntiLatern;
    }

    class Program
    {

        private static bool threadActive = true;
        static void Main(string[] args)
        {
            AssemblyResolver.Init();
            AppDomain.CurrentDomain.DomainUnload += delegate { threadActive = false; };
            AppDomain.CurrentDomain.ProcessExit += delegate { threadActive = false; };
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private async static void Game_OnGameLoad(EventArgs args)
        {
            CreateMenu();
            Game.PrintChat("SMiscs loaded!");
            new Thread(GameOnOnGameUpdate).Start();
        }

        private static void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("SMiscs", "SMiscs", true);

                MainMenu.Misc = Miscs.Misc.SetupMenu(menu);
                //MainMenu.AntiVisualScreenStealth = Miscs.AntiVisualScreenStealth.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.AntiNexusTurret = Miscs.AntiNexusTurret.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.AntiLatern = Miscs.AntiLatern.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.AutoJump = Miscs.AutoJump.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.AutoLatern = Miscs.AutoLatern.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.AutoLevler = Miscs.AutoLevler.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.EasyRangedJungle = Miscs.EasyRangedJungle.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.EloDisplayer = Miscs.EloDisplayer.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.FlashJuke = Miscs.FlashJuke.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.MinionBars = Miscs.MinionBars.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.MinionLocation = Miscs.MinionLocation.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.MoveToMouse = Miscs.MoveToMouse.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.PingerName = Miscs.PingerName.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.RealTime = Miscs.RealTime.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.SafeMovement = Miscs.SafeMovement.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.ShowPing = Miscs.ShowPing.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.SkinChanger = Miscs.SkinChanger.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.SmartPingImprove = Miscs.SmartPingImprove.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.SurrenderVote = Miscs.SurrenderVote.SetupMenu(MainMenu.Misc.Menu);
                MainMenu.TurnAround = Miscs.TurnAround.SetupMenu(MainMenu.Misc.Menu);
                //MainMenu.WallJump = Miscs.WallJump.SetupMenu(MainMenu.Misc.Menu);

                Menu.GlobalSettings.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("Global Settings", "SAssembliesGlobalSettings"));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsServerChatPingActive", "Server Chat/Ping").SetValue(false)));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAssembliesGlobalSettingsVoiceVolume", "Voice Volume").SetValue(new Slider(100, 0, 100))));

                menu.AddItem(new MenuItem("By Screeder", "By Screeder V0.8.0.4"));
                menu.AddToMainMenu();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GameOnOnGameUpdate(/*EventArgs args*/)
        {
            try
            {
                while (threadActive)
                {
                    Thread.Sleep(100);
                    Type classType = typeof(MainMenu);
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
                    FieldInfo[] fields = classType.GetFields(flags);
                    foreach (FieldInfo p in fields.ToList())
                    {
                        try
                        {
                            var item = (Menu.MenuItemSettings)p.GetValue(null);
                            if (item == null)
                            {
                                continue;
                            }
                            if (item.GetActive() == false && item.Item != null)
                            {
                                item.Item = null;
                                //GC.Collect();
                            }
                            else if (item.GetActive() && item.Item == null && !item.ForceDisable && item.Type != null)
                            {
                                try
                                {
                                    item.Item = System.Activator.CreateInstance(item.Type);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    threadActive = false;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("SAssemblies: " + e + "\n" + p.ToString());
                            threadActive = false;
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
