namespace ItemManager.Core.Modules.HpMpAbuse.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;

    internal class AbilityUpdater
    {
        private static Hero Hero;

        private readonly List<uint> addedAbilities = new List<uint>();

        public AbilityUpdater()
        {
            Hero = ObjectManager.LocalHero;
            var abilities = Hero.Spellbook.Spells.ToList();
            abilities.AddRange(
                ObjectManager.GetEntities<Item>().Where(x => x.IsValid && x.Owner?.Handle == Hero.Handle));

            foreach (var ability in abilities)
            {
                if (IsValidAbility(ability))
                {
                    //    Menu.AddAbility(ability.StoredName());
                }
                addedAbilities.Add(ability.Handle);
            }

            ObjectManager.OnAddEntity += ObjectManagerOnAddEntity;
        }

        public List<ClassId> IgnoredAbilities { get; } = new List<ClassId>
        {
            ClassId.CDOTA_Item_TeleportScroll,
            ClassId.CDOTA_Item_BootsOfTravel,
            ClassId.CDOTA_Item_BootsOfTravel_2
        };

        //    private static MenuManager Menu;

        public void OnClose()
        {
            ObjectManager.OnAddEntity -= ObjectManagerOnAddEntity;
        }

        private bool IsValidAbility(Ability ability)
        {
            return ability.IsValid && !ability.IsHidden && ability.GetManaCost(0) > 0
                   && !IgnoredAbilities.Contains(ability.ClassId) && !addedAbilities.Contains(ability.Handle);
        }

        private void ObjectManagerOnAddEntity(EntityEventArgs args)
        {
            var ability = args.Entity as Ability;

            if (ability == null || ability.Owner?.Handle != Hero.Handle || !IsValidAbility(ability))
            {
            }

            //    Menu.AddAbility(ability.StoredName());
        }
    }
}