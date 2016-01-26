using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace CounterSpells {
    internal class Program {

        private static readonly string[] DefVsDamage = {
            "nyx_assassin_spiked_carapace",
            "templar_assassin_refraction",
            "treant_living_armor",
            "abaddon_aphotic_shield",
            "item_blade_mail"
        };

        private static readonly string[] DefVsDisable = {
            "slark_dark_pact",
            "juggernaut_blade_fury",
            "life_stealer_rage",
            "omniknight_repel",
            "phantom_lancer_doppelwalk",
            "item_manta"
        };

        private static readonly string[] DefVsMagic = {
            "item_glimmer_cape",
            "item_hood_of_defiance",
            "oracle_fates_edict",
            "ember_spirit_flame_guard",
            "life_stealer_rage",
            "juggernaut_blade_fury",
            "omniknight_repel",
            "pugna_nether_ward",
            "item_pipe"
        };

        private static readonly string[] Invis = {
            "bounty_hunter_wind_walk",
            "clinkz_wind_walk",
            "sandking_sand_storm",
            "templar_assassin_meld",
            "weaver_shukuchi",
            "item_glimmer_cape",
            "item_invis_sword",
            "item_silver_edge"
        };

        private static readonly string[] DefVsPhys = {
            "item_ghost",
            "pugna_decrepify",
            "windrunner_windrun",
            "winter_wyvern_cold_embrace",
            "lich_frost_armor",
            "arc_warden_magnetic_field",
            "item_crimson_guard",
            "item_shivas_guard",
            "item_buckler"
        };

        private static readonly string[] InstaDisable = {
            "item_sheepstick",
            "item_orchid",
            "puck_waning_rift",
            "dragon_knight_dragon_tail",
            "lion_voodoo",
            "shadow_shaman_voodoo",
            "shadow_shaman_shackles",
            "rubick_telekinesis",
            "skywrath_mage_ancient_seal",
            "keeper_of_the_light_mana_leak",
            "crystal_maiden_frostbite",
            "item_abyssal_blade"
        };

        private static readonly string[] Invul = {
            "shadow_demon_disruption",
            "obsidian_destroyer_astral_imprisonment",
            "bane_nightmare"
        };

        private static readonly string[] OffVsPhys = {
            "item_ethereal_blade",
            "item_heavens_halberd",
            "item_solar_crest",
            "pugna_decrepify",
            "item_rod_of_atos",
            "keeper_of_the_light_blinding_light",
            "razor_static_link",
            "brewmaster_drunken_haze",
            "tinker_laser"
        };

        private static readonly string[] Shift = {
            "puck_phase_shift"
        };

        private static readonly string[] Eul = {
            "item_cyclone"
        };

        private static readonly string[] SnowBall = {
            "tusk_snowball"
        };

        private static readonly string[] Lotus = {
            "item_lotus_orb"
        };

        private static readonly string[] IgnoresMagicImmunity = {
            "item_abyssal_blade"
        };

        private static readonly string[] BlinkAbilities = {
            "item_blink",
            "mirana_leap",
            "antimage_blink",
            "magnataur_skewer",
            "item_force_staff",
            "queenofpain_blink",
            "morphling_morph_replicate",
            "morphling_waveform",
            "sandking_burrowstrike",
            "faceless_void_time_walk",
            "phantom_lancer_doppelwalk",
            "earth_spirit_rolling_boulder",
            "ember_spirit_activate_fire_remnant",
        };

        private static bool inGame;
        private static Hero hero;

        private static bool cameraCentered;
        private static Font text;

        private static Ability spell;
        private static float spellCastRange;
        private static float distance;
        private static double castPoint;
        private static double angle;

        private static readonly Menu Menu = new Menu("Counter Spells", "counterSpells", true);

        private static void Main() {
            Menu.AddItem(new MenuItem("key", "Enabled").SetValue(new KeyBind('P', KeyBindType.Toggle, true)));
            Menu.AddItem(new MenuItem("blink", "Use blink").SetValue(true)
                .SetTooltip("Suports blink dagger and most of blink type spells"));
            Menu.AddItem(new MenuItem("blinkSilenced", "Use blink when silenced").SetValue(true)
               .SetTooltip("Blink usage should be enabled too"));
            Menu.AddItem(new MenuItem("center", "Center camera on blink").SetValue(true));
            Menu.AddItem(new MenuItem("disable", "Disable enemy if can't dodge").SetValue(false)
                .SetTooltip("Use hex, stun, silence when you don't have eul, dagger, dark pact etc. to dodge stun"));
            Menu.AddItem(new MenuItem("diffusal", "Use diffusal when silenced").SetValue(false));
            Menu.AddItem(new MenuItem("size", "Size").SetValue(new Slider(6, 1, 10)))
                .SetTooltip("Reload assembly to apply new size");
            Menu.AddItem(new MenuItem("x", "Position X").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeX())));
            Menu.AddItem(new MenuItem("y", "Position Y").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeY())));
            Menu.AddToMainMenu();

            text = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription {
                    FaceName = "Tahoma",
                    Height = 13 * (Menu.Item("size").GetValue<Slider>().Value / 2),
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,
                    CharacterSet = FontCharacterSet.Hangul,
                    MipLevels = 3,
                    PitchAndFamily = FontPitchAndFamily.Modern,
                    Weight = FontWeight.Heavy,
                    Width = 5 * (Menu.Item("size").GetValue<Slider>().Value / 2)
                });

            Game.OnUpdate += Game_OnUpdate;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("CounterDelay"))
                return;

            if (!inGame) {
                hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || hero == null) {
                    Utils.Sleep(1000, "CounterDelay");
                    return;
                }

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            if (!hero.IsAlive || Game.IsPaused || !Menu.Item("key").GetValue<KeyBind>().Active) {
                Utils.Sleep(500, "CounterDelay");
                return;
            }

            if (cameraCentered) {
                cameraCentered = false;
                Game.ExecuteCommand("-dota_camera_center_on_hero");
            }

            if (!hero.CanUseItems() || hero.IsChanneling() || hero.IsInvul() || (hero.IsInvisible() && !hero.IsVisibleToEnemies))
                return;

            var enemies =
                ObjectMgr.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team == hero.GetEnemyTeam()).ToList();

            foreach (var enemy in enemies) {
                distance = hero.Distance2D(enemy);
                angle = Math.Abs(enemy.FindAngleR() - Utils.DegreeToRadian(enemy.FindAngleForTurnTime(hero.NetworkPosition)));

                if (enemy.NetworkActivity == NetworkActivity.Crit && distance <= 200 && angle <= 0.03) {
                    if (UseOnSelf(
                        Shift.Concat(
                        DefVsPhys.Concat(
                        DefVsDamage)))) return;
                }

                switch (enemy.ClassID) {
                    case ClassID.CDOTA_Unit_Hero_ArcWarden: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Lotus))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Alchemist: {
                        spell = enemy.FindSpell("alchemist_unstable_concoction_throw");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink()) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Lotus.Concat(
                                Invis)))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall.Concat(
                                OffVsPhys)), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_AntiMage: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Axe: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys)))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Lotus))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Bane: {
                        spell = enemy.FindSpell("bane_nightmare");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus)))))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }
                        
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus))))))))  return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        if (spell.IsChanneling) {
                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                Invul)), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Batrider: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus)))))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_batrider_flaming_lasso_self")) {
                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                Invul)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Beastmaster: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Bloodseeker: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus)))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_BountyHunter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if ((distance > spellCastRange || angle > 0.03) &&
                                (hero.Modifiers.All(x => x.Name != "modifier_bounty_hunter_track") || distance > 1000))
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                Lotus)))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Brewmaster: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Broodmother: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > 400 || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus))))) return;
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1200;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus))))) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "broodmother_insatiable_hunger")) {
                            if (UseOnTarget(Eul, enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Centaur: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_ChaosKnight: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (distance <= 300 && angle <= 0.03) {

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDisable.Concat(
                                    DefVsDamage.Concat(
                                    DefVsPhys.Concat(
                                    Invis.Concat(
                                    Lotus))))))) return;

                                if (UseOnTarget(
                                    InstaDisable.Concat(
                                    Eul.Concat(
                                    OffVsPhys.Concat(
                                    SnowBall))), enemy, castPoint)) return;
                            }
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus)))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable), enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Clinkz: {
                        if (enemy.Modifiers.Any(x => x.Name == "modifier_clinkz_strafe")) {
                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                Invul)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_CrystalMaiden: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus))))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = 850;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul.Concat(
                                Invul.Concat(
                                SnowBall))), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DarkSeer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis)))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tinker: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > (enemy.AghanimState() ? 0.8 : 0.03))
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis)))))) return;

                        }

                        spell = enemy.Spellbook.SpellW;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange)
                                continue;

                            var targets = ObjectMgr.GetEntities<Hero>()
                                .Where(x => x.IsVisibleToEnemies && x.IsAlive && !x.IsMagicImmune() && 
                                    !x.IsInvul() && x.Team == hero.Team)
                                .OrderBy(x => enemy.Distance2D(x))
                                .Take(enemy.AghanimState() ? 4 : 2);

                            if (targets.Contains(hero)) {
                                if (UseOnSelf(
                                    Shift.Concat(
                                    Eul.Concat(
                                    DefVsDamage.Concat(
                                    DefVsMagic.Concat(
                                    Invis)))))) return;
                            }

                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Dazzle: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DoomBringer: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus)))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul.Concat(
                                OffVsPhys.Concat(
                                SnowBall))), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DeathProphet: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic)))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(Shift)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic))))) return;

                            if (UseOnTarget(Eul, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Earthshaker: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis))))))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnSelf(Shift))
                                return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (IsCasted(spell)) {
                            spellCastRange = 600;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis)))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Enigma: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDisable.Concat(
                                Lotus.Concat(
                                Invis))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance <= spellCastRange && angle < 0.8) {

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDamage.Concat(
                                    DefVsMagic.Concat(
                                    Invis.Concat(
                                    DefVsDisable)))))) return;

                                if (UseOnTarget(
                                    InstaDisable.Concat(
                                    Eul.Concat(
                                    Invul.Concat(
                                    SnowBall))), enemy, castPoint)) return;

                                if (UseOnSelf(Eul)) return;
                            }

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul.Concat(
                                Invul.Concat(
                                SnowBall))), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_FacelessVoid: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 1)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys)))) return;

                            if (UseOnTarget(
                                Eul.Concat(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall.Concat(
                                Invul)))), enemy, castPoint)) return;

                            if (UseOnSelf(Eul)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Gyrocopter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis)))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DrowRanger: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsPhys.Concat(
                                Invis)))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Huskar: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable), enemy, castPoint)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_huskar_inner_vitality")) {
                            if (UseOnTarget(Eul, enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Juggernaut: {

                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnSelf(
                                Eul.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.4)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Lotus.Concat(
                                Invis.Concat(
                                Invul)))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable), enemy, castPoint)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_juggernaut_omnislash") && distance < 300) {
                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsPhys.Concat(
                                DefVsDamage.Concat(
                                Invis)))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Kunkka: {
                        spell = enemy.FindSpell("kunkka_x_marks_the_spot");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable), enemy, castPoint)) return;
                        }

                        var xMark = hero.Modifiers.FirstOrDefault(x => x.Name == "modifier_kunkka_x_marks_the_spot");

                        if (xMark == null)
                            continue;

                        spell = enemy.FindSpell("kunkka_return");

                        if (spell.IsInAbilityPhase || xMark.RemainingTime < 0.1) {
                            castPoint = 0.1;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Legion_Commander: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsPhys.Concat(
                                DefVsDamage.Concat(
                                DefVsDisable.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_legion_commander_duel")) {
                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lich: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus.Concat(
                                Invis))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                Invul.Concat(
                                Invis)))))  return;

                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lina: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic)))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lion: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus.Concat(
                                Invis)))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Luna: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis)))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Magnataur: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = 300;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (angle < 0.03) {
                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDisable.Concat(
                                    DefVsDamage.Concat(
                                    DefVsMagic.Concat(
                                    Invis)))))) return;
                            }

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Meepo: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = 400;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Morphling: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle < 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))) return;

                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > 400 || angle < 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus.Concat(
                                Invis)))))) return;
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle < 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus.Concat(
                                Invis)))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Necrolyte: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus))))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Nyx_Assassin: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Ogre_Magi: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus)))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellD;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus)))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(Shift)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Oracle: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Obsidian_Destroyer: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Lotus))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                DefVsDamage.Concat(
                                DefVsMagic))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_PhantomAssassin: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Lotus))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (IsCasted(spell)) {
                            if (distance > 300 || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsPhys.Concat(
                                DefVsDamage.Concat(
                                Invis))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                Eul)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_PhantomLancer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus.Concat(
                                Invis)))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Phoenix: {
                        spell = enemy.FindSpell("phoenix_icarus_dive");

                        if (spell.IsHidden || spell.IsInAbilityPhase) {
                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Pudge: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle > 0.2)
                                continue;

                            if (Blink()) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                Invis)))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus.Concat(
                                Invis)))))))) return;

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_QueenOfPain: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 900;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus)))))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(InstaDisable, enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                Invis))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Razor: {
                        if (enemy.Modifiers.Any(x => x.Name == "modifier_razor_static_link_buff")) {
                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                Invul.Concat(
                                OffVsPhys))), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SandKing: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus)))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_sand_king_epicenter") && distance <= spell.GetCastRange()) {
                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis))))))  return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Nevermore: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage)))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage)))) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage)))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Slardar: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SpiritBreaker: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis)))))) return;

                            if (UseOnTarget(
                                Eul.Concat(
                                OffVsPhys.Concat(
                                InstaDisable)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Sven: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance <= 300 && angle < 0.1) { 

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDisable.Concat(
                                    DefVsDamage.Concat(
                                    DefVsPhys.Concat(
                                    Invis.Concat(
                                    Lotus))))))) return;

                                if (UseOnTarget(
                                    InstaDisable.Concat(
                                    Eul.Concat(
                                    OffVsPhys.Concat(
                                    SnowBall))), enemy, castPoint)) return;
                            }
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = 650;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus)))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Terrorblade: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance <= spellCastRange && angle <= 0.03)
                                if (UseOnSelf(
                                    Shift.Concat(
                                    Eul.Concat(
                                    Lotus.Concat(
                                    Invis))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tidehunter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (enemy.AghanimState()) {
                                if (angle > 0.4) continue;
                            } else {
                                if (angle > 0.03) continue;
                            }

                            if (UseOnSelf(Shift)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = 1050;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis)))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul.Concat(
                                SnowBall)), enemy, castPoint)) return;

                            if (UseOnSelf(Eul)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tiny: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis)))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Ursa: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage)))) return;

                            if (UseOnTarget(OffVsPhys, enemy, castPoint)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_ursa_enrage")) {
                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                Invul.Concat(
                                OffVsPhys))), enemy)) return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_ursa_overpower") && distance < 300) {
                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsPhys.Concat(
                                DefVsDamage)))) return;

                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                Invul.Concat(
                                OffVsPhys))), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_VengefulSpirit: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance <= 300 && angle <= 0.03) {
                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDisable.Concat(
                                    DefVsMagic.Concat(
                                    DefVsDamage.Concat(
                                    Invis.Concat(
                                    Lotus))))))) return;

                                if (UseOnTarget(
                                    InstaDisable.Concat(
                                    Eul.Concat(
                                    OffVsPhys.Concat(
                                    SnowBall))), enemy, castPoint)) return;
                            }
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = 550;
                            castPoint = distance / 1250;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus)))))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Invis.Concat(
                                Lotus)))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Venomancer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1200;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage)))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 500;

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Viper: {
                        spell = enemy.Spellbook.SpellR;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1200;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus))))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Visage: {
                        spell = enemy.Spellbook.SpellW;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus))))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Warlock: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Invis))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Windrunner: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > 400 || angle > 0.1)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus.Concat(
                                DefVsDamage)))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 500;
                            castPoint = distance / 1650;

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsPhys.Concat(
                                Invis.Concat(
                                Lotus.Concat(
                                DefVsDamage)))))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                OffVsPhys.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.1)
                                continue;

                            if (UseOnSelf(Shift))  return;
                        }

                        if (enemy.Modifiers.Any(x => x.Name == "modifier_windrunner_focusfire")) {
                            if (UseOnTarget(
                                InstaDisable.Concat(
                                Eul.Concat(
                                Invul.Concat(
                                OffVsPhys))), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Winter_Wyvern: {
                        spell = enemy.Spellbook.SpellW;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 300;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsMagic.Concat(
                                DefVsDamage.Concat(
                                Lotus))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                Lotus))))) return;

                            if (UseOnTarget(
                                InstaDisable.Concat(
                                SnowBall), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_WitchDoctor: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > 400 || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus))))))) return;

                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy)) return;

                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 600;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis.Concat(
                                Lotus))))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange() + 50;

                            if (distance > spellCastRange || angle > 1)
                                continue;

                            if (UseOnSelf(
                                DefVsDamage.Concat(
                                DefVsPhys.Concat(
                                Invis)))) return;

                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable.Concat(
                                SnowBall.Concat(
                                Invul))), enemy)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SkeletonKing: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance <= 300 && angle <= 0.03) {

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDisable.Concat(
                                    DefVsPhys.Concat(
                                    DefVsDamage.Concat(
                                    Invis.Concat(
                                    Lotus))))))) return;

                                if (UseOnTarget(
                                    InstaDisable.Concat(
                                    Eul.Concat(
                                    OffVsPhys.Concat(
                                    SnowBall))), enemy, castPoint)) return;
                            }
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDisable.Concat(
                                DefVsPhys.Concat(
                                DefVsDamage.Concat(
                                Invis.Concat(
                                Lotus)))))))) return;

                            if (UseOnTarget(
                                OffVsPhys.Concat(
                                InstaDisable.Concat(
                                SnowBall)), enemy, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Silencer:  {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (UseOnSelf(Shift)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                Lotus))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(
                                Eul.Concat(
                                InstaDisable), enemy, castPoint)) return;

                            if (distance < 1000)
                                if (UseOnSelf(
                                    Shift.Concat(
                                    DefVsDisable))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Zuus: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 50;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (UseOnSelf(
                                Shift.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus))))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {

                            int[] damage = { 225, 350, 475 };

                            if (enemy.AghanimState()) {
                                damage[0] = 440;
                                damage[1] = 540;
                                damage[2] = 640;
                            }

                            if (hero.Health <= hero.DamageTaken(damage[spell.Level - 1], DamageType.Magical, enemy)) {
                                if (UseOnSelf(
                                    Shift.Concat(
                                    Eul.Concat(
                                    DefVsDamage.Concat(
                                    Invis.Concat(
                                    DefVsMagic)))))) return;

                                if (UseOnTarget(
                                    InstaDisable.Concat(
                                    SnowBall), enemy, castPoint)) return;
                            }
                        }

                        break;
                    }
                }
            }

            foreach (var modifier in hero.Modifiers) {
                switch (modifier.Name) {
                    case "modifier_lina_laguna_blade": {
                        var lina = enemies.FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Lina);

                        if (lina != null && lina.AghanimState()) {
                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                Lotus.Concat(
                                Invis)))))) return;
                        } else {
                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Lotus.Concat(
                                Invis))))))) return;
                        }

                        break;
                    }
                    case "modifier_lion_finger_of_death": {
                        if (UseOnSelf(
                            Shift.Concat(
                            Eul.Concat(
                            DefVsDamage.Concat(
                            DefVsMagic.Concat(
                            Lotus.Concat(
                            Invis))))))) return;

                        break;
                    }
                    case "modifier_spirit_breaker_charge_of_darkness_vision": {
                        var bara = enemies.FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_SpiritBreaker);

                        if (bara == null)
                            continue;

                        distance = hero.Distance2D(bara);

                        if (distance <= 500) {
                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic.Concat(
                                Invis)))))) return;
                        }

                        if (UseOnTarget(Eul.Concat(InstaDisable), bara, hero.Distance2D(bara)))
                            return;

                        break;
                    }
                    case "modifier_sniper_assassinate": {
                        if (modifier.RemainingTime < 2) {

                            if (Blink()) return;

                            if (UseOnSelf(
                                Shift.Concat(
                                Eul.Concat(
                                DefVsDamage.Concat(
                                DefVsMagic))))) return;
                        }

                        break;
                    }
                    case "modifier_earth_spirit_magnetize": {
                        if (UseOnSelf(
                            Eul.Concat(
                            DefVsMagic.Concat(
                            DefVsDamage)))) return;

                        break;
                    }
                }
            }

            var units =
                ObjectMgr.GetEntities<Unit>()
                    .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC && x.Team == hero.GetEnemyTeam());

            foreach (var unit in units) {
                foreach (var modifier in unit.Modifiers) {
                    switch (modifier.Name) {
                        case "modifier_lina_light_strike_array": {
                            if (hero.Distance2D(unit) <= 250) {

                                if (Blink(0.5 - modifier.ElapsedTime)) return;

                                if (UseOnSelf(
                                    DefVsDisable.Concat(
                                    DefVsMagic.Concat(
                                    DefVsDamage.Concat(
                                    Invis))))) return;
                            }

                            break;
                        }
                        case "modifier_kunkka_torrent_thinker": {
                            var time = modifier.ElapsedTime;

                            if (hero.Distance2D(unit) <= 250 && time > 1) {

                                if (Blink(1.6 - time)) return;

                                if (UseOnSelf(
                                    DefVsDisable.Concat(
                                    DefVsMagic.Concat(
                                    DefVsDamage.Concat(
                                    Invis))))) return;
                            }

                            break;
                        }
                        case "modifier_leshrac_split_earth_thinker": {
                            if (hero.Distance2D(unit) <= 250) {

                                if (Blink(0.35 - modifier.ElapsedTime)) return;

                                if (UseOnSelf(
                                    DefVsDisable.Concat(
                                    DefVsMagic.Concat(
                                    DefVsDamage.Concat(
                                    Invis))))) return;
                            }

                            break;
                        }
                        case "modifier_bloodseeker_bloodbath_thinker": {
                            var time = modifier.ElapsedTime;

                            if (hero.Distance2D(unit) <= 250 && time > 2) {

                                if (Blink(2.6 - time)) return;

                                if (UseOnSelf(
                                    DefVsDisable.Concat(
                                    DefVsMagic.Concat(
                                    DefVsDamage.Concat(
                                    Invis))))) return;
                            }

                            break;
                        }
                    }
                }
            }

            if (hero.IsSilenced()) {
                if (Menu.Item("blinkSilenced").GetValue<bool>())
                    if (Blink()) return;

                if (UseOnSelf(Eul.Concat(Lotus).Concat(new[] { "item_guardian_greaves" }).Concat(DefVsMagic)))
                    return;

                if (!hero.Modifiers.Any(
                    x => x.Name == "modifier_riki_smoke_screen" || x.Name == "modifier_disruptor_static_storm")) {
                    if (UseOnSelf(new[] {"item_manta"})) return;
                    if (Menu.Item("diffusal").GetValue<bool>())
                        if (UseOnSelf(new[] {"item_diffusal_blade", "item_diffusal_blade_2"})) return;
                }
            }

            if (hero.IsRooted()) {
                if (UseOnSelf(new[] {"item_manta", "item_guardian_greaves"}.Concat(Eul.Concat(Lotus)))) return;
            }
        }

        private static bool Blink(double castpoint = 5) {
            if (!Menu.Item("blink").GetValue<bool>() || hero.IsMagicImmune())
                return false;

            castpoint -= 0.05;

            var blink =
                hero.Inventory.Items.Concat(hero.Spellbook.Spells)
                    .FirstOrDefault(x => BlinkAbilities.Any(x.Name.Equals) && x.CanBeCasted());

            if (blink == null)
                return false;

            var castRange = blink.GetCastRange() - 100;

            if (blink.ClassID == ClassID.CDOTA_Item_BlinkDagger)
                castRange = 1900;

            if (!(blink is Item) && !hero.CanCast())
                return false;

            var home =
                ObjectMgr.GetEntities<Entity>()
                    .FirstOrDefault(x => x.Team == hero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain) as Unit;


            if (blink.ClassID == ClassID.CDOTA_Item_ForceStaff) {
                castRange = 40;
                castPoint -= 0.5;
            }

            if (blink.GetCastDelay(hero, home, true) > castpoint && 
                blink.ClassID != ClassID.CDOTA_Ability_EmberSpirit_Activate_FireRemnant || home == null)
                return false;


            if (blink.IsAbilityBehavior(AbilityBehavior.NoTarget))
                castRange = 40;

            var findangle = hero.NetworkPosition.ToVector2().FindAngleBetween(home.NetworkPosition.ToVector2(), true);
            var position = new Vector3(hero.Position.X + castRange * (float) Math.Cos(findangle),
                hero.Position.Y + castRange * (float) Math.Sin(findangle), hero.Position.Z);
        
            if (blink.ClassID == ClassID.CDOTA_Ability_EmberSpirit_Activate_FireRemnant)
                position = hero.Position;

            if (blink.ClassID == ClassID.CDOTA_Item_ForceStaff) {
                if (hero.GetTurnTime(home) > 0) {
                    hero.Move(position);
                    blink.UseAbility(hero, true);
                } else {
                    blink.UseAbility(hero);
                }
            } else if (blink.IsAbilityBehavior(AbilityBehavior.NoTarget)) {
                if (hero.GetTurnTime(home) > 0 &&
                    blink.Name != "morphling_morph_replicate") {
                    hero.Move(position);
                    blink.UseAbility(true);
                } else {
                    blink.UseAbility();
                }
            } else {
                blink.UseAbility(position);
            }

            if (Menu.Item("center").GetValue<bool>()) {
                cameraCentered = true;
                Game.ExecuteCommand("+dota_camera_center_on_hero");
            }

            Utils.Sleep(1000, "CounterDelay");
            return true;
        }

        private static bool UseOnTarget(IEnumerable<string> abilitiesNames, Unit target, double castpoint = 5) {
            if (!Menu.Item("disable").GetValue<bool>())
                return false;

            foreach (
                var ability in
                    abilitiesNames.Select(ability => hero.FindItem(ability) ?? hero.FindSpell(ability))
                        .Where(
                            ability => ability != null && ability.CanBeCasted() && (ability is Item || hero.CanCast()))) {

                if (ability.IsAbilityBehavior(AbilityBehavior.NoTarget)) {
                    if (ability.GetCastDelay(hero, target, true) <= castpoint &&
                        target.IsValidTarget(ability.GetCastRange(), false, hero.NetworkPosition) &&
                        (!target.IsMagicImmune() || IgnoresMagicImmunity.Any(ability.Name.Equals))) {
                            ability.UseAbility();
                        } else continue;
                } else if (ability.IsAbilityBehavior(AbilityBehavior.UnitTarget)) {
                    if (ability.GetCastDelay(hero, target, true) <= castpoint &&
                        target.IsValidTarget(ability.GetCastRange(), false, hero.NetworkPosition) &&
                        (!target.IsMagicImmune() || IgnoresMagicImmunity.Any(ability.Name.Equals))) {
                            ability.UseAbility(target);
                    } else continue;
                } else {
                    if (ability.GetCastDelay(hero, target, true) <= castpoint &&
                        target.IsValidTarget(ability.GetCastRange(), false, hero.NetworkPosition) &&
                        (!target.IsMagicImmune() || IgnoresMagicImmunity.Any(ability.Name.Equals))) {
                            ability.UseAbility(hero.Position);
                        } else continue;
                }

                Utils.Sleep(1000, "CounterDelay");
                return true;
            }
            return false;
        }

        private static bool UseOnSelf(IEnumerable<string> abilitiesNames) {
            if (hero.IsMagicImmune())
                return false;

            foreach (
                var ability in
                    abilitiesNames.Select(ability => hero.FindItem(ability) ?? hero.FindSpell(ability))
                        .Where(
                            ability => ability != null && ability.CanBeCasted() && (ability is Item || hero.CanCast()))) {

                if (ability.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                    ability.UseAbility();
                else if (ability.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                    ability.UseAbility(hero);
                else
                    ability.UseAbility(hero.Position);

                Utils.Sleep(1000, "CounterDelay");
                return true;
            }
            return false;
        }

        private static bool IsCasted(Ability ability) {
            return ability.Level > 0 && ability.CooldownLength > 0 && Math.Ceiling(ability.CooldownLength).Equals(Math.Ceiling(ability.Cooldown));
        }

        private static void Drawing_OnEndScene(EventArgs args) {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !inGame || !Menu.Item("key").GetValue<KeyBind>().Active)
                return;

            text.DrawText(null, "Dodge enabled", Menu.Item("x").GetValue<Slider>().Value,
                Menu.Item("y").GetValue<Slider>().Value, Color.DarkOrange);
        }

        private static void Drawing_OnPostReset(EventArgs args) {
            text.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args) {
            text.OnLostDevice();
        }
    }
}