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

		private static Ensage.Attribute _lastPtState = 0;
		private static bool _ptChanged = false;

		private static readonly string[] bonusHealth = { "bonus_strength", "bonus_all_stats", "bonus_health" };
		private static readonly string[] bonusMana = {"bonus_intellect", "bonus_all_stats", "bonus_mana"};

		private static void Main() {
			Game.OnUpdate += Game_OnUpdate;
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

		private static void Game_OnUpdate(EventArgs args) {

			if (!Game.IsInGame || !_enabled || Game.IsPaused || !Utils.SleepCheck("delay"))
				return;

			var player = ObjectMgr.LocalPlayer;
			if (player == null || player.Team == Team.Observer)
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null)
				return;

			if (hero.Mana == hero.MaximumMana && hero.Health == hero.MaximumHealth)
				return;

			if (hero.NetworkActivity == NetworkActivity.Move || ObjectMgr.GetEntities<Hero>().Any(x => x.IsAlive && x.IsVisible && x.Team == hero.GetEnemyTeam() && x.Distance2D(hero) < 400)) {
				PickUpItems();
				Utils.Sleep(1000, "delay");
				return;
			}

			var arcaneBoots = hero.FindItem("item_arcane_boots") ?? hero.FindItem("item_guardian_greaves");
			var soulRing = hero.FindItem("item_soul_ring");
			var bottle = hero.FindItem("item_bottle");
			var stick = hero.FindItem("item_magic_stick") ?? hero.FindItem("item_magic_wand");
			var powerTreads = hero.FindItem("item_power_treads");
			var meka = hero.FindItem("item_mekansm");
			var urn = hero.FindItem("item_urn_of_shadows");

			if (powerTreads != null && !_ptChanged) {
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

			if (meka != null && meka.CanBeCasted() && hero.Health / hero.MaximumHealth < 0.8) {
				if (powerTreads != null)
					ChangePt(powerTreads, Ensage.Attribute.Intelligence);
				DropItems(bonusHealth, meka);
				meka.UseAbility(true);
			}

			if (arcaneBoots != null && arcaneBoots.CanBeCasted() && hero.Mana != hero.MaximumHealth) {
				if (powerTreads != null)
					ChangePt(powerTreads, Ensage.Attribute.Agility);
				DropItems(bonusMana, arcaneBoots);
				arcaneBoots.UseAbility(true);
			}

			if (soulRing != null && soulRing.CanBeCasted()) {
				if (powerTreads != null)
					ChangePt(powerTreads, Ensage.Attribute.Strength);
				DropItems(bonusMana);
				soulRing.UseAbility();
			}

			if (bottle != null && bottle.CanBeCasted() && bottle.CurrentCharges != 0 && hero.Modifiers.All(x => x.Name != "modifier_bottle_regeneration")) {
				if (powerTreads != null)
					ChangePt(powerTreads, Ensage.Attribute.Agility);

				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(bonusHealth);

				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(bonusMana);

				bottle.UseAbility();
			}

			if (stick != null && stick.CanBeCasted() && stick.CurrentCharges != 0) {
				if (powerTreads != null)
					ChangePt(powerTreads, Ensage.Attribute.Agility);
	
				if (hero.Health / hero.MaximumHealth < 0.9)
					DropItems(bonusHealth, stick);

				if (hero.Mana / hero.MaximumMana < 0.9)
					DropItems(bonusMana, stick);

				stick.UseAbility();
			}

			if (urn != null && hero.Health/hero.MaximumHealth < 0.9) {
				if (powerTreads != null)
					ChangePt(powerTreads, Ensage.Attribute.Agility);
				
				if (urn.CanBeCasted() && urn.CurrentCharges != 0 && hero.Modifiers.All(x => x.Name != "modifier_item_urn_heal"))
					DropItems(bonusHealth, urn);
				else
					DropItems(bonusHealth);

				urn.UseAbility(hero);
			}

			var allies = ObjectMgr.GetEntities<Hero>().Where(x => x.Distance2D(hero) <= 900 && x.IsAlive && x.Team == hero.Team);

			foreach (var ally in allies) {

				var allyArcaneBoots = ally.FindItem("item_arcane_boots");
				var allyGreaves = ally.FindItem("item_guardian_greaves");
				var allyMeka = ally.FindItem("item_mekansm");

				if (allyArcaneBoots != null && allyArcaneBoots.AbilityState == AbilityState.Ready) {
					if (powerTreads != null)
						ChangePt((Ensage.Items.PowerTreads) powerTreads, Ensage.Attribute.Agility);
					DropItems(bonusMana);
				}

				if (allyGreaves != null && allyGreaves.AbilityState == AbilityState.Ready) {
					if (powerTreads != null)
						ChangePt((Ensage.Items.PowerTreads) powerTreads, Ensage.Attribute.Agility);
					DropItems(bonusMana.Concat(bonusHealth));
				}

				if (allyMeka != null && allyMeka.AbilityState == AbilityState.Ready) {
					if (powerTreads != null)
						ChangePt((Ensage.Items.PowerTreads) powerTreads, Ensage.Attribute.Agility);
					DropItems(bonusHealth);
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
			var ptNow = 0; //(int) pt.ActiveAttribute + 1;
			var ptTo = 0; //(int) atrb + 1;

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
			
			var to = ptTo - ptNow % 3;

			if (ptNow == 2 && ptTo == 1) // another fix
				to = 2;

			for (var i = 0; i < to; i++) {
				pt.ToggleAbility(true);
			}

			_ptChanged = true;

		}

		private static void PickUpItems() {

			var hero = ObjectMgr.LocalHero;

			if (hero == null)
				return;

			var droppedItems = ObjectMgr.GetEntities<PhysicalItem>().Where(x => x.Distance2D(hero) < 250).ToList();

			foreach (var item in droppedItems) {
				hero.PickUpItem(item, true);
			}

			if (!_ptChanged)
				return;

			ChangePt(hero.FindItem("item_power_treads"), _lastPtState);
			_ptChanged = false;
		}

	}
}
