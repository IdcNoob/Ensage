namespace Evader.UsableAbilities.External
{
    using System;

    using Base;

    using Common;

    using Data;

    using Ensage;
    using Ensage.Common.AbilityInfo;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;
    using Utils = Ensage.Common.Utils;

    internal class GoldSpender : UsableAbility
    {
        #region Constructors and Destructors

        public GoldSpender(string name)
            : base(null, AbilityType.Counter, AbilityCastTarget.Self)
        {
            Name = name;
            IsItem = true;
            Handle = Hero.Handle;
            IgnoresLinkensSphere = true;
            PiercesMagicImmunity = true;
            CastPoint = 0.2f;

            Debugger.WriteLine("///////// UsableAbility // Gold Spender");
            Debugger.WriteLine("// Type: " + Type);
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            if (Sleeper.Sleeping)
            {
                return false;
            }

            var damage = (int)Math.Round(AbilityDamage.CalculateDamage(ability.Ability, ability.AbilityOwner, unit));
            return unit.Health <= damage;
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var damage = (int)Math.Round(AbilityDamage.CalculateDamage(ability.Ability, ability.AbilityOwner, target));

            if (damage > 850)
            {
                Debugger.WriteLine("// * Damage calculations probably incorrect // " + damage);
                damage = 850;
            }

            Debugger.WriteLine("// * Incoming damage: " + damage + " from: " + ability.Name);
            Debugger.WriteLine("// * HP left: " + target.Health + " (" + target.GetName() + ")");

            Utils.Sleep(1000, "GoldSpender.ForceSpend");
            Sleep();
        }

        #endregion
    }
}