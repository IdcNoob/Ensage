namespace ItemManager.Core.Abilities.Base
{
    using System.Linq;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    internal class Shrine : UsableAbility, IRecoveryAbility
    {
        public Shrine(Manager manager)
            : base(null, manager)
        {
            var specialData = Ability.GetAbilityDataById(AbilityId.filler_ability).AbilitySpecialData.ToList();
            var duration = specialData.First(x => x.Name == "duration").Value;
            HealthRestore = specialData.First(x => x.Name == "hp_heal").Value * duration;
            ManaRestore = specialData.First(x => x.Name == "mp_heal").Value * duration;

            PowerTreadsAttribute = Attribute.Agility;
            RestoredStats = RestoredStats.All;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

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