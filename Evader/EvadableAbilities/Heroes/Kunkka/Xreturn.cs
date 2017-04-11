namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using System;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;
    using Modifiers.Special;

    using static Data.AbilityNames;

    internal class Xreturn : AOE, IModifier
    {
        public Xreturn(Ability ability)
            : base(ability)
        {
            Modifier = new XmarkModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            DisableTimeSinceCastCheck = true;
            DisablePathfinder = true;

            BlinkAbilities.Add("morphling_waveform");
            BlinkAbilities.Add("sandking_burrowstrike");
            BlinkAbilities.Add("faceless_void_time_walk");
            BlinkAbilities.Add("phantom_lancer_doppelwalk");
            BlinkAbilities.Add("ember_spirit_activate_fire_remnant");

            CounterAbilities.Add(Eul);
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            if (Modifier.IsValid() && Obstacle == null)
            {
                StartPosition = Modifier.GetSource().Position;
                EndCast = Game.RawGameTime + Modifier.GetRemainingTime() - 0.01f;
                Obstacle = Pathfinder.AddObstacle(StartPosition, 75, Obstacle, (int)Modifier.GetSource().Position.Z);
            }
            else if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint;
            }
            else if (Obstacle != null && (Game.RawGameTime > EndCast || !Modifier.IsValid()))
            {
                EndXreturn();
            }
            else if (Obstacle != null && Modifier.IsValid())
            {
                StartPosition = Modifier.GetSource().Position;
                Pathfinder.UpdateObstacle(Obstacle.Value, StartPosition, 75, (int)Modifier.GetSource().Position.Z);
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);
        }

        public override void End()
        {
            // phase stop fix
        }

        public void EndXreturn()
        {
            base.End();
            Modifier.Remove();
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            var modRemainingTime = 4f;
            if (Modifier.IsValid())
            {
                modRemainingTime = Modifier.GetRemainingTime();
            }

            var phaseRemainingTime = 4f;
            if (StartCast > 0)
            {
                phaseRemainingTime = EndCast - Game.RawGameTime + 0.05f;
            }

            return Math.Min(phaseRemainingTime, modRemainingTime);
        }

        public override bool IsStopped()
        {
            if (StartCast > 0 && !IsInPhase && CanBeStopped())
            {
                StartCast = 0;
                EndCast = Game.RawGameTime + Modifier.GetRemainingTime() - 0.01f;
                return true;
            }

            return false;
        }
    }
}