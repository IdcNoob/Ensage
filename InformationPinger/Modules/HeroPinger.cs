namespace InformationPinger.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects;

    internal class HeroPinger
    {
        #region Fields

        private readonly List<uint> pinged = new List<uint>();

        private readonly Random random = new Random();

        #endregion

        #region Constructors and Destructors

        public HeroPinger(Hero hero)
        {
            Hero = hero;
            Handle = hero.Handle;
        }

        #endregion

        #region Public Properties

        public uint Handle { get; }

        public Hero Hero { get; }

        #endregion

        #region Public Methods and Operators

        public void IgnoreCurrentAbilities()
        {
            pinged.AddRange(Hero.Inventory.Items.Select(x => x.Handle));
            pinged.AddRange(Hero.Spellbook.Spells.Select(x => x.Handle));
        }

        public bool ItemPinger(bool wardsEnabled)
        {
            var newItem =
                Hero.Inventory.Items.FirstOrDefault(
                    x =>
                    x.IsValid && !pinged.Contains(x.Handle)
                    && (x.Cost >= 1800 || Variables.IncludedItems.Contains(x.StoredName())
                        || (wardsEnabled
                            && (x.ClassID == ClassID.CDOTA_Item_ObserverWard
                                || x.ClassID == ClassID.CDOTA_Item_SentryWard
                                || x.ClassID == ClassID.CDOTA_Item_Ward_Dispenser))));

            return Announce(newItem);
        }

        #endregion

        #region Methods

        internal bool AbilityPinger()
        {
            var newAbility =
                Hero.Spellbook.Spells.FirstOrDefault(
                    x =>
                    x.IsValid && !pinged.Contains(x.Handle) && x.Level > 0
                    && Variables.Abilities.Contains(x.StoredName()));

            return Announce(newAbility);
        }

        private bool Announce(Ability ability)
        {
            if (ability == null)
            {
                return false;
            }

            ability.Announce();

            pinged.Add(ability.Handle);
            Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            return true;
        }

        private bool Announce(Item item)
        {
            if (item == null)
            {
                return false;
            }

            Network.EnemyItemAlert(item);

            pinged.Add(item.Handle);
            Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            return true;
        }

        #endregion
    }
}