using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK.Core.UI.IMenu.Values;

namespace SAssemblies.Detectors
{
    class DisReconnect
    {
        public static Menu2.MenuItemSettings DisReconnectDetector = new Menu2.MenuItemSettings(typeof(DisReconnect));

        public DisReconnect()
        {
            Game.OnProcessPacket += Game_OnGameProcessPacket;
        }

        ~DisReconnect()
        {
            Game.OnProcessPacket -= Game_OnGameProcessPacket;
        }

        public bool IsActive()
        {
#if DETECTORS
            return Detector.Detectors.GetActive() && DisReconnectDetector.GetActive();
#else
            return DisReconnectDetector.GetActive();
#endif
        }

        public static Menu2.MenuItemSettings SetupMenu(LeagueSharp.SDK.Core.UI.IMenu.Menu menu)
        {
            DisReconnectDetector.Menu = Menu2.AddMenu(ref menu, new LeagueSharp.SDK.Core.UI.IMenu.Menu("SAssembliesDetectorsDisReconnect", Language.GetString("DETECTORS_DISRECONNECT_MAIN")));
            Menu2.AddComponent(ref DisReconnectDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsDisReconnectChat", Language.GetString("GLOBAL_CHAT")));
            Menu2.AddComponent(ref DisReconnectDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsDisReconnectNotification", Language.GetString("GLOBAL_NOTIFICATION")));
            Menu2.AddComponent(ref DisReconnectDetector.Menu, new LeagueSharp.SDK.Core.UI.IMenu.Values.MenuBool("SAssembliesDetectorsDisReconnectSpeech", Language.GetString("GLOBAL_VOICE")));
            DisReconnectDetector.CreateActiveMenuItem("SAssembliesDetectorsDisReconnectActive");
            return DisReconnectDetector;
        }

        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;
            DetectDisconnect(args);
            DetectReconnect(args);
        }

        private void DetectDisconnect(GamePacketEventArgs args)
        {
            try
            {
                var reader = new BinaryReader(new MemoryStream(args.PacketData));
                byte packetId = reader.ReadByte(); //PacketId
                if (packetId != 249 || args.PacketData.Length != 12)
                    return;
                if (DisReconnectDetector.Menu["SAssembliesDetectorsDisReconnectChat"].GetValue<MenuBool>().Value &&
                        Menu2.GlobalSettings.Menu["SAssembliesGlobalSettingsServerChatPingActive"].GetValue<MenuBool>().Value)
                {
                    Game.Say("A Champion has disconnected!");
                }
                if (DisReconnectDetector.Menu["SAssembliesDetectorsDisReconnectSpeech"].GetValue<MenuBool>().Value)
                {
                    Speech.Speak("A Champion has disconnected!");
                }
                if (DisReconnectDetector.Menu["SAssembliesDetectorsDisReconnectNotification"].GetValue<MenuBool>().Value)
                {
                    Common.ShowNotification("A Champion has disconnected!", Color.LawnGreen, 3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DisconnectProcess: " + ex);
            }
        }

        private void DetectReconnect(GamePacketEventArgs args)
        {
            try
            {
                var reader = new BinaryReader(new MemoryStream(args.PacketData));
                byte packetId = reader.ReadByte(); //PacketId
                if (packetId != 142 || args.PacketData.Length != 6)
                    return;
                if (
                    DisReconnectDetector.Menu["SAssembliesDetectorsDisReconnectChat"].GetValue<MenuBool>().Value &&
                    Menu2.GlobalSettings.Menu["SAssembliesGlobalSettingsServerChatPingActive"].GetValue<MenuBool>().Value)
                {
                    Game.Say("A Champion has reconnected!");
                }
                if (DisReconnectDetector.Menu["SAssembliesDetectorsDisReconnectSpeech"].GetValue<MenuBool>().Value)
                {
                    Speech.Speak("A Champion has reconnected!");
                }
                if (DisReconnectDetector.Menu["SAssembliesDetectorsDisReconnectNotification"].GetValue<MenuBool>().Value)
                {
                    Common.ShowNotification("A Champion has reconnected!", Color.Yellow, 3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReconnectProcess: " + ex);
            }
        }
    }
}
