using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Items;
using Attribute = Ensage.Attribute;

namespace PTSwitcher {
	internal class Program {

		private static Attribute _lastPtState;
		private static bool _ptChanged;
		private static bool _healActive;
		private static bool _disableSwitchBack;

		private static readonly string[] IgnoredSpells = {
			"item_tpscroll",
			"item_travel_boots",
			"item_travel_boots_2",
			"brewmaster_storm_dispel_magic",
			"brewmaster_storm_cyclone",
			"brewmaster_storm_wind_walk",
			"legion_commander_duel",
			"clinkz_searing_arrows",
			"pudge_dismember",
			"chaos_knight_phantasm",
			"drow_ranger_frost_arrows",
			"lion_mana_drain",
			"ogre_magi_unrefined_fireblast", // aghanim stun
			"templar_assassin_meld",
			"tusk_walrus_punch",
			"item_invis_sword", //shadow blade
			"item_silver_edge"
		};

		private static readonly string[] HealModifiers = {
			"modifier_item_urn_heal",
			"modifier_flask_healing",
			"modifier_bottle_regeneration",
			"modifier_voodoo_restoration_heal"
		};

		private static readonly string[] DisableSwitchBackModifiers = {
			"modifier_leshrac_pulse_nova",
			"modifier_morphling_morph_agi",
			"modifier_morphling_morph_str",
			"modifier_voodoo_restoration_aura",
			"modifier_brewmaster_primal_split"
		};

		private static void Main(string[] args) {
			Player.OnExecuteOrder += Player_OnExecuteAction;
			Game.OnUpdate += Game_OnUpdate;
			//Game.OnWndProc += Game_OnWndProc;
		}

		//private static void Game_OnWndProc(WndEventArgs args) {
		//	if (!Game.IsInGame || Game.IsPaused || Game.IsChatOpen || Game.IsKeyDown(16) || Game.IsKeyDown(17) || args.Msg == (uint) Utils.WindowsMessages.WM_KEYUP)
		//		return;

		//switch (args.WParam) {
		//	case 'Q':
		//		CastSpell(ObjectMgr.LocalHero.Spellbook.SpellQ, args);
		//		break;
		//	case 'W':
		//		CastSpell(ObjectMgr.LocalHero.Spellbook.SpellW, args);
		//		break;
		//	case 'E':
		//		CastSpell(ObjectMgr.LocalHero.Spellbook.SpellE, args);
		//		break;
		//	case 'D':
		//		CastSpell(ObjectMgr.LocalHero.Spellbook.SpellD, args);
		//		break;
		//	case 'F':
		//		CastSpell(ObjectMgr.LocalHero.Spellbook.SpellF, args);
		//		break;
		//	case 'R':
		//		CastSpell(ObjectMgr.LocalHero.Spellbook.SpellR, args);
		//		break;
		//}
		//}

		private static void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args) {
			if (args.Order == Order.AbilityTarget || args.Order == Order.AbilityLocation || args.Order == Order.Ability ||
			    args.Order == Order.ToggleAbility)
				if (!Game.IsKeyDown(16))
					CastSpell(sender.Hero, args);
		}

		private static void CastSpell(Unit hero, ExecuteOrderEventArgs args) {

			var spell = args.Ability;

			if (hero == null || spell == null || spell.ManaCost <= 25 || IgnoredSpells.Any(spell.Name.Contains))
				return;

			var powerTreads = hero.FindItem("item_power_treads");

			if (powerTreads == null || ((PowerTreads) powerTreads).ActiveAttribute == Attribute.Agility) // INT
				return;

			args.Process = false;

			var sleep = spell.FindCastPoint() * 1000 + 500;

			switch (args.Order) {
				case Order.AbilityTarget: {
					var target = (Unit) args.Target;
					if (target != null && target.IsAlive) {

						var castRange = spell.CastRange + 300;

						if (spell.Name == "dragon_knight_dragon_tail" &&
						    hero.Modifiers.Any(x => x.Name == "modifier_dragon_knight_dragon_form"))
							castRange += 350;

						if (hero.Distance2D(target) <= castRange || castRange == 300) {
							ChangePt(powerTreads, Attribute.Intelligence);
							sleep += hero.GetTurnTime(target) * 1000;
						}

						spell.UseAbility(target);

					}
					break;
				}
				case Order.AbilityLocation: {
					if (spell.Name == "phantom_lancer_doppelwalk")
						sleep += 1000;

					var castRange = spell.CastRange + 300;

					if (hero.Distance2D(Game.MousePosition) <= castRange + 300 || castRange == 300) {
						ChangePt(powerTreads, Attribute.Intelligence);
						sleep += hero.GetTurnTime(Game.MousePosition) * 1000;
					}

					spell.UseAbility(Game.MousePosition);

					break;
				}
				case Order.Ability: {
					ChangePt(powerTreads, Attribute.Intelligence);
					spell.UseAbility();
					break;
				}
				case Order.ToggleAbility: {
					ChangePt(powerTreads, Attribute.Intelligence);
					spell.ToggleAbility();
					break;
				}
			}

			Utils.Sleep(sleep, "delay");

		}

		private static void Game_OnUpdate(EventArgs args) {
			if (Game.IsPaused || !Game.IsInGame || !Utils.SleepCheck("delay"))
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null || !hero.IsAlive)
				return;

			var powerTreads = hero.FindItem("item_power_treads");

			if (powerTreads == null)
				return;

			if (!_healActive && !_ptChanged) {
				switch (((PowerTreads) powerTreads).ActiveAttribute) {
					case Attribute.Intelligence: // agi
						_lastPtState = Attribute.Agility;
						break;
					case Attribute.Strength:
						_lastPtState = Attribute.Strength;
						break;
					case Attribute.Agility: // int
						_lastPtState = Attribute.Intelligence;
						break;
				}
			}

			_disableSwitchBack = hero.Modifiers.Any(x => DisableSwitchBackModifiers.Any(x.Name.Contains));

			if (hero.Modifiers.Any(x => HealModifiers.Any(x.Name.Contains)) && !_disableSwitchBack) {
				_healActive = true;
				ChangePt(powerTreads, Attribute.Agility);
			} else {
				_healActive = false;
			}

			if (_ptChanged && !_healActive && !_disableSwitchBack) {

				foreach (var spell in hero.Spellbook.Spells.Where(spell => spell.IsInAbilityPhase)) {
					Utils.Sleep(spell.FindCastPoint() * 1000 + 300, "delay");
					return;
				}

				ChangePt(powerTreads, _lastPtState, false);
			}

			Utils.Sleep(200, "delay");
		}

		private static void ChangePt(Item pt, Attribute atrb, bool changed = true) {
			if (pt == null)
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null || hero.IsChanneling() || hero.IsInvisible() || !hero.CanUseItems())
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

			if (ptNow == 2 && ptTo == 1)
				change = 2;

			_ptChanged = changed;

			for (var i = 0; i < change; i++)
				pt.UseAbility();
		}
	}
}
