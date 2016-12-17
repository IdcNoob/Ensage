namespace HpMpAbuse.Helpers
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Fountain
    {
        #region Fields

        private readonly Team team;

        private Unit fountain;

        #endregion

        #region Constructors and Destructors

        public Fountain(Team heroTeam)
        {
            team = heroTeam;
        }

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static MultiSleeper Sleeper => Variables.Sleeper;

        private Unit FountainUnit
        {
            get
            {
                return fountain
                       ?? (fountain =
                               ObjectManager.GetEntities<Unit>()
                                   .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == team));
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool BottleCanBeRefilled()
        {
            if (FountainUnit == null)
            {
                return false;
            }

            if (Hero.Distance2D(FountainUnit) < 1300)
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