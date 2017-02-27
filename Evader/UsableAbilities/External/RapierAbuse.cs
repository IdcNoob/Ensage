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

    internal class RapierAbuse
    {
        #region Fields

        private readonly Sleeper sleeper = new Sleeper();

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        #endregion

        #region Public Methods and Operators

        public void Disassemble()
        {
            Utils.Sleep(1000, "ItemManager.ForceRapierDisassemble");
            sleeper.Sleep(1000);
        }

        public bool ShouldForceRapierDisassemble(EvadableAbility ability)
        {
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

            return Hero.Health <= damage || ability.IsDisable;
        }

        #endregion
    }
}