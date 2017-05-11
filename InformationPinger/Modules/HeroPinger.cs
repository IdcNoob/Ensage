namespace InformationPinger.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Items;

    internal class HeroPinger
    {
        private readonly Dictionary<uint, RuneType> bottleRunePinged = new Dictionary<uint, RuneType>();

        private readonly bool isRubick;

        private readonly List<uint> pinged = new List<uint>();

        private readonly Random random = new Random();

        public HeroPinger(Hero hero)
        {
            Hero = hero;
            Handle = hero.Handle;
            isRubick = hero.ClassId == ClassId.CDOTA_Unit_Hero_Rubick;
        }

        public uint Handle { get; }

        public Hero Hero { get; }

        public bool ShouldPing => Hero.IsVisible && Hero.IsAlive;

        public bool AbilityPinger(bool doublePing, bool checkEnemies, bool rubickDisable, bool rubickUltimate)
        {
            if (checkEnemies && OtherEnemiesNear())
            {
                return false;
            }

            var newAbility = Hero.Spellbook.Spells.FirstOrDefault(
                x => x.IsValid && !x.IsHidden && !pinged.Contains(x.Handle)
                     && (Variables.Abilities.Contains(x.StoredName()) || isRubick && x.AbilitySlot == AbilitySlot.Slot_4
                         && (rubickDisable && x.IsDisable() || rubickUltimate && x.AbilityType == AbilityType.Ultimate))
                     && x.Level > 0);

            return Announce(newAbility, doublePing);
        }

        public bool BottledRunePinger(bool doublePing, bool checkEnemies, IEnumerable<RuneType> runes)
        {
            var bottle = Hero.FindItem("item_bottle") as Bottle;
            if (bottle == null)
            {
                return false;
            }

            var storedRune = bottle.StoredRune;
            var handle = bottle.Handle;

            RuneType lastRune;
            if (bottleRunePinged.TryGetValue(handle, out lastRune) && lastRune == storedRune)
            {
                return false;
            }

            if (!runes.Contains(storedRune))
            {
                bottleRunePinged[handle] = storedRune;
                return false;
            }

            if (checkEnemies && OtherEnemiesNear())
            {
                return false;
            }

            bottleRunePinged[handle] = storedRune;

            return Announce(bottle, doublePing, true);
        }

        public void IgnoreCurrentAbilities()
        {
            pinged.AddRange(Hero.Inventory.Items.Select(x => x.Handle));
            pinged.AddRange(Hero.Spellbook.Spells.Select(x => x.Handle));
        }

        public bool ItemPinger(bool doublePing, bool checkEnemies, float cost, IEnumerable<string> forcePingItems)
        {
            if (checkEnemies && OtherEnemiesNear())
            {
                return false;
            }

            var newItem = Hero.Inventory.Items.FirstOrDefault(
                x => x.IsValid && !pinged.Contains(x.Handle)
                     && (x.Cost >= cost || forcePingItems.Contains(x.StoredName())));

            return Announce(newItem, doublePing);
        }

        private bool Announce(Ability ability, bool doublePing)
        {
            if (ability == null)
            {
                return false;
            }

            DelayAction.Add(
                750,
                () =>
                    {
                        ability.Announce();

                        if (doublePing)
                        {
                            DelayAction.Add(250, () => ability.Announce());
                            Variables.Sleeper.Sleep(random.Next(3333, 4333), "CanPing");
                        }
                        else
                        {
                            Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
                        }
                    });

            pinged.Add(ability.Handle);
            return true;
        }

        private bool Announce(Item item, bool doublePing, bool ignoreList = false)
        {
            if (item == null)
            {
                return false;
            }

            DelayAction.Add(
                750,
                () =>
                    {
                        Network.EnemyItemAlert(item);

                        if (doublePing)
                        {
                            DelayAction.Add(250, () => Network.EnemyItemAlert(item));
                            Variables.Sleeper.Sleep(random.Next(3333, 4333), "CanPing");
                        }
                        else
                        {
                            Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
                        }
                    });

            if (!ignoreList)
            {
                pinged.Add(item.Handle);
            }

            return true;
        }

        private bool OtherEnemiesNear()
        {
            return ObjectManager.GetEntitiesParallel<Unit>()
                .Any(
                    x => x.IsValid && !x.Equals(Hero) && x.IsAlive && x.Team == Variables.EnemyTeam
                         && x.Distance2D(Variables.Hero) <= 700 && x.Distance2D(Hero) >= 700);
        }
    }
}