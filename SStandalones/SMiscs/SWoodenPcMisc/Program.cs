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

    class Program
    {

        private static bool threadActive = true;
        private static float lastDebugTime = 0;
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
            new WoodenPc();
        }

        public static Program Instance()
        {
            return instance;
        }
    }
}
