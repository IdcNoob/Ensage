using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Items;
using SharpDX;
using SharpDX.Direct3D9;
using Attribute = Ensage.Attribute;

namespace HpMpAbuse {
    internal class Program {
        private static bool stopAttack = true;
        private static bool ptChanged, healActive, disableSwitchBack, autoDisablePT, attacking, enabledRecovery;
        private static bool inGame;

        private static Attribute lastPtAttribute;
        private static PowerTreads powerTreads;
        private static Hero hero;

        private static Font manaCheckText;
        private static int manaLeft;

        private static readonly string[] BonusHealth = {"bonus_strength", "bonus_health", "bonus_all_stats", "bonus_stats"};
        private static readonly string[] BonusMana = {"bonus_intellect", "bonus_mana", "bonus_all_stats", "bonus_stats"};

        private static readonly Menu Menu = new Menu("Smart HP/MP Abuse", "smartAbuse", true);

        private static readonly Menu PTMenu = new Menu("PT Switcher", "ptSwitcher");
        private static readonly Menu RecoveryMenu = new Menu("Recovery Abuse", "recoveryAbuse");
        private static readonly Menu SoulRingMenu = new Menu("Auto Soul Ring", "soulringAbuse");
        private static readonly Menu ManaCheckMenu = new Menu("Mana Combo Checker", "manaChecker");

        private static readonly Dictionary<string, bool> AbilitiesPT = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> AbilitiesSR = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> AbilitiesMC = new Dictionary<string, bool>();
        private static readonly Dictionary<ItemSlot, Item> ItemSlots = new Dictionary<ItemSlot, Item>();

        private static readonly string[] AttackSpells = {
            "windrunner_focusfire",
            "clinkz_searing_arrows",
            "silencer_glaives_of_wisdom",
            "templar_assassin_meld"
        };

        private static readonly string[] IgnoredSpells = {
            "item_tpscroll",
            "item_travel_boots",
            "item_travel_boots_2"
        };

        private static readonly string[] HealModifiers = {
            "modifier_item_urn_heal",
            "modifier_flask_healing",
            "modifier_bottle_regeneration",
            "modifier_voodoo_restoration_heal",
            "modifier_tango_heal",
            "modifier_enchantress_natures_attendants",
            "modifier_oracle_purifying_flames",
            "modifier_warlock_shadow_word",
            "modifier_treant_living_armor",
            "modifier_clarity_potion"
        };

        private static readonly string[] DisableSwitchBackModifiers = {
            "modifier_leshrac_pulse_nova",
            "modifier_morphling_morph_agi",
            "modifier_morphling_morph_str",
            "modifier_voodoo_restoration_aura",
            "modifier_brewmaster_primal_split",
            "modifier_eul_cyclone"
        };

        private static void Main() {
            RecoveryMenu.AddItem(new MenuItem("hotkey", "Change hotkey").SetValue(new KeyBind('T', KeyBindType.Press)));

            var forcePick = new Menu("Force item picking", "forcePick");

            forcePick.AddItem(new MenuItem("forcePickMoved", "When hero moved").SetValue(true));
            forcePick.AddItem(new MenuItem("forcePickEnemyNearDistance", "When enemy in range").SetValue(new Slider(500, 0,
                700)).SetTooltip("If enemy is closer then pick items"));

            RecoveryMenu.AddSubMenu(forcePick);

            PTMenu.AddItem(new MenuItem("enabledPT", "Enabled").SetValue(true));
            PTMenu.AddItem(new MenuItem("enabledPTAbilities", "Enabled for").SetValue(new AbilityToggler(AbilitiesPT)))
                .DontSave();
            PTMenu.AddItem(new MenuItem("switchPTonMove", "Switch when moving").SetValue(
                new StringList(new[] {"Don't switch", "Main attribute", "Strength", "Intelligence", "Agility"})))
                .SetTooltip("Switch PT to selected attribute when moving");
            PTMenu.AddItem(new MenuItem("switchPTonAttack", "Swtich when attacking").SetValue(
                new StringList(new[] {"Don't switch", "Main attribute", "Strength", "Intelligence", "Agility"})))
                .SetTooltip("Switch PT to selected attribute when attacking");
            PTMenu.AddItem(new MenuItem("switchPTHeal", "Swtich when healing").SetValue(true))
                .SetTooltip("Bottle, flask, clarity and other hero spells");
            PTMenu.AddItem(new MenuItem("manaPTThreshold", "Mana cost threshold").SetValue(new Slider(15, 0, 50))
                .SetTooltip("Don't switch PT if spell/item costs less mana"));
            PTMenu.AddItem(new MenuItem("switchbackPTdelay", "Switch back delay").SetValue(new Slider(500, 100, 1000))
                .SetTooltip("Make delay bigger if you have issues with PT when casting more than 1 spell in a row"));
            PTMenu.AddItem(new MenuItem("autoPTdisable", "Auto disable PT switcher").SetValue(new Slider(0, 0, 60))
                .SetTooltip("Auto disable PT switching after X min (always enabled: 0)"));

            SoulRingMenu.AddItem(new MenuItem("enabledSR", "Enabled").SetValue(true));
            SoulRingMenu.AddItem(
                new MenuItem("enabledSRAbilities", "Enabled for").SetValue(new AbilityToggler(AbilitiesSR)))
                .DontSave();
            SoulRingMenu.AddItem(new MenuItem("soulringHPThreshold", "HP threshold").SetValue(new Slider(30))
                .SetTooltip("Don't use soul ring if HP % less than X"));

            ManaCheckMenu.AddItem(new MenuItem("enabledMC", "Enabled").SetValue(false));
            ManaCheckMenu.AddItem(
                new MenuItem("enabledMCAbilities", "Enabled for").SetValue(new AbilityToggler(AbilitiesMC)))
                .DontSave();
            ManaCheckMenu.AddItem(new MenuItem("mcManaInfo", "Show mana info").SetValue(true)
                .SetTooltip("Will show how much mana left/needed after/before casting combo"));
            ManaCheckMenu.AddItem(new MenuItem("mcPTcalculations", "Include PT switcher calculations").SetValue(true)
                .SetTooltip("Will include in calculations mana gained from PT switching"));
            ManaCheckMenu.AddItem(new MenuItem("mcSize", "Size").SetValue(new Slider(8, 1, 10)))
                .SetTooltip("Reload assembly to apply new size");
            ManaCheckMenu.AddItem(
                new MenuItem("mcX", "Position X").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeX())));
            ManaCheckMenu.AddItem(
                new MenuItem("mcY", "Position Y").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeY())));

            Menu.AddSubMenu(PTMenu);
            Menu.AddSubMenu(RecoveryMenu);
            Menu.AddSubMenu(SoulRingMenu);
            Menu.AddSubMenu(ManaCheckMenu);

            Menu.AddItem(new MenuItem("checkPTdelay", "PT check delay").SetValue(new Slider(250, 200, 500))
                .SetTooltip("Make delay bigger if PT constantly switching when using bottle for example"));

            Menu.AddToMainMenu();

            manaCheckText = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription {
                    FaceName = "Tahoma",
                    Height = 13 * (ManaCheckMenu.Item("mcSize").GetValue<Slider>().Value / 2),
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,
                    CharacterSet = FontCharacterSet.Hangul,
                    MipLevels = 3,
                    PitchAndFamily = FontPitchAndFamily.Modern,
                    Weight = FontWeight.Heavy,
                    Width = 5 * (ManaCheckMenu.Item("mcSize").GetValue<Slider>().Value / 2)
                });

            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;

            Player.OnExecuteOrder += Player_OnExecuteAction;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnWndProc(WndEventArgs args) {
            if (args.WParam == RecoveryMenu.Item("hotkey").GetValue<KeyBind>().Key && !Game.IsChatOpen) {
                enabledRecovery = args.Msg == (uint) Utils.WindowsMessages.WM_KEYDOWN;

                if (stopAttack) {
                    Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 0");
                    stopAttack = false;
                }

                if (!enabledRecovery) {
                    PickUpItems();
                    Game.ExecuteCommand("dota_player_units_auto_attack_after_spell 1");
                    stopAttack = true;
                }
            }
        }

        private static void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args) {
            switch (args.Order) {
                case Order.AttackTarget:
                case Order.AttackLocation:
                    ChangePtOnAction("switchPTonAttack", true);
                    break;
                case Order.AbilityTarget:
                case Order.AbilityLocation:
                case Order.Ability:
                case Order.ToggleAbility:
                    if (!Game.IsKeyDown(16))
                        CastSpell(args);
                    break;
                case Order.MoveLocation:
                case Order.MoveTarget:
                    PickUpItemsOnMove(args);
                    ChangePtOnAction("switchPTonMove");
                    break;
                default:
                    attacking = false;
                    break;
            }
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("HpMpAbuseDelay"))
                return;

            if (!inGame) {
                hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || hero == null) {
                    Utils.Sleep(1000, "HpMpAbuseDelay");
                    return;
                }

                AbilitiesPT.Clear();
                AbilitiesSR.Clear();
                AbilitiesMC.Clear();
                ItemSlots.Clear();

                lastPtAttribute = Attribute.Strength;
                stopAttack = true;
                ptChanged = healActive = disableSwitchBack = attacking = enabledRecovery = false;

                if (autoDisablePT) {
                    PTMenu.Item("enabledPT").SetValue(true).DontSave();
                    autoDisablePT = false;
                }

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            if (!hero.IsAlive || Game.IsPaused) {
                Utils.Sleep(Menu.Item("checkPTdelay").GetValue<Slider>().Value, "HpMpAbuseDelay");
                return;
            }

            var reloadMenu = false;

            foreach (var spell in hero.Spellbook.Spells.Where(CheckAbility)) {
                if (spell.ClassID != ClassID.CDOTA_Ability_SkeletonKing_Reincarnation) {
                    AbilitiesPT.Add(spell.Name, true);
                    AbilitiesSR.Add(spell.Name, true);
                }
                AbilitiesMC.Add(spell.Name, false);
                reloadMenu = true;
            }

            foreach (var item in hero.Inventory.Items.Where(CheckAbility)) {
                AbilitiesPT.Add(item.Name, true);
                AbilitiesSR.Add(item.Name, true);
                AbilitiesMC.Add(item.Name, false);
                reloadMenu = true;
            }

            if (reloadMenu) {
                PTMenu.Item("enabledPTAbilities").SetValue((new AbilityToggler(AbilitiesPT))).DontSave();
                SoulRingMenu.Item("enabledSRAbilities").SetValue((new AbilityToggler(AbilitiesSR))).DontSave();
                ManaCheckMenu.Item("enabledMCAbilities").SetValue((new AbilityToggler(AbilitiesMC))).DontSave();
            }

            if (!autoDisablePT && Game.GameTime / 60 > PTMenu.Item("autoPTdisable").GetValue<Slider>().Value &&
                PTMenu.Item("autoPTdisable").GetValue<Slider>().Value != 0 && PTMenu.Item("enabledPT").GetValue<bool>()) {
                PTMenu.Item("enabledPT").SetValue(false).DontSave();
                autoDisablePT = true;
            }

            if (autoDisablePT && PTMenu.Item("enabledPT").GetValue<bool>()) {
                autoDisablePT = false;
            }

            powerTreads = (PowerTreads) hero.FindItem("item_power_treads");

            if (powerTreads != null && !ptChanged && !attacking) {
                switch (powerTreads.ActiveAttribute) {
                    case Attribute.Intelligence: // agi
                        lastPtAttribute = Attribute.Agility;
                        break;
                    case Attribute.Strength:
                        lastPtAttribute = Attribute.Strength;
                        break;
                    case Attribute.Agility: // int
                        lastPtAttribute = Attribute.Intelligence;
                        break;
                }
            }

            if (attacking)
                ChangePtOnAction("switchPTonAttack", true);

            if (ManaCheckMenu.Item("enabledMC").GetValue<bool>()) {
                var heroMana = hero.Mana;

                var manaCost = (from ability in AbilitiesMC
                                where ManaCheckMenu.Item("enabledMCAbilities")
                                    .GetValue<AbilityToggler>()
                                    .IsEnabled(ability.Key)
                                select hero.FindSpell(ability.Key) ?? hero.FindItem(ability.Key))
                                    .Aggregate<Ability, uint>(0,
                                        (current, spell) => current + spell.ManaCost);

                if (powerTreads != null && lastPtAttribute != Attribute.Intelligence &&
                    ManaCheckMenu.Item("mcPTcalculations").GetValue<bool>()) {
                    heroMana += heroMana / hero.MaximumMana * 117;
                }

                manaLeft = (int) Math.Ceiling(heroMana - manaCost);
            }

            if (enabledRecovery && (hero.Mana < hero.MaximumMana || hero.Health < hero.MaximumHealth)) {
                if (ObjectMgr.GetEntities<Hero>().Any(x =>
                        x.IsAlive && x.IsVisible && x.Team == hero.GetEnemyTeam() &&
                        x.Distance2D(hero) <= RecoveryMenu.Item("forcePickEnemyNearDistance").GetValue<Slider>().Value)) {
                    PickUpItems();
                    Utils.Sleep(1000, "HpMpAbuseDelay");
                    return;
                }

                var arcaneBoots = hero.FindItem("item_arcane_boots");
                var greaves = hero.FindItem("item_guardian_greaves");
                var soulRing = hero.FindItem("item_soul_ring");
                var bottle = hero.FindItem("item_bottle");
                var stick = hero.FindItem("item_magic_stick") ?? hero.FindItem("item_magic_wand");
                var meka = hero.FindItem("item_mekansm");
                var urn = hero.FindItem("item_urn_of_shadows");

                if (meka != null && meka.CanBeCasted() && hero.Health != hero.MaximumHealth) {
                    ChangePowerTreads(Attribute.Intelligence);
                    DropItems(BonusHealth, meka);
                    meka.UseAbility(true);
                }

                if (arcaneBoots != null && arcaneBoots.CanBeCasted() && hero.Mana < hero.MaximumMana) {
                    ChangePowerTreads(Attribute.Agility);
                    DropItems(BonusMana, arcaneBoots);
                    arcaneBoots.UseAbility(true);
                }

                if (greaves != null && greaves.CanBeCasted()) {
                    ChangePowerTreads(Attribute.Agility);
                    DropItems(BonusHealth.Concat(BonusMana), greaves);
                    greaves.UseAbility(true);
                }

                if (soulRing != null && soulRing.CanBeCasted()) {
                    ChangePowerTreads(Attribute.Strength);
                    DropItems(BonusMana);
                    soulRing.UseAbility(true);
                }

                var bottleRegen = hero.Modifiers.FirstOrDefault(x => x.Name == "modifier_bottle_regeneration");

                if (bottle != null && bottle.CanBeCasted() && bottle.CurrentCharges != 0 &&
                    (bottleRegen == null || bottleRegen.RemainingTime < 0.2)) {
                    if ((float) hero.Health / hero.MaximumHealth < 0.9)
                        DropItems(BonusHealth);
                    if (hero.Mana / hero.MaximumMana < 0.9)
                        DropItems(BonusMana);

                    bottle.UseAbility(true);
                }

                if (stick != null && stick.CanBeCasted() && stick.CurrentCharges != 0) {
                    ChangePowerTreads(Attribute.Agility);

                    if ((float) hero.Health / hero.MaximumHealth < 0.9)
                        DropItems(BonusHealth, stick);
                    if (hero.Mana / hero.MaximumMana < 0.9)
                        DropItems(BonusMana, stick);

                    stick.UseAbility(true);
                }

                if (urn != null && urn.CanBeCasted() && urn.CurrentCharges != 0 &&
                    hero.Modifiers.All(x => x.Name != "modifier_item_urn_heal") &&
                    (float) hero.Health / hero.MaximumHealth < 0.9) {
                    DropItems(BonusHealth, urn);
                    urn.UseAbility(hero, true);
                }

                if (hero.Modifiers.Any(x => HealModifiers.Any(x.Name.Equals))) {
                    if ((float) hero.Health / hero.MaximumHealth < 0.9)
                        DropItems(BonusHealth);
                    if (hero.Mana / hero.MaximumMana < 0.9 && bottleRegen != null)
                        DropItems(BonusMana);
                }

                var allies =
                    ObjectMgr.GetEntities<Hero>()
                        .Where(x => x.Distance2D(hero) <= 900 && x.IsAlive && x.Team == hero.Team);

                foreach (var ally in allies) {
                    var allyArcaneBoots = ally.FindItem("item_arcane_boots");
                    var allyMeka = ally.FindItem("item_mekansm");
                    var allyGreaves = ally.FindItem("item_guardian_greaves");

                    if (allyArcaneBoots != null && allyArcaneBoots.AbilityState == AbilityState.Ready) {
                        ChangePowerTreads(Attribute.Agility);
                        DropItems(BonusMana);
                    }

                    if (allyMeka != null && allyMeka.AbilityState == AbilityState.Ready) {
                        ChangePowerTreads(Attribute.Agility);
                        DropItems(BonusHealth);
                    }

                    if (allyGreaves != null && allyGreaves.AbilityState == AbilityState.Ready) {
                        ChangePowerTreads(Attribute.Agility);
                        DropItems(BonusMana.Concat(BonusHealth));
                    }
                }
            }

            if (powerTreads == null) {
                Utils.Sleep(Menu.Item("checkPTdelay").GetValue<Slider>().Value, "HpMpAbuseDelay");
                return;
            }

            disableSwitchBack = hero.Modifiers.Any(x => DisableSwitchBackModifiers.Any(x.Name.Equals));

            if (hero.Modifiers.Any(x => HealModifiers.Any(x.Name.Equals)) && !disableSwitchBack &&
                (PTMenu.Item("switchPTHeal").GetValue<bool>() && PTMenu.Item("enabledPT").GetValue<bool>() ||
                 enabledRecovery)) {
                if (hero.Modifiers.Any(
                        x => (x.Name == "modifier_bottle_regeneration" || x.Name == "modifier_clarity_potion"))) {
                    if (hero.Mana / hero.MaximumMana < 0.9 && (float) hero.Health / hero.MaximumHealth > 0.9) {
                        if (lastPtAttribute == Attribute.Intelligence) {
                            ChangePowerTreads(Attribute.Strength, true, true);
                        } else {
                            healActive = false;
                        }
                    } else if (hero.Mana / hero.MaximumMana > 0.9 && (float) hero.Health / hero.MaximumHealth < 0.9) {
                        if (lastPtAttribute == Attribute.Strength) {
                            if (hero.PrimaryAttribute == Attribute.Agility)
                                ChangePowerTreads(Attribute.Agility, true, true);
                            else if (hero.PrimaryAttribute == Attribute.Intelligence)
                                ChangePowerTreads(Attribute.Intelligence, true, true);
                        } else {
                            healActive = false;
                        }
                    } else if (hero.Mana / hero.MaximumMana < 0.9 && (float) hero.Health / hero.MaximumHealth < 0.9) {
                        ChangePowerTreads(Attribute.Agility, true, true);
                    } else {
                        healActive = false;
                    }
                } else {
                    if ((float) hero.Health / hero.MaximumHealth < 0.9) {
                        if (lastPtAttribute == Attribute.Strength) {
                            if (hero.PrimaryAttribute == Attribute.Agility)
                                ChangePowerTreads(Attribute.Agility, true, true);
                            else if (hero.PrimaryAttribute == Attribute.Intelligence)
                                ChangePowerTreads(Attribute.Intelligence, true, true);
                        } else {
                            healActive = false;
                        }
                    } else if (hero.Health == hero.MaximumHealth && healActive) {
                        healActive = false;
                    }
                }
            } else {
                healActive = false;
            }

            if (ptChanged && !healActive && !disableSwitchBack && !enabledRecovery && !attacking) {
                foreach (var spell in hero.Spellbook.Spells.Where(spell => spell.IsInAbilityPhase)) {
                    Utils.Sleep(
                        spell.FindCastPoint() * 1000 + PTMenu.Item("switchbackPTdelay").GetValue<Slider>().Value,
                        "HpMpAbuseDelay");
                    return;
                }

                ChangePowerTreads(lastPtAttribute, false);
            }

            Utils.Sleep(Menu.Item("checkPTdelay").GetValue<Slider>().Value, "HpMpAbuseDelay");
        }

        private static void CastSpell(ExecuteOrderEventArgs args) {
            var spell = args.Ability;

            if (spell.ManaCost <= PTMenu.Item("manaPTThreshold").GetValue<Slider>().Value ||
                IgnoredSpells.Any(spell.Name.Equals))
                return;

            var soulRing = hero.FindItem("item_soul_ring");

            if (powerTreads == null && soulRing == null)
                return;

            if (!PTMenu.Item("enabledPT").GetValue<bool>() && !SoulRingMenu.Item("enabledSR").GetValue<bool>())
                return;

            args.Process = false;

            if (SoulRingMenu.Item("enabledSR").GetValue<bool>()) {
                if (soulRing != null && soulRing.CanBeCasted() &&
                    SoulRingMenu.Item("enabledSRAbilities").GetValue<AbilityToggler>().IsEnabled(spell.Name) &&
                    ((float) hero.Health / hero.MaximumHealth) * 100 >=
                    SoulRingMenu.Item("soulringHPThreshold").GetValue<Slider>().Value) {
                    soulRing.UseAbility();
                }
            }

            var sleep = spell.FindCastPoint() * 1000 + PTMenu.Item("switchbackPTdelay").GetValue<Slider>().Value;

            if (AttackSpells.Any(spell.Name.Equals))
                sleep += hero.SecondsPerAttack * 1000;

            switch (args.Order) {
                case Order.AbilityTarget: {
                    var target = (Unit) args.Target;
                    if (target != null && target.IsAlive) {
                        var castRange = spell.GetCastRange() + 300;

                        if (hero.Distance2D(target) <= castRange && PTMenu.Item("enabledPT").GetValue<bool>()) {
                            if (PTMenu.Item("enabledPTAbilities").GetValue<AbilityToggler>().IsEnabled(spell.Name))
                                ChangePowerTreads(Attribute.Intelligence);
                            else if (AttackSpells.Any(spell.Name.Equals)) {
                                ChangePtOnAction("switchPTonAttack", true);
                            }
                            sleep += hero.GetTurnTime(target) * 1000;
                        }
                        spell.UseAbility(target);
                    }
                    break;
                }
                case Order.AbilityLocation: {
                    var castRange = spell.GetCastRange() + 300;

                    if (hero.Distance2D(Game.MousePosition) <= castRange && PTMenu.Item("enabledPT").GetValue<bool>() &&
                        PTMenu.Item("enabledPTAbilities").GetValue<AbilityToggler>().IsEnabled(spell.Name)) {
                        ChangePowerTreads(Attribute.Intelligence);
                        sleep += hero.GetTurnTime(Game.MousePosition) * 1000;
                    }
                    spell.UseAbility(Game.MousePosition);
                    break;
                }
                case Order.Ability: {
                    if (PTMenu.Item("enabledPT").GetValue<bool>()) {
                        if (PTMenu.Item("enabledPTAbilities").GetValue<AbilityToggler>().IsEnabled(spell.Name))
                            ChangePowerTreads(Attribute.Intelligence);
                        else if (spell.Name == AttackSpells[3]) {
                            ChangePtOnAction("switchPTonAttack", true);
                        }
                    }
                    spell.UseAbility();
                    break;
                }
                case Order.ToggleAbility: {
                    if (PTMenu.Item("enabledPT").GetValue<bool>() &&
                        PTMenu.Item("enabledPTAbilities").GetValue<AbilityToggler>().IsEnabled(spell.Name))
                        ChangePowerTreads(Attribute.Intelligence);
                    spell.ToggleAbility();
                    break;
                }
            }
            Utils.Sleep(sleep, "HpMpAbuseDelay");
        }

        private static void ChangePowerTreads(Attribute attribute, bool switchBack = true, bool healing = false) {
            if (powerTreads == null)
                return;

            healActive = healing;

            if (hero.IsChanneling() || hero.IsInvisible() || !hero.CanUseItems())
                return;

            var ptNow = 0;
            var ptTo = 0;

            switch (powerTreads.ActiveAttribute) {
                case Attribute.Strength:
                    ptNow = 1;
                    break;
                case Attribute.Agility: // int
                    ptNow = 2;
                    break;
                case Attribute.Intelligence: // agi
                    ptNow = 3;
                    break;
            }

            switch (attribute) {
                case Attribute.Strength:
                    ptTo = 1;
                    break;
                case Attribute.Intelligence:
                    ptTo = 2;
                    break;
                case Attribute.Agility:
                    ptTo = 3;
                    break;
            }

            if (ptNow == ptTo)
                return;

            var change = ptTo - ptNow % 3;

            if (ptNow == 2 && ptTo == 1) // random fix
                change = 2;

            ptChanged = switchBack;

            for (var i = 0; i < change; i++)
                powerTreads.UseAbility();
        }

        private static void ChangePtOnAction(string action, bool isAttacking = false) {
            if (!PTMenu.Item("enabledPT").GetValue<bool>() || healActive || enabledRecovery || !Utils.SleepCheck("HpMpAbuseDelay2"))
                return;

            switch (PTMenu.Item(action).GetValue<StringList>().SelectedIndex) {
                case 1:
                    ChangePowerTreads(hero.PrimaryAttribute, false);
                    break;
                case 2:
                    ChangePowerTreads(Attribute.Strength, false);
                    break;
                case 3:
                    ChangePowerTreads(Attribute.Intelligence, false);
                    break;
                case 4:
                    ChangePowerTreads(Attribute.Agility, false);
                    break;
                default:
                    return;
            }

            attacking = isAttacking;

            Utils.Sleep(500, "HpMpAbuseDelay2");
        }

        private static void PickUpItemsOnMove(ExecuteOrderEventArgs args) {
            if(enabledRecovery && RecoveryMenu.Item("forcePickMoved").GetValue<bool>()) {
                args.Process = false;
                PickUpItems(true);
                Utils.Sleep(1000, "HpMpAbuseDelay");
            }
        }

        private static void PickUpItems(bool move = false) {
            var droppedItems =
                ObjectMgr.GetEntities<PhysicalItem>().Where(x => x.Distance2D(hero) < 250).Reverse().ToList();

            for (var i = 0; i < droppedItems.Count; i++)
                hero.PickUpItem(droppedItems[i], i != 0);

            foreach (var itemSlot in ItemSlots)
                itemSlot.Value.MoveItem(itemSlot.Key);

            ItemSlots.Clear();

            if (move) hero.Move(Game.MousePosition, true);

            if (!ptChanged)
                return;

            if(hero.Modifiers.All(x => x.Name != "modifier_bottle_regeneration"))
                ChangePowerTreads(lastPtAttribute, false);
        }

        private static void DropItems(IEnumerable<string> bonusStats, Item ignoredItem = null) {
            var items = hero.Inventory.Items;
            foreach (
                var item in
                    items.Where(
                        item => !item.Equals(ignoredItem) && item.AbilityData.Any(x => bonusStats.Any(x.Name.Equals)))) {
                SaveItemSlot(item);
                hero.DropItem(item, hero.NetworkPosition, true);
            }
        }

        private static void SaveItemSlot(Item item) {
            if (!hero.Inventory.FreeSlots.Any())
                return;

            for (var i = 0; i < 6; i++) {
                var currentItem = hero.Inventory.GetItem((ItemSlot) i);

                if (currentItem == null || !currentItem.Equals(item) || ItemSlots.ContainsKey((ItemSlot) i)) continue;

                ItemSlots.Add((ItemSlot) i, item);
                break;
            }
        }

        private static bool CheckAbility(Ability ability) {
            return ability.ManaCost > 0 && !AbilitiesMC.ContainsKey(ability.Name) &&
                   !IgnoredSpells.Any(ability.Name.Equals);
        }

        private static void Drawing_OnEndScene(EventArgs args) {
            if (Drawing.Direct3DDevice9 == null || !ManaCheckMenu.Item("enabledMC").GetValue<bool>() || !inGame)
                return;

            var text = manaLeft >= 0 ? "Yes" : "No";

            if (ManaCheckMenu.Item("mcManaInfo").GetValue<bool>())
                text += " (" + manaLeft + ")";

            manaCheckText.DrawText(
                null,
                text,
                ManaCheckMenu.Item("mcX").GetValue<Slider>().Value,
                ManaCheckMenu.Item("mcY").GetValue<Slider>().Value,
                manaLeft >= 0 ? Color.Yellow : Color.DarkOrange);
        }

        private static void Drawing_OnPostReset(EventArgs args) {
            manaCheckText.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args) {
            manaCheckText.OnLostDevice();
        }
    }
}