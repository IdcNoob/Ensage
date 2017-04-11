namespace ItemManager.Core.Modules.HpMpAbuse.Helpers
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    internal class Fountain
    {
        private readonly Team team;

        private Unit fountain;

        public Fountain(Team heroTeam)
        {
            team = heroTeam;
        }

        private static Hero Hero => ObjectManager.LocalHero;

        private Unit FountainUnit
        {
            get
            {
                return fountain ?? (fountain = ObjectManager.GetEntities<Unit>()
                                        .FirstOrDefault(
                                            x => x.ClassId == ClassId.CDOTA_Unit_Fountain && x.Team == team));
            }
        }

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

            if (!Hero.HasModifier(ModifierUtils.FountainRegeneration))
            {
                return false;
            }

            if (!HpMpAbuse.Sleeper.Sleeping("FountainRegeneration"))
            {
                HpMpAbuse.Sleeper.Sleep(5000, "FountainRegeneration");
                HpMpAbuse.Sleeper.Sleep(2000, "CanRefill");
            }

            return HpMpAbuse.Sleeper.Sleeping("CanRefill");
        }
    }
}