using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using System.Collections.Generic;

namespace CounterSpells {
    internal static class Counter {

        public static void MainCounters() {
            var enemies =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team == Program.Hero.GetEnemyTeam() && x.IsVisible && x.IsAlive && !x.IsIllusion);
           
            foreach (var enemy in enemies) {
                Ability spell;
                float spellCastRange;
                double castPoint;
                var distance = Program.Hero.Distance2D(enemy);
                var angle = 
                    Math.Abs(enemy.FindAngleR() -
                             Utils.DegreeToRadian(enemy.FindAngleForTurnTime(Program.Hero.NetworkPosition)));

                switch (enemy.ClassID) {
                    case ClassID.CDOTA_Unit_Hero_ArcWarden: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsMagic,
                                Spells.Lotus);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Alchemist: {
                        spell = enemy.FindSpell("alchemist_unstable_concoction_throw");

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint))
                                return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Lotus,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall,
                                Spells.OffVsPhys);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_AntiMage: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable))
                                return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70 + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Axe: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            // TODO: manta timings

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.SnowBall)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Lotus,
                                Spells.Manta)) return;

                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Bane: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.FindSpell("bane_nightmare");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        if (spell.IsChanneling) {
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Batrider: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        if (enemy.HasModifier("modifier_batrider_flaming_lasso_self")) {
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Beastmaster: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.1)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.OffVsPhys);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Bloodseeker: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() - 0.18;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.OffVsPhys);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_BountyHunter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Lotus);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Brewmaster: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            //todo: manta ?

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();
                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Broodmother: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (distance > 300 || angle > 0.03)
                                return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus)) return;
                        }

                        if (enemy.HasModifier("broodmother_insatiable_hunger"))
                            UseOnTarget(enemy, 555, Spells.Eul);

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Centaur: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            //TODO: add manta timing

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 150;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_ChaosKnight: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.SnowBall)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();
                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Clinkz: {
                        if (enemy.HasModifier("modifier_clinkz_strafe")) {
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_CrystalMaiden: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        if (spell.IsChanneling) {
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DarkSeer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius() + 70;

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tinker: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > (enemy.AghanimState() ? 0.8 : 0.03))
                                return;

                            castPoint = spell.FindCastPoint();

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Dazzle: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DoomBringer: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.SnowBall,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DeathProphet: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.1)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint, Spells.Shift)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.1)
                                return;

                            if (UseOnSelf(555,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage)) return;

                            if (UseOnTarget(enemy, 555, Spells.Eul)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase)
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.Invul);

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Earthshaker: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.5)
                                return;

                            castPoint = spell.FindCastPoint();

                            //todo manta ?

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint, Spells.Shift))
                                return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange();

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Enigma: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.Lotus,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(555,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Manta,
                                Spells.Invis,
                                Spells.DefVsDisable)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        if (spell.IsChanneling) {
                            UseOnTarget(enemy, 555,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.Invul,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_FacelessVoid: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius() + 70;

                            if (distance > spellCastRange || angle > 1)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.SnowBall,
                                Spells.Invul)) return;

                            UseOnSelf(castPoint, Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Puck: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.5)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Gyrocopter: {
                        var barage = enemy.FindModifier("modifier_gyrocopter_rocket_barrage");

                        if (barage != null) {
                            spellCastRange = enemy.Spellbook.SpellQ.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            UseOnSelf(barage.RemainingTime,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_DrowRanger: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsPhys,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint, Spells.OffVsPhys);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Huskar: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable)) return;
                        }

                        var vitality = enemy.FindModifier("modifier_huskar_inner_vitality");

                        if (vitality != null)
                            UseOnTarget(enemy, vitality.RemainingTime, Spells.Eul);

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Juggernaut: {
                        var fury = enemy.FindModifier("modifier_juggernaut_blade_fury");

                        if (fury != null) {
                            spell = enemy.Spellbook.SpellQ;
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            if (UseOnSelf(fury.RemainingTime,
                                Spells.Eul,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.4)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Lotus,
                                Spells.Invis,
                                Spells.Invul)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable)) return;
                        }

                        if (enemy.HasModifier("modifier_juggernaut_omnislash") && distance < 300) {
                            UseOnSelf(555,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsPhys,
                                Spells.DefVsDamage,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Kunkka: {
                        spell = enemy.FindSpell("kunkka_x_marks_the_spot");

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Manta,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable)) return;
                        }

                        var xMark =
                            Program.Hero.FindModifier("modifier_kunkka_x_marks_the_spot");

                        if (xMark == null)
                            return;

                        spell = enemy.FindSpell("kunkka_return");

                        if (spell.IsInAbilityPhase || xMark.RemainingTime < 0.1) {
                            castPoint = 0.1;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Legion_Commander: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.1)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsPhys,
                                Spells.DefVsDamage,
                                Spells.DefVsDisable,
                                Spells.Invis)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.SnowBall)) return;
                        }

                        var duel = enemy.FindModifier("modifier_legion_commander_duel");

                        if (duel != null) {
                            if (UseOnTarget(enemy, duel.RemainingTime,
                                Spells.InstaDisable,
                                Spells.OffVsPhys)) return;

                            var dueledAlly =
                                ObjectManager.GetEntities<Hero>()
                                    .FirstOrDefault(
                                        x =>
                                            x.IsAlive && x.Team == Program.Hero.Team && !x.IsIllusion &&
                                            x.HasModifier("modifier_legion_commander_duel"));

                            UseOnTarget(dueledAlly, duel.RemainingTime, Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lich: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lina: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 200;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + 0.25;

                            UseOnSelf(castPoint,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lion: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 200;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall)) return;
                        }

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 200;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + 0.25;

                            UseOnSelf(castPoint,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Luna: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Mirana: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Magnataur: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Morphling: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle < 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (distance > 300 || angle < 0.3)
                                return;

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Necrolyte: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Nyx_Assassin: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall)) return;
                        }

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Ogre_Magi: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase || enemy.Spellbook.SpellD.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Oracle: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Rattletrap: {
                        if (enemy.HasModifier("modifier_rattletrap_battery_assault")) {
                            spell = enemy.Spellbook.SpellQ;
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            if (UseOnSelf(0.7,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Invul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.17)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invul,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Obsidian_Destroyer: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius() + 70;

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_PhantomAssassin: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted()) {
                            if (distance > 300 || angle > 0.1)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsPhys,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Phoenix: {
                        var dive = enemy.FindModifier("modifier_phoenix_icarus_dive");

                        if (dive != null)
                            UseOnTarget(enemy, dive.RemainingTime,
                                Spells.Eul,
                                Spells.InstaDisable);


                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Naga_Siren: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (distance > 300 || angle > 0.03)
                                return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Pudge: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.2)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint, Spells.InstaDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_QueenOfPain: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(enemy, castPoint, Spells.InstaDisable)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 700;

                            if (distance > spellCastRange || angle > 0.5)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Invul)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 700;

                            if (distance > spellCastRange || angle > 0.5)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.Invis);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Razor: {
                        if (enemy.HasModifier("modifier_razor_static_link_buff")) {
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.OffVsPhys);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SandKing: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase || spell.IsChanneling) {
                            if (UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.SnowBall)) return;
                        }

                        var epicenter = enemy.FindModifier("modifier_sand_king_epicenter");

                        if (epicenter != null) {
                            if (distance <= spell.GetCastRange() + 200)
                                if (UseOnSelf(epicenter.RemainingTime,
                                    Spells.Shift,
                                    Spells.Eul,
                                    Spells.Invul,
                                    Spells.DefVsDamage,
                                    Spells.DefVsMagic,
                                    Spells.Invis)) return;

                            UseOnTarget(enemy, epicenter.RemainingTime,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Nevermore: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsChanneling) {
                            UseOnTarget(enemy, 1.4,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Slardar: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            //todo manta

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SpiritBreaker: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.InstaDisable,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Sven: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.1)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Terrorblade: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;
                            castPoint = spell.FindCastPoint();

                            if (distance <= spellCastRange && angle <= 0.03)
                                if (UseOnSelf(castPoint,
                                    Spells.Shift,
                                    Spells.Eul,
                                    Spells.Manta,
                                    Spells.Lotus,
                                    Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tidehunter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (angle < 0.4 && enemy.AghanimState())
                                if (UseOnSelf(castPoint,
                                    Spells.Shift,
                                    Spells.DefVsDamage,
                                    Spells.DefVsMagic)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.SnowBall)) return;

                            UseOnSelf(castPoint, Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tiny: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.5)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.OffVsPhys,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Ursa: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.OffVsPhys)) return;
                        }

                        var enrage = enemy.FindModifier("modifier_ursa_enrage");

                        if (enrage != null) {
                            if (UseOnTarget(enemy, enrage.RemainingTime,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.OffVsPhys)) return;
                        }

                        if (enemy.HasModifier("modifier_ursa_overpower") && distance < 300) {
                            if (UseOnSelf(555,
                                Spells.Shift,
                                Spells.DefVsPhys,
                                Spells.DefVsDamage)) return;

                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.OffVsPhys,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_VengefulSpirit: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.SnowBall)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Venomancer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Viper: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Warlock: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Windrunner: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.1)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDisable,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus,
                                Spells.DefVsDamage)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.OffVsPhys,
                                Spells.SnowBall)) return;
                        }

                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted()) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.1)
                                return;

                            castPoint = distance / spell.GetProjectileSpeed();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        if (enemy.HasModifier("modifier_windrunner_focusfire")) {
                            UseOnTarget(enemy, 555,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.Invul,
                                Spells.OffVsPhys);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Winter_Wyvern: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius() + 70;

                            if (distance > spellCastRange || angle > 0.8)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.SnowBall,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_WitchDoctor: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 300;

                            if (distance > spellCastRange || angle > 0.3)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            if (UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.SnowBall)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(555,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis)) return;

                            if (UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Invul,
                                Spells.SnowBall)) return;
                        }

                        if (spell.IsChanneling) {
                            UseOnTarget(enemy, 555,
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.SnowBall,
                                Spells.Invul);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SkeletonKing: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            if (distance > 300 || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint() + distance / spell.GetProjectileSpeed();

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsPhys,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;

                            UseOnTarget(enemy, castPoint,
                                Spells.InstaDisable,
                                Spells.Eul,
                                Spells.OffVsPhys,
                                Spells.SnowBall);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Silencer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + spell.GetRadius();

                            if (distance > spellCastRange || angle > 0.5)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint, Spells.Shift)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();

                            if (UseOnTarget(enemy, castPoint,
                                Spells.Eul,
                                Spells.InstaDisable)) return;

                            if (distance < 1000)
                                UseOnSelf(castPoint,
                                    Spells.Shift,
                                    Spells.DefVsDisable);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Zuus: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsInAbilityPhase) {
                            spellCastRange = spell.GetCastRange() + 70;

                            if (distance > spellCastRange || angle > 0.03)
                                return;

                            castPoint = spell.FindCastPoint();

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsInAbilityPhase) {
                            castPoint = spell.FindCastPoint();
                            int[] damage = {225, 350, 475};

                            if (enemy.AghanimState()) {
                                damage[0] = 440;
                                damage[1] = 540;
                                damage[2] = 640;
                            }

                            //todo manta ?

                            if (Program.Hero.Health <=
                                Program.Hero.DamageTaken(damage[spell.Level - 1], DamageType.Magical, enemy)) {
                                if (UseOnSelf(castPoint,
                                    Spells.Shift,
                                    Spells.Eul,
                                    Spells.DefVsDamage,
                                    Spells.DefVsMagic,
                                    Spells.Invis)) return;

                                UseOnTarget(enemy, castPoint,
                                    Spells.InstaDisable,
                                    Spells.SnowBall);
                            }
                        }

                        break;
                    }
                }
            }
        }

        public static void Projectile() {
            var projectiles =
                ObjectManager.TrackingProjectiles.Where(x => x.Target.Equals(Program.Hero));

            foreach (var projectile in projectiles) {
                double castPoint;
                Ability spell;

                var enemy = projectile.Source as Hero;

                if (enemy == null)
                    return;

                //if (projectile.Speed == 1200 && enemy.FindItem("item_ethereal_blade").IsCasted()) {
                //    castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                //    if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                //    if (UseOnSelf(castPoint,
                //        Spells.Shift,
                //        Spells.Eul,
                //        Spells.DefVsDamage,
                //        Spells.DefVsMagic,
                //        Spells.Invis,
                //        Spells.Lotus)) return;
                //}

                switch (enemy.ClassID) {
                    case ClassID.CDOTA_Unit_Hero_BountyHunter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.Eul,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_ChaosKnight: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus,
                                Spells.Invul)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Dazzle: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnSelf(castPoint, Spells.Shift)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Ogre_Magi: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnSelf(castPoint, Spells.Shift)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_PhantomAssassin: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnSelf(castPoint, Spells.Shift)) return;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_QueenOfPain: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Lotus)) return;
                        }

                        spell = enemy.Spellbook.SpellE;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Sniper: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus,
                                Spells.Invul)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tidehunter: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus);
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_VengefulSpirit: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Windrunner: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus,
                                Spells.DefVsDamage)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Broodmother: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Morphling: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Lotus,
                                Spells.Invis)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Naga_Siren: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Oracle: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_PhantomLancer: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Manta,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Lotus,
                                Spells.Invis)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Sven: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis,
                                Spells.Lotus,
                                Spells.Invul)) return;
                        }
                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tinker: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Viper: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsMagic,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_SkeletonKing: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (Blink(castPoint, true)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsPhys,
                                Spells.DefVsDamage,
                                Spells.Invis,
                                Spells.Lotus)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Alchemist: {
                        spell = enemy.FindSpell("alchemist_unstable_concoction_throw");

                        if (projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            // no casted check
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (castPoint < 0.25)
                                if (UseOnSelf(castPoint, Spells.Manta)) return;

                            if (Blink(castPoint))
                                return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.Manta,
                                Spells.DefVsDisable,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Lotus,
                                Spells.Invis,
                                Spells.Invul)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Lich: {
                        spell = enemy.Spellbook.SpellR;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (Blink(castPoint)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invis)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Tusk: {
                        spell = enemy.Spellbook.SpellW;

                        if (projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            // no casted check
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsPhys,
                                Spells.Invis)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Medusa: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Necrolyte: {
                        spell = enemy.Spellbook.SpellQ;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic)) return;
                        }

                        break;
                    }
                    case ClassID.CDOTA_Unit_Hero_Visage: {
                        spell = enemy.Spellbook.SpellW;

                        if (spell.IsCasted() && projectile.Speed == (int) spell.GetProjectileSpeed()) {
                            castPoint = Program.Hero.Distance2D(projectile.Position) / projectile.Speed;

                            if (UseOnTarget(enemy, castPoint, Spells.Fist)) return;

                            if (castPoint < 0.25)
                                if (UseOnSelf(castPoint, Spells.Manta)) return;

                            if (UseOnSelf(castPoint,
                                Spells.Shift,
                                Spells.Eul,
                                Spells.DefVsDamage,
                                Spells.DefVsMagic,
                                Spells.Invul)) return;
                        }

                        break;
                    }
                }
            }
        }

        public static void Effect() {
            if (Program.Hero.IsSilenced() && !Program.Hero.HasModifier("modifier_rattletrap_hookshot")) {
                if (Program.Menu.Item("blinkSilenced").GetValue<bool>())
                    if (Blink()) return;

                if (UseOnSelf(555,
                    Spells.Eul,
                    Spells.DefVsMagic,
                    Spells.DefVsDamage,
                    Spells.Invis)) return;

                if (!Program.Hero.HasModifiers(new []{ "modifier_riki_smoke_screen", "modifier_disruptor_static_storm" })) {
                    if (UseOnSelf(555, 
                        Spells.Manta,
                        Spells.Lotus,
                        Spells.Greaves)) return;

                    if (Program.Menu.Item("diffusal").GetValue<bool>())
                        if (UseOnSelf(555, Spells.Diffusal)) return;
                }
            }

            if (Program.Hero.IsRooted() && !Program.Hero.HasModifier("modifier_phoenix_sun_ray")) {
                UseOnSelf(555,
                    Spells.Manta,
                    Spells.Greaves,
                    Spells.Eul,
                    Spells.Lotus);
            }
        }

        public static void SpellModifier() {
            var units =
                ObjectManager.GetEntities<Unit>()
                    .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC && x.Team == Program.Hero.GetEnemyTeam());

            foreach (var unit in units) {
                foreach (var modifier in unit.Modifiers) {
                    switch (modifier.Name) {
                        case "modifier_lina_light_strike_array": {
                            if (Program.Hero.Distance2D(unit) <= 250) {
                                var castPoint = 0.5 - modifier.ElapsedTime;

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(castPoint,
                                    Spells.DefVsDisable,
                                    Spells.DefVsDamage,
                                    Spells.DefVsMagic,
                                    Spells.Invis)) return;
                            }

                            break;
                        }
                        case "modifier_kunkka_torrent_thinker": {
                            var elapsedTime = modifier.ElapsedTime;

                            if (Program.Hero.Distance2D(unit) <= 250 && elapsedTime > 1) {
                                var castPoint = 1.6 - elapsedTime;

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(castPoint,
                                    Spells.DefVsDisable,
                                    Spells.DefVsDamage,
                                    Spells.DefVsMagic,
                                    Spells.Invis)) return;
                            }

                            break;
                        }
                        case "modifier_leshrac_split_earth_thinker": {
                            if (Program.Hero.Distance2D(unit) <= 250) {
                                var castPoint = 0.35 - modifier.ElapsedTime;

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(castPoint,
                                    Spells.DefVsDisable,
                                    Spells.DefVsMagic,
                                    Spells.DefVsDamage,
                                    Spells.Invis)) return;
                            }

                            break;
                        }
                        case "modifier_bloodseeker_bloodbath_thinker": {
                            var elapsedTime = modifier.ElapsedTime;

                            if (Program.Hero.Distance2D(unit) <= 250 && elapsedTime > 2) {
                                var castPoint = 2.6 - modifier.ElapsedTime;

                                if (Blink(castPoint)) return;

                                if (UseOnSelf(castPoint,
                                    Spells.DefVsDisable,
                                    Spells.DefVsMagic,
                                    Spells.DefVsDamage,
                                    Spells.Invis)) return;
                            }

                            break;
                        }
                    }
                }
            }
        }

        public static void Modifier() {
            foreach (var modifier in Program.Hero.Modifiers) {
                switch (modifier.Name) {
                    case "modifier_lina_laguna_blade": {
                        var lina =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(
                                    x =>
                                        x.ClassID == ClassID.CDOTA_Unit_Hero_Lina && x.IsAlive &&
                                        x.Team == Program.Hero.GetEnemyTeam());

                        if (lina != null)
                            if (UseOnTarget(lina, 0.25, Spells.Fist)) return;

                        if (UseOnSelf(0.25,
                            Spells.Shift,
                            Spells.Eul)) return;

                        break;
                    }
                    case "modifier_lion_finger_of_death": {
                        var lion =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(
                                    x =>
                                        x.ClassID == ClassID.CDOTA_Unit_Hero_Lion && x.IsAlive &&
                                        x.Team == Program.Hero.GetEnemyTeam());

                        if (lion != null)
                            if (UseOnTarget(lion, 0.25, Spells.Fist)) return;

                        if (UseOnSelf(0.25,
                            Spells.Shift,
                            Spells.Eul)) return;

                        break;
                    }
                    case "modifier_spirit_breaker_charge_of_darkness_vision": {
                        var bara =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(
                                    x =>
                                        x.ClassID == ClassID.CDOTA_Unit_Hero_SpiritBreaker && x.IsAlive &&
                                        x.Team == Program.Hero.GetEnemyTeam());

                        if (bara != null) {
                            var distance = Program.Hero.Distance2D(bara);

                            if (distance <= 500) {
                                if (UseOnSelf(distance / bara.Spellbook.SpellQ.GetProjectileSpeed(),
                                    Spells.Shift,
                                    Spells.Eul,
                                    Spells.DefVsDamage,
                                    Spells.DefVsMagic,
                                    Spells.Invis)) return;
                            }

                            if (UseOnTarget(bara, Program.Hero.Distance2D(bara),
                                Spells.Eul,
                                Spells.InstaDisable,
                                Spells.Invul)) return;
                        }
                        break;
                    }
                    case "modifier_earth_spirit_magnetize": {
                        if (UseOnSelf(modifier.RemainingTime,
                            Spells.Manta,
                            Spells.Eul,
                            Spells.DefVsDamage,
                            Spells.DefVsMagic,
                            Spells.Invul)) return;
                        break;
                    }
                    case "modifier_tinker_laser_blind": {
                        if (UseOnSelf(modifier.RemainingTime, Spells.Manta)) return;
                        break;
                    }
                    case "modifier_item_diffusal_blade_slow": {
                        if (UseOnSelf(modifier.RemainingTime, Spells.Manta)) return;
                        break;
                    }
                }
            }
        }

        public static void PanicEscape() {
            var enemyNear =
                ObjectManager.GetEntities<Hero>()
                    .Any(
                        x =>
                            x.IsAlive && !x.IsIllusion && x.Team == Program.Hero.GetEnemyTeam() &&
                            x.Distance2D(Program.Hero) <= Program.Menu.Item("panicDistance").GetValue<Slider>().Value);

            if (enemyNear)
                Blink();
        }

        private static bool IsCasted(this Ability ability) {
            return ability.Level > 0 && ability.CooldownLength > 0 &&
                   Math.Ceiling(ability.CooldownLength).Equals(Math.Ceiling(ability.Cooldown));
        }

        private static bool Blink(double castpoint = 5, bool forceBlink = false) {
            if (!Program.Menu.Item("blink").GetValue<bool>() || Program.Hero.IsMagicImmune())
                return false;

            castpoint -= 0.1;

            var blink = Program.Hero.Inventory.Items.Concat(Program.Hero.Spellbook.Spells)
                .FirstOrDefault(x => Spells.BlinkAbilities.Any(x.Name.Equals) && x.CanBeCasted());

            if (blink == null)
                return false;

            var castRange = blink.GetCastRange() - 50;

            if (!(blink is Item) && !Program.Hero.CanCast())
                return false;

            var home =
                ObjectManager.GetEntities<Entity>()
                    .FirstOrDefault(x => x.Team == Program.Hero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain) as
                    Unit;

            if (home == null)
                return false;

            var isLeap = blink.ClassID == ClassID.CDOTA_Item_ForceStaff ||
                         blink.ClassID == ClassID.CDOTA_Ability_Mirana_Leap;
            var isInvul = blink.ClassID == ClassID.CDOTA_Ability_EmberSpirit_Activate_FireRemnant ||
                          blink.ClassID == ClassID.CDOTA_Ability_FacelessVoid_TimeWalk;

            if (isLeap) castRange = 60;

            var findangle = Program.Hero.NetworkPosition.ToVector2()
                .FindAngleBetween(home.Position.ToVector2(), true);
            var position = new Vector3(Program.Hero.Position.X + castRange * (float) Math.Cos(findangle),
                Program.Hero.Position.Y + castRange * (float) Math.Sin(findangle), Program.Hero.Position.Z);

            if (blink.ClassID == ClassID.CDOTA_Ability_EmberSpirit_Activate_FireRemnant) {
                if (Program.Hero.HasModifier("modifier_ember_spirit_fire_remnant_timer"))
                    castpoint = 0.30;
                else return false;
            }

            if (blink.ClassID == ClassID.CDOTA_Ability_Mirana_Leap)
                castpoint -= Program.Hero.GetTurnTime(home);

            if (blink.ClassID == ClassID.CDOTA_Item_ForceStaff)
                castpoint -= 0.08;

            if (Program.Hero.HasModifier("modifier_bloodseeker_rupture") && !isInvul)
                return false;

            var enoughTime = blink.GetCastDelay(Program.Hero, home, true) < castpoint;

            if (!enoughTime &&
                (!forceBlink || blink.ClassID != ClassID.CDOTA_Item_BlinkDagger ||
                 !Program.Menu.Item("forceBlink").GetValue<bool>())) {
                return false;
            }

            if (isLeap) {
                if (Program.Hero.GetTurnTime(home) > 0) {
                    Program.Hero.Move(position);
                    if (blink.IsAbilityBehavior(AbilityBehavior.NoTarget))
                        blink.UseAbility(true);
                    else
                        blink.UseAbility(Program.Hero, true);
                }
                else {
                    if (blink.IsAbilityBehavior(AbilityBehavior.NoTarget))
                        blink.UseAbility();
                    else
                        blink.UseAbility(Program.Hero);
                }
            }
            else {
                if (forceBlink && !enoughTime || blink.ClassID == ClassID.CDOTA_Ability_EmberSpirit_Activate_FireRemnant)
                    position = new Vector3(Program.Hero.Position.X + 100 * (float) Math.Cos(Program.Hero.RotationRad),
                        Program.Hero.Position.Y + 100 * (float) Math.Sin(Program.Hero.RotationRad),
                        Program.Hero.Position.Z);
                blink.UseAbility(position);
            }

            if (Program.Menu.Item("center").GetValue<bool>()) {
                Program.CameraCentered = true;
                Game.ExecuteCommand("+dota_camera_center_on_hero");
            }

            Utils.Sleep(Program.Menu.Item("delay").GetValue<Slider>().Value, "CounterDelay");
            return true;
        }

        private static bool UseOnTarget(Unit target, double castpoint, params string[][] abilitiesNames) {
            if (!Program.Menu.Item("disable").GetValue<bool>())
                return false;

            var canCast = Program.Hero.CanCast();
            var magicImmune = target.IsMagicImmune();

            var ability =
                abilitiesNames.SelectMany(
                    x =>
                        x.Select(y => Program.Hero.FindItem(y) ?? Program.Hero.FindSpell(y))
                            .Where(
                                y =>
                                    y.CanBeCasted() && y.GetCastDelay(Program.Hero, target) < castpoint &&
                                    target.IsValidTarget(y.GetCastRange(), false, Program.Hero.NetworkPosition) &&
                                    (!magicImmune || Spells.IgnoresMagicImmunity.Any(y.Name.Equals)) &&
                                    (y is Item || canCast))).FirstOrDefault();

            if (ability == null)
                return false;

            if (ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
                ability.UseAbility();
            else if (ability.IsAbilityBehavior(AbilityBehavior.UnitTarget))
                ability.UseAbility(target);
            else
                ability.UseAbility(target.NetworkPosition);

            Utils.Sleep(Program.Menu.Item("delay").GetValue<Slider>().Value, "CounterDelay");
            return true;
        }

        private static bool UseOnSelf(double castpoint, params string[][] abilitiesNames) {
            if (Program.Hero.IsMagicImmune())
                return false;

            var canCast = Program.Hero.CanCast();

            var ability =
                abilitiesNames.SelectMany(
                    x =>
                        x.Select(y => Program.Hero.FindItem(y) ?? Program.Hero.FindSpell(y))
                            .Where(y => y.CanBeCasted() && y.FindCastPoint() < castpoint && (y is Item || canCast)))
                    .FirstOrDefault();

            if (ability == null)
                return false;

            if (ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
                ability.UseAbility();
            else if (ability.IsAbilityBehavior(AbilityBehavior.UnitTarget))
                ability.UseAbility(Program.Hero);
            else
                ability.UseAbility(Program.Hero.NetworkPosition);

            Utils.Sleep(Program.Menu.Item("delay").GetValue<Slider>().Value, "CounterDelay");
            return true;
        }

    }
}