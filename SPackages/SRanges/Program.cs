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
        public static MenuItemSettings Range;
        public static MenuItemSettings TurretRange;
        public static MenuItemSettings ShopRange;
        public static MenuItemSettings VisionRange;
        public static MenuItemSettings ExperienceRange;
        public static MenuItemSettings AttackRange;
        public static MenuItemSettings SpellQRange;
        public static MenuItemSettings SpellWRange;
        public static MenuItemSettings SpellERange;
        public static MenuItemSettings SpellRRange;
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
            Game.PrintChat("SRanges loaded!");
            new Thread(GameOnOnGameUpdate).Start();
        }

        private static void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("SRanges", "SRanges", true);

                MainMenu.Range = Ranges.Range.SetupMenu(menu);
                MainMenu.SpellQRange = Ranges.SpellQ.SetupMenu(MainMenu.Range.Menu);
                MainMenu.SpellWRange = Ranges.SpellW.SetupMenu(MainMenu.Range.Menu);
                MainMenu.SpellERange = Ranges.SpellE.SetupMenu(MainMenu.Range.Menu);
                MainMenu.SpellRRange = Ranges.SpellR.SetupMenu(MainMenu.Range.Menu);
                MainMenu.ShopRange = Ranges.Shop.SetupMenu(MainMenu.Range.Menu);
                MainMenu.VisionRange = Ranges.Vision.SetupMenu(MainMenu.Range.Menu);
                MainMenu.ExperienceRange = Ranges.Experience.SetupMenu(MainMenu.Range.Menu);
                MainMenu.AttackRange = Ranges.Attack.SetupMenu(MainMenu.Range.Menu);
                MainMenu.TurretRange = Ranges.Turret.SetupMenu(MainMenu.Range.Menu);

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
