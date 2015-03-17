using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAssemblies.Wards
{
    class FowWardPlacement
    {
        public static Menu.MenuItemSettings FowWardPlacementWard = new Menu.MenuItemSettings(typeof(FowWardPlacement));

		Dictionary<Obj_AI_Hero, List<ExpandedWardItem>> _wards = new Dictionary<Obj_AI_Hero, List<ExpandedWardItem>>();
		
		private int lastGameUpdateTime = 0;

        public class ExpandedWardItem : SAssemblies.Ward.WardItem
        {
            public int Stacks;
            public int Charges;

            public ExpandedWardItem(int id, string name, string spellName, int range, int duration, SAssemblies.Ward.WardType type, int stacks, int charges)
                : base(id, name, spellName, range, duration, type)
            {
                Stacks = stacks;
                Charges = charges;
            }

            public ExpandedWardItem(SAssemblies.Ward.WardItem ward, int stacks, int charges)
                : base(ward.Id, ward.Name, ward.SpellName, ward.Range, ward.Duration, ward.Type)
            {
                Stacks = stacks;
                Charges = charges;
            }
        }

        public FowWardPlacement()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    List<ExpandedWardItem> wards = new List<ExpandedWardItem>();
					wards = GetWardItems(hero);
                    _wards.Add(hero, wards);
                }
            }
            //Game.OnGameUpdate += Game_OnGameUpdate;
            ThreadHelper.GetInstance().Called += Game_OnGameUpdate;
        }

        ~FowWardPlacement()
        {
            //Game.OnGameUpdate -= Game_OnGameUpdate;
            ThreadHelper.GetInstance().Called -= Game_OnGameUpdate;
            enemiesUsed = null;
            enemiesRefilled = null;
        }

        public bool IsActive()
        {
            return Ward.Wards.GetActive() && FowWardPlacementWard.GetActive();
        }

        private static void SetupMainMenu()
        {
            var menu = new LeagueSharp.Common.Menu("SAssembliesFowWardPlacement", "SAssembliesWardsFowWardPlacement", true);
            SetupMenu(menu);
            menu.AddToMainMenu();
        }

        public static Menu.MenuItemSettings SetupMenu(LeagueSharp.Common.Menu menu)
        {
            FowWardPlacementWard.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu(Language.GetString("WARDS_FOWWARDPLACEMENT_MAIN"), "SAssembliesWardsFowWardPlacement"));
            FowWardPlacementWard.MenuItems.Add(
                FowWardPlacementWard.Menu.AddItem(new MenuItem("SAssembliesWardsFowWardPlacementActive", Language.GetString("GLOBAL_ACTIVE")).SetValue(false)));
            return FowWardPlacementWard;
        }
		
		private void Game_OnGameUpdate(object sender, EventArgs args)
        {
            if (!IsActive() || lastGameUpdateTime + new Random().Next(500, 1000) > Environment.TickCount)
                return;

            lastGameUpdateTime = Environment.TickCount;

            foreach (var enemy in _wards.ToArray())
            {
                Obj_AI_Hero hero = enemy.Key;
				List<ExpandedWardItem> allWards = new List<ExpandedWardItem>(GetWardItems(hero));
				
				List<ExpandedWardItem> soldWards = allWards.Except(enemy.Value);
                foreach (var wardItem in soldWards)
                {
                    Game.PrintChat("{0} has used {1}", enemy.Key.ChampionName, wardItem.Name);
                }
				
				List<ExpandedWardItem> boughtWards = enemy.Value.Except(allWards);
				foreach (var wardItem in boughtWards)
                {
                    Game.PrintChat("{0} got {1}", enemy.Key.ChampionName, wardItem.Name);
                }
				
				foreach (var item in allWards)
				{
					foreach (var wardItem in enemy.Value.ToArray())
					{
						if (item.Id == wardItem.Id && wardItem.Type != SAssemblies.Ward.WardType.Temp && wardItem.Type != SAssemblies.Ward.WardType.TempVision && hero.Spellbook.CanUseSpell(item.SpellSlot) == SpellState.Ready)
                        {
                            if (item.Charges > 0 ? item.Charges >= wardItem.Charges : false || item.Stacks >= wardItem.Stacks) //Check for StackItems etc fail
                            {
                                Game.PrintChat("{0} has used {1}", enemy.Key.ChampionName, wardItem.Name);
                            }
                        }
					}
				}

				foreach (var item in hero.InventoryItems)
                {
                    foreach (var wardItem in enemy.Value.ToArray())
                    {
                        if ((int)item.Id == wardItem.Id && (item.Charges > wardItem.Charges || item.Stacks > wardItem.Stacks) &&
                            wardItem.Type != SAssemblies.Ward.WardType.Temp && wardItem.Type != SAssemblies.Ward.WardType.TempVision && hero.Spellbook.CanUseSpell(item.SpellSlot) == SpellState.Ready)
                        {
                            Game.PrintChat("{0} got {1}", enemy.Key.ChampionName, wardItem.Name);
                        }
                    }
                    foreach (var ward in SAssemblies.Ward.WardItems)
                    {
                        if ((int)item.Id == ward.Id && ward.Type != SAssemblies.Ward.WardType.Temp && ward.Type != SAssemblies.Ward.WardType.TempVision && hero.Spellbook.CanUseSpell(item.SpellSlot) == SpellState.Ready &&
                            (enemy.Value.Find(wardItem => wardItem.Id == ward.Id) == null))
                        {
                            Game.PrintChat("{0} got {1}", enemy.Key.ChampionName, wardItem.Name);
                        }
                    }
                }				

				enemy.Value = allWards;
            }
        }
		
		private List<ExpandedWardItem> GetWardItems(Obj_AI_Hero hero)
        {
            List<ExpandedWardItem> wards = new List<ExpandedWardItem>();
            foreach (var item in hero.InventoryItems)
            {
                foreach (var wardItem in SAssemblies.Ward.WardItems)
                {
                    if ((int)item.Id == wardItem.Id && wardItem.Type != SAssemblies.Ward.WardType.Temp && wardItem.Type != SAssemblies.Ward.WardType.TempVision)
                    {
                        wards.Add(new ExpandedWardItem(wardItem, item.Stacks, item.Charges));
                    }
                }
            }
            return wards;
        }
    }
}
