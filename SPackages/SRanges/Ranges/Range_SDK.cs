using System;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAssemblies.Ranges
{
    internal class Range
    {
        public static Menu2.MenuItemSettings Ranges = new Menu2.MenuItemSettings();

        private Range()
        {

        }

        ~Range()
        {
            
        }

        private static void SetupMainMenu()
        {
            var menu = Menu2.CreateMainMenu("SAssembliesSRanges", "SRanges");
            SetupMenu(menu);
        }

        public static Menu2.MenuItemSettings SetupMenu(LeagueSharp.SDK.Core.UI.IMenu.Menu menu, bool useExisitingMenu = false)
        {
            Language.SetLanguage();
            if (!useExisitingMenu)
            {
                Ranges.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesRanges", Language.GetString("RANGES_RANGE_MAIN")));
            }
            else
            {
                Ranges.Menu = menu;
            }
            if (!useExisitingMenu)
            {
                Ranges.CreateActiveMenuItem("SAssembliesRangesActive");
            }
            return Ranges;
        }
    }
}