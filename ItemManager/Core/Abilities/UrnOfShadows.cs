namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.SDK.Extensions;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    [Ability(AbilityId.item_urn_of_shadows)]
    internal class UrnOfShadows : OffensiveAbility, IRecoveryAbility
    {
        private readonly Item urnOfShadows;

        public UrnOfShadows(Ability ability, Manager manager)
            : base(ability, manager)
        {
            urnOfShadows = ability as Item;

            ManaRestore = 0;

            var duration = ability.AbilitySpecialData.First(x => x.Name == "duration").Value;
            HealthRestore = ability.AbilitySpecialData.First(x => x.Name == "soul_heal_amount").Value * duration;

            PowerTreadsAttribute = Attribute.Agility;
            RestoredStats = RestoredStats.Health;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && urnOfShadows.CurrentCharges > 0;
        }

        public override bool CanBeCasted(Unit target)
        {
            return base.CanBeCasted(target) && urnOfShadows.CurrentCharges > 0 && !target.HasModifier(ModifierUtils.UrnDebuff)
                   && !Manager.MyHero.HasModifier(ModifierUtils.SpiritVesselDebuff);
        }

        public virtual bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingHealth >= menu.ItemSettingsMenu.UrnOfShadows.HpThreshold;
        }

        void IRecoveryAbility.Use(Unit target, bool queue)
        {
            var regeneration = Manager.MyHero.Hero.GetModifierByName(ModifierUtils.UrnRegeneration)
                               ?? Manager.MyHero.Hero.GetModifierByName(ModifierUtils.SpiritVesselRegeneration);

            if (regeneration?.RemainingTime > 0.15 + (Game.Ping / 1000))
            {
                return;
            }

            Ability.UseAbility(Manager.MyHero.Hero, queue);
            SetSleep(500);
        }
    }
}