namespace HpMpAbuse.Helpers
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Fountain
    {
        #region Fields

        private readonly Unit fountain;

        #endregion

        #region Constructors and Destructors

        public Fountain(Team heroTeam)
        {
            fountain =
                ObjectManager.GetEntities<Unit>()
                    .First(x => x.Name == "dota_fountain" && x.Team == heroTeam);
        }

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static MultiSleeper Sleeper => Variables.Sleeper;

        #endregion

        #region Public Methods and Operators

        public bool BottleCanBeRefilled()
        {
            if (Hero.Distance2D(fountain) < 1300)
            {
                return true;
            }

            if (!Hero.HasModifier(Modifiers.FountainRegeneration))
            {
                return false;
            }

            if (!Sleeper.Sleeping("FountainRegeneration"))
            {
                Sleeper.Sleep(5000, "FountainRegeneration");
                Sleeper.Sleep(2000, "CanRefill");
            }

            return Sleeper.Sleeping("CanRefill");
        }

        #endregion
    }
}