namespace Evader.UsableAbilities.External
{
    using System;

    using Common;

    using Core;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;

    using Utils = Ensage.Common.Utils;

    internal class GoldSpender
    {
        private readonly Sleeper sleeper = new Sleeper();

        private static Hero Hero => Variables.Hero;

        public bool ShouldSpendGold(EvadableAbility ability)
        {
            if (sleeper.Sleeping)
            {
                return false;
            }

            float damage;

            try
            {
                damage = (int)Math.Round(AbilityDamage.CalculateDamage(ability.Ability, ability.AbilityOwner, Hero));
            }
            catch (Exception)
            {
                return false;
            }

            if (damage > 850)
            {
                Debugger.WriteLine("// * Damage calculations probably incorrect // " + damage + " // " + ability.Name);
                damage = 350;
            }

            //  Debugger.WriteLine("// * Incoming damage: " + damage + " from: " + ability.Name);
            //  Debugger.WriteLine("// * HP left: " + Hero.Health);

            return Hero.Health <= damage;
        }

        public void Spend()
        {
            Utils.Sleep(1000, "GoldSpender.ForceSpend");
            sleeper.Sleep(1000);
        }
    }
}