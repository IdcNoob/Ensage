namespace Evader.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using Core.Menus;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.Common.Threading;

    using EvadableAbilities.Base;

    using UsableAbilities.Base;
    using UsableAbilities.External;

    using AbilityType = Data.AbilityType;

    internal class AbilityUpdater : IDisposable
    {
        #region Fields

        private readonly List<uint> addedAbilities = new List<uint>();

        private readonly AllyAbilities allyAbilitiesData;

        private readonly EnemyAbilities enemyAbilitiesData;

        private readonly Sleeper sleeper;

        private bool processing;

        #endregion

        #region Constructors and Destructors

        public AbilityUpdater()
        {
            sleeper = new Sleeper();
            allyAbilitiesData = new AllyAbilities();
            enemyAbilitiesData = new EnemyAbilities();

            Menu.UsableAbilities.AddAbility(AbilityNames.GoldSpender, AbilityType.Counter);
            GoldSpender = new GoldSpender();

            Debugger.WriteLine("* Evader =>");
            Debugger.WriteLine("* Total abilities countered: " + enemyAbilitiesData.EvadableAbilities.Count);
            Debugger.WriteLine("* Total usable blink abilities: " + allyAbilitiesData.BlinkAbilities.Count);
            Debugger.WriteLine("* Total usable counter abilities: " + allyAbilitiesData.CounterAbilities.Count);
            Debugger.WriteLine("* Total usable disable abilities: " + allyAbilitiesData.DisableAbilities.Count);

            sleeper.Sleep(1000);

            GameDispatcher.OnUpdate += OnUpdate;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
        }

        #endregion

        #region Public Properties

        public List<EvadableAbility> EvadableAbilities { get; } = new List<EvadableAbility>();

        public GoldSpender GoldSpender { get; }

        public List<UsableAbility> UsableAbilities { get; } = new List<UsableAbility>();

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static Team HeroTeam => Variables.HeroTeam;

        private static MenuManager Menu => Variables.Menu;

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            GameDispatcher.OnUpdate -= OnUpdate;

            foreach (var disposable in UsableAbilities.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var handle = args.Entity.Handle;

            var disposable = UsableAbilities.FirstOrDefault(x => x.Handle == handle) as IDisposable;
            disposable?.Dispose();

            EvadableAbilities.RemoveAll(x => x.Handle == handle || x.OwnerHandle == handle);
            UsableAbilities.RemoveAll(x => x.Handle == handle);
        }

        public async void OnUpdate(EventArgs args)
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
                            !(x is Building) && x.IsValid && x.IsAlive && x.IsSpawned
                            && (!x.IsIllusion
                                || x.HasModifiers(
                                    new[]
                                    {
                                        "modifier_arc_warden_tempest_double",
                                        "modifier_vengefulspirit_hybrid_special"
                                    },
                                    false))))
            {
                var abilities = new List<Ability>();

                try
                {
                    abilities.AddRange(unit.Spellbook.Spells.ToList());

                    if (unit.HasInventory)
                    {
                        abilities.AddRange(unit.Inventory.Items);
                    }
                }
                catch (Exception)
                {
                }

                foreach (var ability in
                    abilities.Where(x => x.IsValid && !addedAbilities.Contains(x.Handle) && x.Level > 0))
                {
                    if (unit.Equals(Hero))
                    {
                        var abilityName = ability.Name;

                        Func<Ability, UsableAbility> func;
                        if (allyAbilitiesData.CounterAbilities.TryGetValue(abilityName, out func))
                        {
                            Menu.UsableAbilities.AddAbility(abilityName, AbilityType.Counter);
                            UsableAbilities.Add(func.Invoke(ability));
                        }
                        if (allyAbilitiesData.DisableAbilities.TryGetValue(abilityName, out func))
                        {
                            Menu.UsableAbilities.AddAbility(abilityName, AbilityType.Disable);
                            UsableAbilities.Add(func.Invoke(ability));
                        }
                        if (allyAbilitiesData.BlinkAbilities.TryGetValue(abilityName, out func))
                        {
                            Menu.UsableAbilities.AddAbility(abilityName, AbilityType.Blink);
                            UsableAbilities.Add(func.Invoke(ability));
                        }
                    }
                    else if (unit.Team != HeroTeam || ability.ClassID == ClassID.CDOTA_Ability_FacelessVoid_Chronosphere)
                    {
                        Func<Ability, EvadableAbility> func;
                        if (enemyAbilitiesData.EvadableAbilities.TryGetValue(ability.Name, out func))
                        {
                            var evadableAbility = func.Invoke(ability);
                            await Menu.EnemiesSettings.AddAbility(evadableAbility);
                            EvadableAbilities.Add(evadableAbility);
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