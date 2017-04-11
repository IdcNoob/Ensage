namespace Evader.EvadableAbilities.Heroes.EmberSpirit
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal class SleightOfFist : AOE, IParticle
    {
        private readonly float jumpTime;

        public SleightOfFist(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;
            jumpTime = Ability.AbilitySpecialData.First(x => x.Name == "attack_interval").Value;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Armlet);

            CounterAbilities.Remove("item_blade_mail");
            CounterAbilities.Remove("nyx_assassin_spiked_carapace");
            CounterAbilities.Remove("windrunner_windrun");
            CounterAbilities.Remove("item_shivas_guard");
            CounterAbilities.Remove("tusk_frozen_sigil");
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (particleArgs.Name.Contains("caster"))
            {
                return;
            }

            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        var units = ObjectManager.GetEntities<Unit>()
                            .Count(
                                x => x.IsValid && !(x is Building) && x.IsAlive && x.Team == HeroTeam && x.IsSpawned
                                     && x.Distance2D(StartPosition) < GetRadius());
                        EndCast = StartCast + units * jumpTime;
                        Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                    });
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Obstacle != null && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return true;
        }
    }
}