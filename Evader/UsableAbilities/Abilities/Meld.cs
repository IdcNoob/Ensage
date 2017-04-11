namespace Evader.UsableAbilities.Abilities
{
    using System;
    using System.Linq;

    using Base;

    using Core;
    using Core.Menus;

    using Data;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Meld : NotTargetable, IDisposable
    {
        private readonly Sleeper sleeper;

        public Meld(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            sleeper = new Sleeper();
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        private static UsableAbilitiesMenu Menu => Variables.Menu.UsableAbilities;

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            base.Use(ability, target);
            if (Menu.MeldBlock)
            {
                sleeper.Sleep(Menu.MeldBlockTime);
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!sleeper.Sleeping || !args.Entities.Contains(Hero) || !args.IsPlayerInput)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AttackLocation:
                case OrderId.AttackTarget:
                case OrderId.Stop:
                case OrderId.Hold:
                case OrderId.MoveTarget:
                case OrderId.MoveLocation:
                    args.Process = false;
                    break;
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                    sleeper.Sleep(0);
                    break;
            }
        }
    }
}