using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SAssemblies.Detectors
{
    class Detector
    {
        public static Menu2.MenuItemSettings Detectors = new Menu2.MenuItemSettings();

        private Detector()
        {

        }

        ~Detector()
        {
            
        }

        private static void SetupMainMenu()
        {
            var menu = Menu2.CreateMainMenu();
            Menu2.CreateGlobalMenuItems(menu);
            SetupMenu(menu);
        }

        public static Menu2.MenuItemSettings SetupMenu(LeagueSharp.SDK.Core.UI.IMenu.Menu menu, bool useExisitingMenu = false)
        {
            Language.SetLanguage();
            if (!useExisitingMenu)
            {
                Detectors.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesDetectors", Language.GetString("DETECTORS_DETECTOR_MAIN")));
            }
            else
            {
                Detectors.Menu = menu;
            }
            if (!useExisitingMenu)
            {
                Detectors.CreateActiveMenuItem("SAssembliesDetectorsActive");
            }
            return Detectors;
        }
    }
}
