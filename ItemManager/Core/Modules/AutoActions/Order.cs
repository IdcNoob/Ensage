namespace ItemManager.Core.Modules.AutoActions
{
    using Ensage;

    using SharpDX;

    internal class Order
    {
        public void Recast(ExecuteOrderEventArgs args)
        {
            args.Process = false;
            var ability = args.Ability;

            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                {
                    var target = args.Target as Unit;
                    if (target != null && target.IsValid && target.IsAlive)
                    {
                        UseAbility(ability, target);
                    }
                    break;
                }
                case OrderId.AbilityLocation:
                {
                    UseAbility(ability, args.TargetPosition);
                    break;
                }
                case OrderId.Ability:
                {
                    UseAbility(ability);
                    break;
                }
                case OrderId.ToggleAbility:
                {
                    ToggleAbility(ability);
                    break;
                }
            }
        }

        public void ToggleAbility(Ability ability)
        {
            ability.ToggleAbility();
        }

        public void UseAbility(Ability ability, Unit target)
        {
            ability.UseAbility(target);
        }

        public void UseAbility(Ability ability, Vector3 location)
        {
            ability.UseAbility(location);
        }

        public void UseAbility(Ability ability)
        {
            ability.UseAbility();
        }
    }
}