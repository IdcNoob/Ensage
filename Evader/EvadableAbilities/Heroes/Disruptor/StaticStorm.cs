namespace Evader.EvadableAbilities.Heroes.Disruptor
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    using AbilityType = Data.AbilityType;

    internal class StaticStorm : AOE, IModifierObstacle
    {
        private readonly float duration, durationAghanim;

        private Modifier modifierThinker;

        public StaticStorm(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;
            ObstacleStays = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);

            duration = Ability.AbilitySpecialData.First(x => x.Name == "duration").Value;
            durationAghanim = Ability.AbilitySpecialData.First(x => x.Name == "duration_scepter").Value;
        }

        public void AddModifierObstacle(Modifier mod, Unit unit)
        {
            modifierThinker = mod;
            StartPosition = unit.Position;
            StartCast = Game.RawGameTime;
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (modifierThinker == null)
            {
                return;
            }

            if (Obstacle != null)
            {
                if (StartCast > 0 && Game.RawGameTime > EndCast)
                {
                    End();
                }
                return;
            }

            EndCast = Game.RawGameTime + (GetDuration() - modifierThinker.ElapsedTime);
            Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            modifierThinker = null;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + CastPoint - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return ability.Type != AbilityType.Disable && modifierThinker != null;
        }

        private float GetDuration()
        {
            return AbilityOwner.AghanimState() ? durationAghanim : duration;
        }
    }
}