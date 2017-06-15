namespace Evader.EvadableAbilities.Heroes.StormSpirit
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Vortex : LinearTarget, IModifier
    {
        private readonly float aghanimRadius;

        public Vortex(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(NetherWard);

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            aghanimRadius = Ability.AbilitySpecialData.First(x => x.Name == "radius_scepter").Value + 50;
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay;
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + 150);
                Obstacle = AbilityOwner.AghanimState()
                               ? Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle)
                               : Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), AbilityOwner.Position);

            if (AbilityOwner.AghanimState())
            {
                AbilityDrawer.DrawCircle(StartPosition, GetRadius());
            }
            else
            {
                AbilityDrawer.DrawRectangle(StartPosition, EndPosition, GetRadius());
            }
        }

        protected override float GetRadius()
        {
            return AbilityOwner.AghanimState() ? aghanimRadius : base.GetRadius();
        }
    }
}