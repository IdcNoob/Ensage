using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System.Windows.Input;

namespace PTSwitcher {
	class Program {

		private static Ensage.Attribute _lastPtState;
		private static bool _ptChanged;
		private static bool _regen;

		private static readonly string[] IgnoredSpells = {
			"item_tpscroll",
			"item_travel_boots",
			"item_travel_boots_2",
			"legion_commander_duel",
			"clinkz_searing_arrows"
		};

		private static readonly string[] HealModifiers = {
			"modifier_item_urn_heal",
			"modifier_flask_healing",
			"modifier_bottle_regeneration"
		};

		private static void Main(string[] args) {
			Player.OnExecuteOrder += Player_OnExecuteAction;
			Game.OnIngameUpdate += Game_OnIngameUpdate;
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

		private static void CastSpell(Ability spell, ExecuteOrderEventArgs args) {

			var hero = ObjectMgr.LocalHero;

			if (hero == null || spell == null || !hero.IsAlive || !hero.CanCast() || spell.ManaCost <= 5 || IgnoredSpells.Any(spell.Name.Contains))
				return;

			var powerTreads = hero.FindItem("item_power_treads");

			if (powerTreads == null || ((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute == Ensage.Attribute.Agility) { // INT
				return;
			}
			args.Process = false;

			var sleep = spell.FindCastPoint() * 1000 + 800;

			if (spell.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget)) {
				ChangePt(powerTreads, Ensage.Attribute.Intelligence);
				spell.UseAbility();
			}
			else {
				var target = (Unit) args.Target;
				if (target != null && target.IsAlive) {

					//var castRange = spell.CastRange + 35;

					//if (spell.Name == "dragon_knight_dragon_tail" && hero.Modifiers.Any(x => x.Name == "modifier_dragon_knight_dragon_form"))
					//	castRange = 435;
					
					

					//if (hero.Distance2D(target) <= castRange || spell.CastRange == 0) {
						ChangePt(powerTreads, Ensage.Attribute.Intelligence);
					//}
					spell.UseAbility(target);
				}
				else {

					if (spell.Name == "phantom_lancer_doppelwalk") {
						sleep += 1000;
					}
					//if (hero.Distance2D(Game.MousePosition) <= spell.CastRange + 35 || spell.CastRange == 0) {
						ChangePt(powerTreads, Ensage.Attribute.Intelligence);
					//}
					spell.UseAbility(Game.MousePosition);
				}
				sleep += hero.GetTurnTime(Game.MousePosition) * 1000;
			}

			Utils.Sleep(sleep, "delay");

		}

		static void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args) {
			if (args.Order == Order.AbilityTarget || args.Order == Order.AbilityLocation || args.Order == Order.Ability) {
				if (!Game.IsKeyDown(16))
					CastSpell(args.Ability, args);
			}
		}

		private static void Game_OnIngameUpdate(EventArgs args) {
			if (Game.IsPaused || !Game.IsInGame || !Utils.SleepCheck("delay"))
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null || !hero.IsAlive)
				return;

			var powerTreads = hero.FindItem("item_power_treads");

			if (powerTreads == null)
				return;

			if (!_regen && !_ptChanged) {
				switch (((Ensage.Items.PowerTreads) powerTreads).ActiveAttribute) {
					case Ensage.Attribute.Intelligence: // agi
					_lastPtState = Ensage.Attribute.Agility;
					break;
					case Ensage.Attribute.Strength:
					_lastPtState = Ensage.Attribute.Strength;
					break;
					case Ensage.Attribute.Agility: // int
					_lastPtState = Ensage.Attribute.Intelligence;
					break;
				}
			}

			if (hero.Modifiers.Any(x => HealModifiers.Any(x.Name.Contains))) {
				_regen = true;
				ChangePt(powerTreads, Ensage.Attribute.Agility);
			}
			else {
				_regen = false;
			}

			if (_ptChanged && !hero.IsChanneling() && !hero.IsInvisible() && !_regen && hero.CanMove()) {

				foreach (var spell in hero.Spellbook.Spells.Where(spell => spell.IsInAbilityPhase)) {
					Utils.Sleep(spell.FindCastPoint() * 1000 + 300, "delay");
					return;
				}

				ChangePt(powerTreads, _lastPtState);
				_ptChanged = false;
			}

			Utils.Sleep(200, "delay");
		}

		private static void ChangePt(Ensage.Item pt, Ensage.Attribute atrb) {
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

			if (ptNow == 2 && ptTo == 1)
				change = 2;

			_ptChanged = true;

			for (var i = 0; i < change; i++)
				pt.UseAbility();

		}
	}
}
