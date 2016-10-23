namespace Evader.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;

    using UsableAbilities.Base;

    internal class AbilityUpdater
    {
        #region Fields

        private readonly List<uint> addedAbilities = new List<uint>();

        //todo: move lists from core
        private readonly List<EvadableAbility> evadableAbilities;

        private readonly Sleeper sleeper;

        private readonly List<UsableAbility> usableAbilities;

        private bool processing;

        #endregion

        #region Constructors and Destructors

        public AbilityUpdater(List<UsableAbility> usable, List<EvadableAbility> evadable)
        {
            usableAbilities = usable;
            evadableAbilities = evadable;
            sleeper = new Sleeper();

            Game.OnUpdate += Update;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
        }

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static Team HeroTeam => Variables.HeroTeam;

        private static MenuManager Menu => Variables.Menu;

        #endregion

        #region Public Methods and Operators

        public void Close()
        {
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Game.OnUpdate -= Update;

            addedAbilities.Clear();
            evadableAbilities.Clear();
            usableAbilities.Clear();
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var handle = args.Entity.Handle;

            evadableAbilities.RemoveAll(x => x.Handle == handle || x.OwnerHandle == handle);
            usableAbilities.RemoveAll(x => x.Handle == handle);
        }

        public async void Update(EventArgs args)
        {
            if (processing || sleeper.Sleeping || !Game.IsInGame)
            {
                return;
            }

            processing = true;

            foreach (var unit in
                ObjectManager.GetEntitiesParallel<Unit>()
                    .Where(
                        x =>
                        (x is Hero || x is Creep) && x.IsValid && x.IsAlive && x.IsSpawned
                        && (!x.IsIllusion
                            || x.HasModifiers(
                                new[] { "modifier_arc_warden_tempest_double", "modifier_vengefulspirit_hybrid_special" },
                                false))))
            {
                var abilities = unit.Spellbook.Spells.ToList();

                if (unit.HasInventory)
                {
                    abilities.AddRange(unit.Inventory.Items);
                }

                foreach (var ability in
                    abilities.Where(x => x.IsValid && !addedAbilities.Contains(x.Handle) && x.Level > 0))
                {
                    if (unit.Equals(Hero))
                    {
                        var abilityName = ability.Name;

                        Func<Ability, UsableAbility> func;
                        if (Abilities.EvadeCounterAbilities.TryGetValue(abilityName, out func))
                        {
                            Menu.AddUsableCounterAbility(abilityName);
                            usableAbilities.Add(func.Invoke(ability));
                        }
                        if (Abilities.EvadeDisableAbilities.TryGetValue(abilityName, out func))
                        {
                            usableAbilities.Add(func.Invoke(ability));
                            Menu.AddUsableDisableAbility(abilityName);
                        }
                        if (Abilities.EvadeBlinkAbilities.TryGetValue(abilityName, out func))
                        {
                            usableAbilities.Add(func.Invoke(ability));
                            Menu.AddUsableBlinkAbility(abilityName);
                        }
                    }
                    else if (unit.Team != HeroTeam || ability.ClassID == ClassID.CDOTA_Ability_FacelessVoid_Chronosphere)
                    {
                        Func<Ability, EvadableAbility> func;
                        if (Abilities.EvadableAbilities.TryGetValue(ability.Name, out func))
                        {
                            var evadableAbility = func.Invoke(ability);
                            await Menu.AddEvadableAbility(evadableAbility);
                            evadableAbilities.Add(evadableAbility);
                        }
                    }

                    addedAbilities.Add(ability.Handle);
                }
            }

            sleeper.Sleep(3000);
            processing = false;
        }

        #endregion
    }
}