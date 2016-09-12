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

        public bool IsVisible => Hero.IsVisible;

        #endregion

        #region Public Methods and Operators

        public void IgnoreCurrentAbilities()
        {
            pinged.AddRange(Hero.Inventory.Items.Select(x => x.Handle));
            pinged.AddRange(Hero.Spellbook.Spells.Select(x => x.Handle));
        }

        public bool ItemPinger(bool wardsEnabled, bool doublePing)
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

            return Announce(newItem, doublePing);
        }

        #endregion

        #region Methods

        internal bool AbilityPinger(bool doublePing)
        {
            var newAbility =
                Hero.Spellbook.Spells.FirstOrDefault(
                    x =>
                    x.IsValid && !pinged.Contains(x.Handle) && Variables.Abilities.Contains(x.StoredName())
                    && x.Level > 0);

            return Announce(newAbility, doublePing);
        }

        private bool Announce(Ability ability, bool doublePing)
        {
            if (ability == null)
            {
                return false;
            }

            ability.Announce();

            if (doublePing)
            {
                ability.Announce();
                Variables.Sleeper.Sleep(random.Next(3333, 4333), "CanPing");
            }
            else
            {
                Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            }

            pinged.Add(ability.Handle);
            return true;
        }

        private bool Announce(Item item, bool doublePing)
        {
            if (item == null)
            {
                return false;
            }

            Network.EnemyItemAlert(item);

            if (doublePing)
            {
                Network.EnemyItemAlert(item);
                Variables.Sleeper.Sleep(random.Next(3333, 4333), "CanPing");
            }
            else
            {
                Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            }

            pinged.Add(item.Handle);
            return true;
        }

        #endregion
    }
}