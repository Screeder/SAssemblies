﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SAssemblies.Timers
{
    class Summoner
    {
        public static Menu.MenuItemSettings SummonerTimer = new Menu.MenuItemSettings(typeof(Summoner));

        private static readonly Utility.Map GMap = Utility.Map.GetMap();
        private static Dictionary<Obj_AI_Hero, SummonerObject> Summoners = new Dictionary<Obj_AI_Hero, SummonerObject>();
        private int lastGameUpdateTime = 0;

        public Summoner()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            GameUpdate a = null;
            a = delegate(EventArgs args)
            {
                Init();
                Game.OnUpdate -= a;
            };
            Game.OnUpdate += a;
        }

        ~Summoner()
        {
            Game.OnUpdate -= Game_OnGameUpdate;
            Summoners = null;
        }

        public static bool IsActive()
        {
#if TIMERS
            return Timer.Timers.GetActive() && SummonerTimer.GetActive();
#else
            return SummonerTimer.GetActive();
#endif
        }

        public static Menu.MenuItemSettings SetupMenu(LeagueSharp.Common.Menu menu)
        {
            SummonerTimer.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("TIMERS_SUMMONER_MAIN"), "SAssembliesTimersSummoner"));
            SummonerTimer.MenuItems.Add(
                SummonerTimer.Menu.AddItem(new MenuItem("SAssembliesTimersSummonerSpeech", Language.GetString("GLOBAL_VOICE")).SetValue(false)));
            SummonerTimer.MenuItems.Add(SummonerTimer.CreateActiveMenuItem("SAssembliesTimersSummonerActive", () => new Summoner()));
            return SummonerTimer;
        }

        public void Init()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    Summoners.Add(hero, new SummonerObject());
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || lastGameUpdateTime + new Random().Next(500, 1000) > Environment.TickCount)
                return;

            lastGameUpdateTime = Environment.TickCount;

            if (SummonerTimer.GetActive())
            {
                foreach (var hero in Summoners)
                {
                    Obj_AI_Hero enemy = hero.Key;
                    List<SpellSlot> summonerSpells = new List<SpellSlot>();
                    summonerSpells.Add(SpellSlot.Summoner1);
                    summonerSpells.Add(SpellSlot.Summoner2);
                    for (int i = 0; i < summonerSpells.Count(); i++)
                    {
                        SpellDataInst spellData = enemy.Spellbook.GetSpell(summonerSpells[i]);
                        if (hero.Value.Called[i])
                        {
                            if (Timer.Timers.GetMenuItem("SAssembliesTimersRemindTime").GetValue<Slider>().Value < spellData.CooldownExpires - Game.ClockTime)
                            {
                                hero.Value.Called[i] = false;
                            }
                        }
                        if (!hero.Value.Called[i] && Timer.Timers.GetMenuItem("SAssembliesTimersRemindTime").GetValue<Slider>().Value > spellData.CooldownExpires - Game.ClockTime)
                        {
                            hero.Value.Called[i] = true;
                            String text = enemy.ChampionName + " ";
                            switch (spellData.Name.ToLower())
                            {
                                case "summonerbarrier":
                                    text = text + "Barrier";
                                    break;

                                case "summonerboost":
                                    text = text + "Cleanse";
                                    break;

                                case "summonerclairvoyance":
                                    text = text + "Clairvoyance";
                                    break;

                                case "summonerdot":
                                    text = text + "Ignite";
                                    break;

                                case "summonerexhaust":
                                    text = text + "Exhaust";
                                    break;

                                case "summonerflash":
                                    text = text + "Flash";
                                    break;

                                case "summonerhaste":
                                    text = text + "Ghost";
                                    break;

                                case "summonerheal":
                                    text = text + "Heal";
                                    break;

                                case "summonermana":
                                    text = text + "Clarity";
                                    break;

                                case "summonerodingarrison":
                                    text = text + "Garrison";
                                    break;

                                case "summonerrevive":
                                    text = text + "Revive";
                                    break;

                                case "smite":
                                    text = text + "Smite";
                                    break;

                                case "summonerteleport":
                                    text = text + "Teleport";
                                    break;
                            }
                            text = text + " " + Timer.Timers.GetMenuItem("SAssembliesTimersRemindTime").GetValue<Slider>().Value + " sec";
                            Timer.PingAndCall(text, new Vector3(), true, false);
                            if (SummonerTimer.GetMenuItem("SAssembliesTimersSummonerSpeech").GetValue<bool>())
                            {
                                Speech.Speak(text + "onds");
                            }
                        }
                    }
                }
            }
        }

        public class SummonerObject
        {
            public bool[] Called = new bool[] { true, true };
            public int LastTimeCalled;
        }
    }
}
