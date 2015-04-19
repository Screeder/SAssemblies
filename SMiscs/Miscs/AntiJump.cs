using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SAssemblies;
using SAssemblies.Miscs;
using Menu = SAssemblies.Menu;

namespace SAwareness.Miscs
{
    class AntiJump
    {
        public static Menu.MenuItemSettings AntiJumpMisc = new Menu.MenuItemSettings(typeof(AntiJump));
        public static List<Champs> Champions = new List<Champs>();

        public AntiJump()
        {
            Obj_AI_Hero.OnPlayAnimation += Obj_AI_Hero_OnPlayAnimation;
        }

        ~AntiJump()
        {
            Obj_AI_Hero.OnPlayAnimation -= Obj_AI_Hero_OnPlayAnimation;
        }

        public bool IsActive()
        {
            return Misc.Miscs.GetActive() && AntiJumpMisc.GetActive();
        }

        public static Menu.MenuItemSettings SetupMenu(LeagueSharp.Common.Menu menu)
        {
            AntiJumpMisc.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("MISCS_ANTIJUMP_MAIN"), "SAssembliesMiscsAntiJump"));
            AntiJumpMisc.MenuItems.Add(
                AntiJumpMisc.Menu.AddItem(new MenuItem("SAssembliesMiscsAntiJumpActive", Language.GetString("GLOBAL_ACTIVE")).SetValue(false)));
            return AntiJumpMisc;
        }

        void Obj_AI_Hero_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender is Obj_AI_Hero)
            {
                var hero = (Obj_AI_Hero)sender;
                if (hero.Team != ObjectManager.Player.Team)
                {
                    if (IsJumping(hero, args.Animation))
                    {
                        //if (_E.IsReady())
                        //{
                        //    _E.Cast(unit.Position, PacketCasting());
                        //}
                    }
                }
            }
        }

        bool IsJumping(Obj_AI_Hero champion, String animation)
        {
            switch (champion.ChampionName)
            {
                case "Rengar":
                    if (animation.Contains("Spell5") && ObjectManager.Player.Distance(champion) <= 700)
                        return true;
                    break;

                case "Khazix":
                    if (animation.Contains("Spell3") && ObjectManager.Player.Distance(champion) <= 700)
                        return true;
                    break;
            }
            return false;
        }

        internal class Champs
        {
            public String Name;
            public SpellSlot SpellSlot;
            public int Range;
            public Champs(string name, int range, SpellSlot spellSlot)
            {
                Name = name;
                Range = range;
                SpellSlot = spellSlot;
            }
        }
    }
}
