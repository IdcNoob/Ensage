using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

namespace HpMpAbuse {
	class Program {

		private static bool _enabled = false;

		private static bool _stopAttack = true;

		private static int _lastPtState = 0;
		private static bool _ptChanged = false;
	
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

			var items = hero.Inventory.Items.ToList();

			foreach (var item in items.Where(item => item.AbilityData.Any(x => (x.Name == "bonus_intellect" || x.Name == "bonus_strength" || x.Name == "bonus_all_stats" || x.Name == "bonus_health" || x.Name == "bonus_mana")))) {
				if (arcaneBoots != null && item.Equals(arcaneBoots) && arcaneBoots.CanBeCasted())
					continue;
				if (stick != null && item.Equals(stick) && stick.CanBeCasted() && stick.CurrentCharges != 0)
					continue;

				hero.DropItem(item, hero.NetworkPosition);
			}

			if (arcaneBoots != null && arcaneBoots.CanBeCasted()) {
				arcaneBoots.UseAbility();
				hero.DropItem(arcaneBoots, hero.NetworkPosition);
				Utils.Sleep(100, "delay");
				return;
			}

			if (powerTreads != null && !_ptChanged) {
				var sr = soulRing != null && soulRing.CanBeCasted();

				switch (((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute) {
					case Ensage.Attribute.Intelligence: // agi
						if (sr) {
							powerTreads.ToggleAbility(true);
							soulRing.UseAbility();
							powerTreads.ToggleAbility(true);
							powerTreads.ToggleAbility(true);
						}
						_lastPtState = 0;
					break;
					case Ensage.Attribute.Strength:
						if (sr) {
							soulRing.UseAbility();
						}
						powerTreads.ToggleAbility(true);
						powerTreads.ToggleAbility(true);
						_lastPtState = 1;
					break;
					case Ensage.Attribute.Agility:  // int
						if (sr) {
							powerTreads.ToggleAbility(true);
							powerTreads.ToggleAbility(true);
							soulRing.UseAbility();
							powerTreads.ToggleAbility(true);
							powerTreads.ToggleAbility(true);
						}
						else {
							powerTreads.ToggleAbility(true);
						}
						_lastPtState = 2;
					break;
				}
				_ptChanged = true;
			}

			if (soulRing != null && soulRing.CanBeCasted() && powerTreads == null)
				soulRing.UseAbility();
				
			if (bottle != null && bottle.CanBeCasted() && bottle.CurrentCharges != 0 && hero.Modifiers.All(x => x.Name != "modifier_bottle_regeneration"))
				bottle.UseAbility();

			if (stick != null && stick.CanBeCasted() && stick.CurrentCharges != 0)
				stick.UseAbility();

			Utils.Sleep(250, "delay");
		}

		private static void PickUpItems() {

			var hero = ObjectMgr.LocalHero;

			var droppedItems = ObjectMgr.GetEntities<PhysicalItem>().Where(x => x.Distance2D(hero) < 250).ToList();

			foreach (var item in droppedItems) {
				hero.PickUpItem(item);
			}

			var powerTreads = hero.FindItem("item_power_treads");

			if (powerTreads != null && _ptChanged) {
				for (var i = 0; i < _lastPtState; i++) {
					powerTreads.ToggleAbility(true);
				}
				_ptChanged = false;
			}

		}

	}
}
