using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

namespace HpMpAbuse {
	class Program {

		private const int WM_KEYDOWN = 0x0100;

		private static bool _enabled = false;

		private static bool _stopAttack = true;

		//private static int lastPtState = 0;

		private static readonly string[] BonusMotherFuckingItems = {  // dont fucking know how to make it ez in e#
			"item_branches",
			"item_arcane_boots",
			"item_magic_wand",
			"item_belt_of_strength",
			"item_circlet",
			"item_energy_booster",
			"item_gauntlets",
			"item_ghost",
			"item_mantle",
			"item_mystic_staff",
			"item_ogre_axe",
			"item_point_booster",
			"item_reaver",
			"item_robe",
			"item_staff_of_wizardry",
			"item_ultimate_orb",
			"item_vitality_booster",
			"item_ultimate_scepter",
			"item_black_king_bar",
			"item_bloodstone",
			"item_bracer",
			"item_crimson_guard",
			"item_dagon_1",
			"item_dagon_2",
			"item_dagon_3",
			"item_dagon_4",
			"item_dagon_5",
			"item_diffusal_blade_1",
			"item_diffusal_blade_2",
			"item_cyclone",
			"item_ancient_janggo",
			"item_skadi",
			"item_force_staff",
			"item_heavens_halberd",
			"item_sphere",
			"item_manta",
			"item_guardian_greaves",
			"item_necronomicon_1",
			"item_necronomicon_2",
			"item_necronomicon_3",
			"item_null_talisman",
			"item_oblivion_staff",
			"item_octarine_core",
			"item_orchid",
			"item_ring_of_aquila",
			"item_rod_of_atos",
			"item_sange",
			"item_sange_and_yasha",
			"item_satanic",
			"item_sheepstick",
			"item_shivas_guard",
			"item_soul_booster",
			"item_urn_of_shadows",
			"item_vanguard",
			"item_veil_of_discord",
			"item_wraith_band",
		};

		private static void Main() {
			Game.OnUpdate += Game_OnUpdate;
			Game.OnWndProc += Game_OnWndProc;
		}
		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == 'T' && !Game.IsChatOpen) {
				_enabled = args.Msg == WM_KEYDOWN ? true : false;

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


			var arcaneBoots = hero.FindItem("item_arcane_boots");
			var soulRing = hero.FindItem("item_soul_ring");
			var bottle = hero.FindItem("item_bottle");
			var stick = hero.Inventory.Items.FirstOrDefault(x => x.Name.Substring(0, 11) == "item_magic_");

			//var powerTreads = hero.FindItem("item_power_treads");

			//if (powerTreads != null) {
			//	switch (((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute) {
			//		case Attribute.Intelligence:
			//			powerTreads.ToggleAbility();
			//			lastPtState = 2;
			//			break;
			//		case Attribute.Strength:
			//			powerTreads.UseAbility();
			//			powerTreads.ToggleAbility(true);
			//			lastPtState = 1;
			//			break;
			//		case Attribute.Agility:
			//			lastPtState = 0;
			//			break;
			//	}
			//}

			var items = hero.Inventory.Items.ToList();

			foreach (var item in items.Where(item => BonusMotherFuckingItems.Any(item.Name.Contains))) {
				if (item.Equals(arcaneBoots) && arcaneBoots.CanBeCasted())
					continue;
				if (item.Equals(stick) && stick.CanBeCasted() && stick.CurrentCharges != 0)
					continue;

				hero.DropItem(item, hero.NetworkPosition);
			}

			if (arcaneBoots != null && arcaneBoots.CanBeCasted()) {
				arcaneBoots.UseAbility();
				return;
			}

			if (soulRing != null && soulRing.CanBeCasted())
				soulRing.UseAbility();

			if (bottle != null && bottle.CanBeCasted() && bottle.CurrentCharges != 0 && hero.Modifiers.All(x => x.Name != "modifier_bottle_regeneration"))
				bottle.UseAbility();

			if (stick != null && stick.CanBeCasted() && stick.CurrentCharges != 0)
				stick.UseAbility();

			Utils.Sleep(250, "delay");
		}

		private static void PickUpItems() {
		
			var hero = ObjectMgr.LocalHero;

			var droppedItems = ObjectMgr.GetEntities<PhysicalItem>().Where(x => x.Distance2D(hero) < 200).ToList();

			foreach (var item in droppedItems) {
				hero.PickUpItem(item, true);
			}

			//var powerTreads = hero.FindItem("item_power_treads");

			//if (powerTreads != null) {
			//	for (int i = 0; i < lastPtState; i++) {
			//		powerTreads.ToggleAbility(true);
			//	}
			//}

		}

	}
}