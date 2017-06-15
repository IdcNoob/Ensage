namespace Evader.EvadableAbilities.Heroes.Jakiro
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class IcePath : LinearAOE, IModifier
    {
        private readonly float bonusDuration;

        private readonly float[] duration = new float[4];

        private readonly Ability talent;

        public IcePath(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("bane_nightmare");
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            AdditionalDelay = ability.AbilitySpecialData.First(x => x.Name == "path_delay").Value;

            for (var i = 0u; i < duration.Length; i++)
            {
                duration[i] = ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }

            talent = AbilityOwner.FindSpell("special_bonus_unique_jakiro");

            if (talent != null)
            {
                bonusDuration = talent.AbilitySpecialData.First(x => x.Name == "value").Value;
            }

            ObstacleStays = true;
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay + GetDuration();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.InFront(-GetRadius() * 0.9f);
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 5);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        private float GetDuration()
        {
            return duration[Ability.Level - 1] + (talent?.Level > 0 ? bonusDuration : 0);
        }
    }
}