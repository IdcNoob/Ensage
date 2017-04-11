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

    internal class PhaseShift : NotTargetable, IDisposable
    {
        private readonly float[] shiftDuration = new float[4];

        private readonly Sleeper sleeper;

        public PhaseShift(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            sleeper = new Sleeper();
            Player.OnExecuteOrder += OnExecuteOrder;

            for (var i = 0u; i < shiftDuration.Length; i++)
            {
                shiftDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i) * 1000;
            }
        }

        private static UsableAbilitiesMenu Menu => Variables.Menu.UsableAbilities;

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            base.Use(ability, target);
            if (Menu.PhaseShiftBlock)
            {
                sleeper.Sleep(Math.Min(Menu.PhaseShiftBlockTime, GetShiftDuration()));
            }
        }

        private float GetShiftDuration()
        {
            return shiftDuration[Ability.Level - 1];
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