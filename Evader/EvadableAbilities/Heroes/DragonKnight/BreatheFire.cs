namespace Evader.EvadableAbilities.Heroes.DragonKnight
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class BreatheFire : LinearProjectile, IModifier
    {
        public BreatheFire(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(AphoticShield);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);
            Modifier.AllyCounterAbilities.Add(Manta);
        }

        public EvadableModifier Modifier { get; }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast <= 0 && TimeSinceCast() < 0.1 && AbilityOwner.IsVisible && Obstacle == null)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 200;
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 300;
        }
    }
}