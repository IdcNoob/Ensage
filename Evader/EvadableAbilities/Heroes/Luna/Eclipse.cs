namespace Evader.EvadableAbilities.Heroes.Luna
{
    using System.Linq;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    using AbilityType = Data.AbilityType;

    internal class Eclipse : AOE
    {
        private readonly float[] duration = new float[3];

        private readonly float[] durationAghanim = new float[3];

        public Eclipse(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);

            for (var i = 0u; i < duration.Length; i++)
            {
                duration[i] = ability.AbilitySpecialData.First(x => x.Name == "duration_tooltip").GetValue(i);
                durationAghanim[i] = ability.AbilitySpecialData.First(x => x.Name == "duration_tooltip_scepter")
                    .GetValue(i);
            }
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint + GetEclipseDuration();
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !CanBeStopped())
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, AbilityOwner.NetworkPosition, GetRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), AbilityOwner.Position);
            AbilityDrawer.DrawCircle(StartPosition, GetRadius());

            AbilityDrawer.UpdateCirclePosition(AbilityOwner.NetworkPosition);
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + CastPoint - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return ability.Type != AbilityType.Disable && AbilityOwner.HasModifier("modifier_luna_eclipse");
        }

        private float GetEclipseDuration()
        {
            var level = Ability.Level - 1;
            return AbilityOwner.AghanimState() ? durationAghanim[level] : duration[level];
        }
    }
}