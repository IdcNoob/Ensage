namespace HpMpAbuse.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects;

    using Menu;

    internal class AbilityUpdater
    {
        #region Fields

        private readonly List<uint> addedAbilities = new List<uint>();

        #endregion

        #region Constructors and Destructors

        public AbilityUpdater()
        {
            var abilities = Hero.Spellbook.Spells.ToList();
            abilities.AddRange(
                ObjectManager.GetEntities<Item>().Where(x => x.IsValid && x.Owner?.Handle == Hero.Handle));

            foreach (var ability in abilities)
            {
                if (IsValidAbility(ability))
                {
                    Menu.AddAbility(ability.StoredName());
                }
                addedAbilities.Add(ability.Handle);
            }

            ObjectManager.OnAddEntity += ObjectManagerOnAddEntity;
        }

        #endregion

        #region Public Properties

        public List<ClassID> IgnoredAbilities { get; } = new List<ClassID>
        {
            ClassID.CDOTA_Item_TeleportScroll,
            ClassID.CDOTA_Item_BootsOfTravel,
            ClassID.CDOTA_Item_BootsOfTravel_2
        };

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static MenuManager Menu => Variables.Menu;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            ObjectManager.OnAddEntity -= ObjectManagerOnAddEntity;
        }

        #endregion

        #region Methods

        private bool IsValidAbility(Ability ability)
        {
            return ability.IsValid && !ability.IsHidden && ability.GetManaCost(0) > 0
                   && !IgnoredAbilities.Contains(ability.ClassID) && !addedAbilities.Contains(ability.Handle);
        }

        private void ObjectManagerOnAddEntity(EntityEventArgs args)
        {
            var ability = args.Entity as Ability;

            if (ability == null || ability.Owner?.Handle != Hero.Handle || !IsValidAbility(ability))
            {
                return;
            }

            Menu.AddAbility(ability.StoredName());
        }

        #endregion
    }
}