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
        private static readonly string[] Deff = {
            "puck_phase_shift",
            "nyx_assassin_spiked_carapace",
            "templar_assassin_refraction",
            "item_cyclone",
            "item_blade_mail",
            "abaddon_aphotic_shield",
            "alchemist_chemical_rage",
            "phantom_lancer_doppelwalk",
            "enchantress_natures_attendants",
            "huskar_inner_vitality",
            "legion_commander_press_the_attack",
        };

        private static readonly string[] DeffVsDisable = {
            "slark_dark_pact",
            "item_manta"
        };

        private static readonly string[] DeffVsTarget = {
            "item_lotus_orb"
        };

        private static readonly string[] DeffVsMagic = {
            "item_glimmer_cape",
            "oracle_fates_edict",
            "ember_spirit_flame_guard",
            "life_stealer_rage",
            "juggernaut_blade_fury",
            "omniknight_repel",
            "pugna_nether_ward"
        };

        private static readonly string[] Invis = {
            "bounty_hunter_wind_walk",
            "clinkz_wind_walk",
            "item_glimmer_cape",
            "item_invis_sword",
            "item_silver_edge",
            "nyx_assassin_vendetta",
            "sandking_sand_storm",
            "templar_assassin_meld",
            "weaver_shukuchi"
        };

        private static readonly string[] DeffVsPhys = {
            "item_ghost",
            "pugna_decrepify",
            "windrunner_windrun",
            "winter_wyvern_cold_embrace",
            "lich_frost_armor"
        };

        private static readonly string[] Off = {
            "item_cyclone",
            "item_sheepstick",
            "item_orchid",
            "item_abyssal_blade",
            "puck_waning_rift",
            "dragon_knight_dragon_tail",
            "lion_voodoo",
            "shadow_shaman_voodoo",
            "shadow_shaman_shackles",
            "rubick_telekinesis",
            "skywrath_mage_ancient_seal",
            "tusk_snowball",
            "keeper_of_the_light_mana_leak"
        };

        private static readonly string[] OffVsPhys = {
            "item_ethereal_blade",
            "item_heavens_halberd",
            "item_solar_crest",
            "keeper_of_the_light_blinding_light",
            "razor_static_link",
            "shadow_demon_demonic_purge",
            "brewmaster_drunken_haze"
        };

        private static readonly string[] IgnoresMagicImmunity = {
            "item_abyssal_blade"
        };

        private static readonly string[] BlinkAbilities = {
            "item_blink",
            "mirana_leap",
            "antimage_blink",
            "magnataur_skewer",
            "queenofpain_blink",
            "morphling_waveform",
            "sandking_burrowstrike",
            "faceless_void_time_walk",
            "earth_spirit_rolling_boulder",
            "ember_spirit_activate_fire_remnant"
        };

        private static bool inGame;
        private static Hero hero;

        private static readonly Menu Menu = new Menu("Counter Spells", "counterSpells", true);

        private static bool dodge = true;
        private static Font text;

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

            if (!hero.IsAlive || Game.IsPaused || !dodge) {
                Utils.Sleep(500, "CounterDelay");
                return;
            }

            if (!hero.CanUseItems() || hero.IsChanneling() || hero.IsInvisible())
                return;

            var enemies =
                ObjectMgr.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsAlive && x.Team == hero.GetEnemyTeam() && !x.IsIllusion);

            foreach (var enemy in enemies) {
                Ability spell;
                float spellCastRange, distance = hero.Distance2D(enemy);
                double castPoint;
                var angle =
                    Math.Abs(enemy.FindAngleR() - Utils.DegreeToRadian(enemy.FindAngleForTurnTime(hero.NetworkPosition)));

                switch (enemy.ClassID) {
                    case ClassID.CDOTA_Unit_Hero_Alchemist:
                    {
                        spell = enemy.FindSpell("alchemist_unstable_concoction_throw");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();

                            if (distance > spellCastRange)
                                continue;

                            if (angle > 0.03)
                                continue;

                            if (Blink()) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys).Concat(DeffVsTarget).Concat(Invis)))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance))
                                return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_AntiMage:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Axe:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys)))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Bane:
                    {
                        spell = enemy.FindSpell("bane_nightmare");

                        if (!spell.IsInAbilityPhase)
                            spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Batrider:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Beastmaster:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget).Concat(DeffVsPhys))))
                                return;
                            if (UseOnTarget(Off.Concat(OffVsPhys), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Bloodseeker:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys).Concat(Invis).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(Off.Concat(OffVsPhys), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_BountyHunter:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if ((distance > spellCastRange || angle > 0.03) &&
                                (hero.Modifiers.All(x => x.Name != "modifier_bounty_hunter_track") || distance > 1000))
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Brewmaster:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1000;

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Broodmother:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1200;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Centaur:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys).Concat(Invis).Concat(DeffVsTarget)))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys).Concat(Invis).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(Off.Concat(OffVsPhys), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_CrystalMaiden:
                    {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = 850;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DarkSeer:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DoomBringer:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Earthshaker:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (IsCasted(spell)) {
                            spellCastRange = 600;
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Enigma:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_FacelessVoid:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 1)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Huskar:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Juggernaut:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();

                            if (distance > spellCastRange || angle > 1)
                                continue;

                            if (Blink()) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys).Concat(Invis))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance)) return;
                        }

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();

                            if (distance > spellCastRange || angle > 1)
                                continue;

                            if (Blink()) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys).Concat(Invis))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Kunkka:
                    {
                        spell = enemy.FindSpell("kunkka_x_marks_the_spot");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsMagic.Concat(Invis).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        var xMark = hero.Modifiers.FirstOrDefault(x => x.Name == "modifier_kunkka_x_marks_the_spot");

                        if (xMark == null)
                            continue;

                        spell = enemy.FindSpell("kunkka_return");

                        if (spell.IsInAbilityPhase || xMark.RemainingTime < 0.10) {
                            castPoint = 0.1;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Legion_Commander:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsPhys).Concat(Invis))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lich:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.6)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lion:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Luna:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Magnataur:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Necrolyte:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Nyx_Assassin:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Ogre_Magi:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (!spell.IsInAbilityPhase)
                            spell = enemy.Spellbook.SpellD;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Oracle:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Obsidian_Destroyer:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_PhantomLancer:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Pudge:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink()) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                            if (UseOnTarget(Off, enemy, distance)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_QueenOfPain:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 900;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget))) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SandKing:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Slardar:
                    {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys).Concat(Invis)))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SpiritBreaker:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys).Concat(Invis)))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Sven:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys).Concat(Invis).Concat(DeffVsTarget)))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Terrorblade:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnTarget(Off.Concat(OffVsPhys), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tidehunter:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tiny:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.5)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_VengefulSpirit:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1250;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget)))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Venomancer:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 500;

                            if (distance > spellCastRange)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Viper:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1200;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget))) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Warlock:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsMagic).Concat(Invis)))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Windrunner:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 600;
                            castPoint = distance / 1515;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(Invis).Concat(DeffVsTarget).Concat(DeffVsPhys)))) return;
                            if (UseOnTarget(Off.Concat(OffVsPhys), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Winter_Wyvern:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();
                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 0.8)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(DeffVsTarget).Concat(DeffVsPhys))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_WitchDoctor:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange() + 600;
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.3)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis).Concat(DeffVsTarget))) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            spellCastRange = spell.GetCastRange();

                            if (enemy.AghanimState())
                                spellCastRange += 500;

                            castPoint = spell.FindCastPoint();

                            if (distance > spellCastRange || angle > 1)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic).Concat(Invis))) return;
                            if (UseOnTarget(Off, enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                    {
                        spell = enemy.Spellbook.SpellQ;

                        if (IsCasted(spell)) {
                            spellCastRange = spell.GetCastRange();
                            castPoint = distance / 1000;

                            if (distance > spellCastRange || angle > 0.03)
                                continue;

                            if (Blink(castPoint)) return;
                            if (UseOnSelf(DeffVsDisable.Concat(Deff.Concat(DeffVsPhys).Concat(Invis).Concat(DeffVsTarget)))) return;
                            if (UseOnTarget(OffVsPhys.Concat(Off), enemy, distance, castPoint)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Zuus:
                    {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            if (UseOnSelf(Deff.Concat(Invis).Concat(DeffVsMagic))) return;
                        }

                        break;
                    }
                }
            }

            foreach (var modifier in hero.Modifiers) {
                switch (modifier.Name) {
                    case "modifier_lina_laguna_blade":
                    case "modifier_lion_finger_of_death":
                    {
                        if (UseOnSelf(Deff.Concat(Invis).Concat(DeffVsMagic)))
                            return;
                        break;
                    }
                    case "modifier_spirit_breaker_charge_of_darkness_vision":
                    {
                        var bara =
                            ObjectMgr.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_SpiritBreaker);

                        if (bara == null)
                            continue;

                        var distance = hero.Distance2D(bara);

                        if (distance <= 500 && UseOnSelf(Deff.Concat(Invis)))
                            return;

                        if (UseOnTarget(Off, bara, hero.Distance2D(bara)))
                            return;

                        break;
                    }
                    case "modifier_sniper_assassinate":
                    {
                        if (modifier.RemainingTime < 2) {
                            if (Blink()) return;
                            if (UseOnSelf(Deff.Concat(DeffVsMagic))) return;
                        }

                        break;
                    }
                }
            }

            if (hero.IsSilenced()) {
                if (Blink()) return;
                if (UseOnSelf(Deff)) return;

                if (!hero.Modifiers.Any(
                    x => x.Name == "modifier_riki_smoke_screen" || x.Name == "modifier_disruptor_static_storm")) {
                    if (UseOnSelf(new[] { "item_manta" })) return;
                    //if (UseOnSelf(new[] {"item_diffusal_blade", "item_diffusal_blade_2"})) return;
                }
            }
        }

        private static bool Blink(double castPoint = 5) {
            if (!Menu.Item("blink").GetValue<bool>())
                return false;

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
                    .FirstOrDefault(x => x.Team == hero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);

            if (hero.GetTurnTime(home) + blink.FindCastPoint() > castPoint || home == null)
                return false;

            var angle = hero.NetworkPosition.ToVector2().FindAngleBetween(home.NetworkPosition.ToVector2(), true);
            var position = new Vector3(hero.Position.X + castRange * (float) Math.Cos(angle),
                hero.Position.Y + castRange * (float) Math.Sin(angle), hero.Position.Z);
            blink.UseAbility(position);

            Utils.Sleep(1000, "CounterDelay");
            return true;
        }

        private static bool UseOnTarget(IEnumerable<string> abilitiesNames, Unit target, float distance,
            double castPoint = 5) {
            foreach (
                var ability in
                    abilitiesNames.Select(ability => hero.FindItem(ability) ?? hero.FindSpell(ability))
                        .Where(
                            ability => ability != null && ability.CanBeCasted() && (ability is Item || hero.CanCast()))) {
                if (ability.IsAbilityBehavior(AbilityBehavior.NoTarget)) {
                    ability.UseAbility();
                } else if (hero.GetTurnTime(target) <= castPoint &&
                           target.IsValidTarget(distance, false, hero.NetworkPosition) &&
                           (!target.IsMagicImmune() || IgnoresMagicImmunity.Any(ability.Name.Equals))) {
                    if (ability.IsAbilityBehavior(AbilityBehavior.UnitTarget))
                        ability.UseAbility(target);
                    else
                        ability.UseAbility(target.NetworkPosition);
                } else continue;

                Utils.Sleep(1000, "CounterDelay");
                return true;
            }
            return false;
        }

        private static bool UseOnSelf(IEnumerable<string> abilitiesNames) {
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
                    ability.UseAbility(hero.NetworkPosition);

                Utils.Sleep(1000, "CounterDelay");
                return true;
            }
            return false;
        }

        private static void Main() {
            Menu.AddItem(new MenuItem("key", "Change hotkey").SetValue(new KeyBind('P', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("blink", "Use blink").SetValue(true)
                .SetTooltip("Suports blink dagger and most of blink type spells"));
            Menu.AddItem(new MenuItem("size", "Size").SetValue(new Slider(6, 1, 10)))
                .SetTooltip("Reload assembly to apply new size");
            Menu.AddItem(
                new MenuItem("x", "Position X").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeX())));
            Menu.AddItem(
                new MenuItem("y", "Position Y").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeY())));
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
            Game.OnWndProc += Game_OnWndProc;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnWndProc(WndEventArgs args) {
            if (inGame && !Game.IsChatOpen && args.Msg == (ulong) Utils.WindowsMessages.WM_KEYUP &&
                args.WParam == Menu.Item("key").GetValue<KeyBind>().Key) {
                dodge = !dodge;
            }
        }

        private static bool IsCasted(Ability spell) {
            return Math.Ceiling(spell.CooldownLength).Equals(Math.Ceiling(spell.Cooldown));
        }

        private static void Drawing_OnEndScene(EventArgs args) {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !inGame || !dodge)
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