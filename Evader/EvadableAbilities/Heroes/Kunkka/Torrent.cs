namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class Torrent : AOE, IModifierObstacle
    {
        #region Fields

        private Modifier modifierThinker;

        #endregion

        #region Constructors and Destructors

        public Torrent(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
        }

        #endregion

        #region Public Methods and Operators

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
                if (GetRemainingTime() <= 0)
                {
                    End();
                }
                return;
            }

            EndCast = Game.RawGameTime + (AdditionalDelay - modifierThinker.ElapsedTime);
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
            return EndCast - Game.RawGameTime;
        }

        #endregion
    }
}