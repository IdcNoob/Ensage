using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Items;
using Attribute = Ensage.Attribute;

namespace HpMpAbuse {
	internal class Program {

		private static bool enabled;
		private static bool stopAttack = true;

		private static Attribute lastPtState;
		private static bool ptChanged;

		private static readonly string[] BonusHealth = {"bonus_strength", "bonus_all_stats", "bonus_health"};
		private static readonly string[] BonusMana = {"bonus_intellect", "bonus_all_stats", "bonus_mana"};

		private static readonly Menu Menu = new Menu("HP/MP Recovery Abuse", "hpmp", true);

		private static void Main() {
			Game.OnUpdate += Game_OnUpdate;
			Game.OnWndProc += Game_OnWndProc;

			Menu.AddItem(new MenuItem("hotkey", "Change hotkey").SetValue(new KeyBind('T', KeyBindType.Press)));

			var forcePick = new Menu("Force item picking", "forcePick");

			forcePick.AddItem(new MenuItem("moved", "When moved").SetValue(true));
			forcePick.AddItem(new MenuItem("enemyNear", "When enemy near").SetValue(true));

			Menu.AddSubMenu(forcePick);
			Menu.AddToMainMenu();
		}

		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == Menu.Item("hotkey").GetValue<KeyBind>().Key && !Game.IsChatOpen) {
				enabled = args.Msg == (uint) Utils.WindowsMessages.WM_KEYDOWN;

				if (stopAttack) {
					Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 0");
					stopAttack = false;
				}

				if (!enabled) {
					PickUpItems();
					Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 1");
					stopAttack = true;
				}
			}
		}

		private static void Game_OnUpdate(EventArgs args) {
			if (Game.IsPaused || !Game.IsInGame || !enabled || !Utils.SleepCheck("delay"))
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null || !hero.IsAlive)
				return;

			if (hero.Mana == hero.MaximumMana && hero.Health == hero.MaximumHealth)
				return;

			if ((hero.NetworkActivity == NetworkActivity.Move && Menu.Item("moved").GetValue<bool>()) ||
			    (ObjectMgr.GetEntities<Hero>()
				    .Any(x => x.IsAlive && x.IsVisible && x.Team == hero.GetEnemyTeam() && x.Distance2D(hero) < 400) &&
			     Menu.Item("enemyNear").GetValue<bool>())) {
				PickUpItems();
				Utils.Sleep(1000, "delay");
				return;
			}

			var arcaneBoots = hero.FindItem("item_arcane_boots");
			var greaves = hero.FindItem("item_guardian_greaves");
			var soulRing = hero.FindItem("item_soul_ring");
			var bottle = hero.FindItem("item_bottle");
			var stick = hero.FindItem("item_magic_stick") ?? hero.FindItem("item_magic_wand");
			var powerTreads = hero.FindItem("item_power_treads");
			var meka = hero.FindItem("item_mekansm");
			var urn = hero.FindItem("item_urn_of_shadows");

			if (powerTreads != null && !ptChanged) {
				//_lastPtState = ((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute;
				switch (((PowerTreads) powerTreads).ActiveAttribute) {
					case Attribute.Intelligence: // agi
						lastPtState = Attribute.Agility;
						break;
					case Attribute.Strength:
						lastPtState = Attribute.Strength;
						break;
					case Attribute.Agility: // int
						lastPtState = Attribute.Intelligence;
						break;
				}
			}

			if (meka != null && meka.CanBeCasted() && hero.Health != hero.MaximumHealth) {
				ChangePt(powerTreads, Attribute.Intelligence);
				DropItems(BonusHealth, meka);
				meka.UseAbility(true);
			}

			if (arcaneBoots != null && arcaneBoots.CanBeCasted() && hero.Mana != hero.MaximumMana) {
				ChangePt(powerTreads, Attribute.Agility);
				DropItems(BonusMana, arcaneBoots);
				arcaneBoots.UseAbility(true);
			}

			if (greaves != null && greaves.CanBeCasted()) {
				ChangePt(powerTreads, Attribute.Agility);
				DropItems(BonusHealth.Concat(BonusMana), greaves);
				greaves.UseAbility(true);
			}

			if (soulRing != null && soulRing.CanBeCasted()) {
				ChangePt(powerTreads, Attribute.Strength);
				DropItems(BonusMana);
				soulRing.UseAbility(true);
			}

			if (bottle != null && bottle.CanBeCasted() && bottle.CurrentCharges != 0 &&
			    hero.Modifiers.All(x => x.Name != "modifier_bottle_regeneration")) {
				ChangePt(powerTreads, Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(BonusHealth);
				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(BonusMana);

				bottle.UseAbility(true);
			}

			if (stick != null && stick.CanBeCasted() && stick.CurrentCharges != 0) {
				ChangePt(powerTreads, Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(BonusHealth, stick);
				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(BonusMana, stick);

				stick.UseAbility(true);
			}

			if (urn != null && urn.CanBeCasted() && urn.CurrentCharges != 0 &&
			    hero.Modifiers.All(x => x.Name != "modifier_item_urn_heal") && hero.Health / hero.MaximumHealth < 0.9) {
				ChangePt(powerTreads, Attribute.Agility);
				DropItems(BonusHealth, urn);
				urn.UseAbility(hero, true);
			}

			if (hero.Modifiers.Any(x => (x.Name == "modifier_item_urn_heal" || x.Name == "modifier_flask_healing"))) {
				ChangePt(powerTreads, Attribute.Agility);
				DropItems(BonusHealth);
			}

			if (hero.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration")) {
				ChangePt(powerTreads, Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(BonusHealth);
				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(BonusMana);
			}

			var allies = ObjectMgr.GetEntities<Hero>().Where(x => x.Distance2D(hero) <= 900 && x.IsAlive && x.Team == hero.Team);

			foreach (var ally in allies) {
				var allyArcaneBoots = ally.FindItem("item_arcane_boots");
				var allyMeka = ally.FindItem("item_mekansm");
				var allyGreaves = ally.FindItem("item_guardian_greaves");

				if (allyArcaneBoots != null && allyArcaneBoots.AbilityState == AbilityState.Ready) {
					ChangePt((PowerTreads) powerTreads, Attribute.Agility);
					DropItems(BonusMana);
				}

				if (allyMeka != null && allyMeka.AbilityState == AbilityState.Ready) {
					ChangePt((PowerTreads) powerTreads, Attribute.Agility);
					DropItems(BonusHealth);
				}

				if (allyGreaves != null && allyGreaves.AbilityState == AbilityState.Ready) {
					ChangePt((PowerTreads) powerTreads, Attribute.Agility);
					DropItems(BonusMana.Concat(BonusHealth));
				}
			}

			Utils.Sleep(250, "delay");
		}

		private static void DropItems(IEnumerable<string> drop, Item ignore = null) {
			var hero = ObjectMgr.LocalHero;
			var items = hero.Inventory.Items.ToList();

			foreach (
				var item in items.Where(item => !item.Equals(ignore) && item.AbilityData.Any(x => drop.Any(x.Name.Contains))))
				hero.DropItem(item, hero.NetworkPosition, true);
		}

		private static void ChangePt(Item pt, Attribute atrb) {
			// okay this shit function is really bad
			if (pt == null)
				return;

			//var ptNow = (int) ((Ensage.Items.PowerTreads) pt).ActiveAttribute + 1;
			//var ptTo = (int) atrb + 1;

			var ptNow = 0;
			var ptTo = 0;

			//some random fixes
			switch (((PowerTreads) pt).ActiveAttribute) {
				case Attribute.Intelligence: // agi
					ptNow = 3;
					break;
				case Attribute.Strength:
					ptNow = 1;
					break;
				case Attribute.Agility: // int
					ptNow = 2;
					break;
			}

			// more fixes
			switch (atrb) {
				case Attribute.Intelligence:
					ptTo = 2;
					break;
				case Attribute.Strength:
					ptTo = 1;
					break;
				case Attribute.Agility:
					ptTo = 3;
					break;
			}

			if (ptNow == ptTo)
				return;

			var change = ptTo - ptNow % 3;

			if (ptNow == 2 && ptTo == 1) // another fix
				change = 2;

			for (var i = 0; i < change; i++)
				pt.ToggleAbility(true);

			ptChanged = true;
		}

		private static void PickUpItems() {
			var hero = ObjectMgr.LocalHero;

			if (hero == null)
				return;

			var droppedItems = ObjectMgr.GetEntities<PhysicalItem>().Where(x => x.Distance2D(hero) < 250).ToList();

			for (var i = 0; i < droppedItems.Count; i++)
				hero.PickUpItem(droppedItems[i], i != 0);

			if (!ptChanged)
				return;

			ChangePt(hero.FindItem("item_power_treads"), lastPtState);
			ptChanged = false;
		}
	}
}