using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

namespace HpMpAbuse {
	class Program {

		private static bool _enabled = false;

		private static bool _stopAttack = true;

		private static Ensage.Attribute _lastPtState;
		private static bool _ptChanged = false;

		private static readonly string[] bonusHealth = { "bonus_strength", "bonus_all_stats", "bonus_health" };
		private static readonly string[] bonusMana = { "bonus_intellect", "bonus_all_stats", "bonus_mana" };

		private static void Main() {
			Game.OnIngameUpdate += Game_OnIngameUpdate;
			Game.OnWndProc += Game_OnWndProc;
		}

		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == 'T' && !Game.IsChatOpen) {
				_enabled = args.Msg == (uint) Utils.WindowsMessages.WM_KEYDOWN ? true : false;

				if (_stopAttack) {
					Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 0");
					_stopAttack = false;
				}

				if (!_enabled) {
					PickUpItems();
					Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 1");
					_stopAttack = true;
				}
			}
		}

		private static void Game_OnIngameUpdate(EventArgs args) {
			if (Game.IsPaused || !_enabled || !Utils.SleepCheck("delay"))
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null || !hero.IsAlive)
				return;

			if (hero.Mana == hero.MaximumMana && hero.Health == hero.MaximumHealth)
				return;

			if (hero.NetworkActivity == NetworkActivity.Move || ObjectMgr.GetEntities<Hero>().Any(x => x.IsAlive && x.IsVisible && x.Team == hero.GetEnemyTeam() && x.Distance2D(hero) < 400)) {
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

			if (powerTreads != null && !_ptChanged) {
				//_lastPtState = ((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute;
				switch (((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute) {
					case Ensage.Attribute.Intelligence: // agi
						_lastPtState = Ensage.Attribute.Agility;
					break;
					case Ensage.Attribute.Strength:
						_lastPtState = Ensage.Attribute.Strength;
					break;
					case Ensage.Attribute.Agility:  // int
						_lastPtState = Ensage.Attribute.Intelligence;
					break;
				}
			}

			if (meka != null && meka.CanBeCasted() && hero.Health != hero.MaximumHealth) {
				ChangePt(powerTreads, Ensage.Attribute.Intelligence);
				DropItems(bonusHealth, meka);
				meka.UseAbility(true);
			}

			if (arcaneBoots != null && arcaneBoots.CanBeCasted() && hero.Mana != hero.MaximumMana) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);
				DropItems(bonusMana, arcaneBoots);
				arcaneBoots.UseAbility(true);
			}

			if (greaves != null && greaves.CanBeCasted()) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);
				DropItems(bonusHealth.Concat(bonusMana), greaves);
				greaves.UseAbility(true);
			}

			if (soulRing != null && soulRing.CanBeCasted()) {
				ChangePt(powerTreads, Ensage.Attribute.Strength);
				DropItems(bonusMana);
				soulRing.UseAbility(true);
			}

			if (bottle != null && bottle.CanBeCasted() && bottle.CurrentCharges != 0 && hero.Modifiers.All(x => x.Name != "modifier_bottle_regeneration")) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(bonusHealth);
				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(bonusMana);

				bottle.UseAbility(true);
			}

			if (stick != null && stick.CanBeCasted() && stick.CurrentCharges != 0) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(bonusHealth, stick);
				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(bonusMana, stick);

				stick.UseAbility(true);
			}

			if (urn != null && urn.CanBeCasted() && urn.CurrentCharges != 0 && hero.Modifiers.All(x => x.Name != "modifier_item_urn_heal") && hero.Health / hero.MaximumHealth < 0.9) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);
				DropItems(bonusHealth, urn);
				urn.UseAbility(hero, true);
			}

			if (hero.Modifiers.Any(x => (x.Name == "modifier_item_urn_heal" || x.Name == "modifier_flask_healing"))) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);
				DropItems(bonusHealth);
			}

			if (hero.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration")) {
				ChangePt(powerTreads, Ensage.Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(bonusHealth);
				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(bonusMana);
			}

			var allies = ObjectMgr.GetEntities<Hero>().Where(x => x.Distance2D(hero) <= 900 && x.IsAlive && x.Team == hero.Team);

			foreach (var ally in allies) {
				var allyArcaneBoots = ally.FindItem("item_arcane_boots");
				var allyMeka = ally.FindItem("item_mekansm");
				var allyGreaves = ally.FindItem("item_guardian_greaves");

				if (allyArcaneBoots != null && allyArcaneBoots.AbilityState == AbilityState.Ready) {
					ChangePt((Ensage.Items.PowerTreads) powerTreads, Ensage.Attribute.Agility);
					DropItems(bonusMana);
				}

				if (allyMeka != null && allyMeka.AbilityState == AbilityState.Ready) {
					ChangePt((Ensage.Items.PowerTreads) powerTreads, Ensage.Attribute.Agility);
					DropItems(bonusHealth);
				}

				if (allyGreaves != null && allyGreaves.AbilityState == AbilityState.Ready) {
					ChangePt((Ensage.Items.PowerTreads) powerTreads, Ensage.Attribute.Agility);
					DropItems(bonusMana.Concat(bonusHealth));
				}
			}

			Utils.Sleep(250, "delay");
		}

		private static void DropItems(IEnumerable<string> drop, Ensage.Item ignore = null) {
			var hero = ObjectMgr.LocalHero;
			var items = hero.Inventory.Items.ToList();

			foreach (var item in items.Where(item => !item.Equals(ignore) && item.AbilityData.Any(x => drop.Any(x.Name.Contains)))) {
				hero.DropItem(item, hero.NetworkPosition, true);
			}
		}

		private static void ChangePt(Ensage.Item pt, Ensage.Attribute atrb) {  // okay this shit function is really bad
			if (pt == null)
				return;

			//var ptNow = (int) ((Ensage.Items.PowerTreads) pt).ActiveAttribute + 1;
			//var ptTo = (int) atrb + 1;

			var ptNow = 0;
			var ptTo = 0;

			//some random fixes
			switch (((Ensage.Items.PowerTreads) pt).ActiveAttribute) {
				case Ensage.Attribute.Intelligence: // agi
					ptNow = 3;
				break;
				case Ensage.Attribute.Strength:
					ptNow = 1;
				break;
				case Ensage.Attribute.Agility:  // int
					ptNow = 2;
				break;
			}

			// more fixes
			switch (atrb) {
				case Ensage.Attribute.Intelligence:
					ptTo = 2;
				break;
				case Ensage.Attribute.Strength:
					ptTo = 1;
				break;
				case Ensage.Attribute.Agility:
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

			_ptChanged = true;
		}

		private static void PickUpItems() {
			var hero = ObjectMgr.LocalHero;

			if (hero == null)
				return;

			var droppedItems = ObjectMgr.GetEntities<PhysicalItem>().Where(x => x.Distance2D(hero) < 250).ToList();

			for (var i = 0; i < droppedItems.Count; i++)
				hero.PickUpItem(droppedItems[i], i != 0);
			
			if (!_ptChanged)
				return;

			ChangePt(hero.FindItem("item_power_treads"), _lastPtState);
			_ptChanged = false;
		}

	}
}