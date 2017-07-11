namespace ItemManager.Core.Abilities.Base
{
    using System.Linq;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    internal class Shrine : UsableAbility, IRecoveryAbility
    {
        private readonly float duration;

        private readonly float hpGrow;

        private readonly float hpHeal;

        private readonly float mpGrow;

        private readonly float mpHeal;

        public Shrine(Manager manager)
            : base(null, manager)
        {
            var specialData = Ability.GetAbilityDataById(AbilityId.filler_ability).AbilitySpecialData.ToList();
            hpHeal = specialData.First(x => x.Name == "hp_heal").Value;
            mpHeal = specialData.First(x => x.Name == "mp_heal").Value;
            hpGrow = specialData.First(x => x.Name == "hp_heal_growth").Value;
            mpGrow = specialData.First(x => x.Name == "mp_heal_growth").Value;
            duration = specialData.First(x => x.Name == "duration").Value;

            PowerTreadsAttribute = Attribute.Agility;
            RestoredStats = RestoredStats.All;
        }

        public float HealthRestore => (hpHeal + hpGrow * (Game.RawGameTime / 60)) * duration;

        public float ManaRestore => (mpHeal + mpGrow * (Game.RawGameTime / 60)) * duration;

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public override bool CanBeCasted()
        {
            return Manager.MyHero.HasModifier(ModifierUtils.ShrineRegeneration);
        }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return true;
        }

        public override void Use(Unit target = null, bool queue = false)
        {
            SetSleep(200);
        }
    }
}