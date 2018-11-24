namespace Evader.UsableAbilities.Base
{
    using System;

    using Data;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class OrbAbility : UsableAbility, IDisposable
    {
        private bool bladeMailActive;

        private bool callActive;

        private bool reenableOrb;

        private bool taunted;

        public OrbAbility(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            Unit.OnModifierAdded += OnModifierAdded;
            Unit.OnModifierRemoved += OnModifierRemoved;
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return false;
        }

        public void Dispose()
        {
            Unit.OnModifierAdded -= OnModifierAdded;
            Unit.OnModifierRemoved -= OnModifierRemoved;
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return 0;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            SetState(sender, args.Modifier.Name, true);

            if (bladeMailActive && callActive && taunted && Ability.IsAutoCastEnabled)
            {
                Ability.ToggleAutocastAbility();
                reenableOrb = true;
            }
        }

        private void OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            SetState(sender, args.Modifier.Name, false);

            if (!bladeMailActive && reenableOrb)
            {
                if (!Ability.IsAutoCastEnabled)
                {
                    Ability.ToggleAutocastAbility();
                }

                reenableOrb = false;
            }
        }

        private void SetState(Entity sender, string modifierName, bool added)
        {
            switch (modifierName)
            {
                case "modifier_axe_berserkers_call_armor":
                    if (sender.NetworkName == "CDOTA_Unit_Hero_Axe" && sender.Team != HeroTeam)
                    {
                        callActive = added;
                    }
                    break;
                case "modifier_axe_berserkers_call":
                    if (sender.Handle == Hero.Handle)
                    {
                        taunted = added;
                    }
                    break;
                case "modifier_item_blade_mail_reflect":
                    if (sender.NetworkName == "CDOTA_Unit_Hero_Axe" && sender.Team != HeroTeam)
                    {
                        bladeMailActive = added;
                    }
                    break;
            }
        }
    }
}