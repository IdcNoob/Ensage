namespace Evader.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core;
    using Core.Menus;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using EvadableAbilities.Base;

    using UsableAbilities.Base;
    using UsableAbilities.External;

    using AbilityType = Data.AbilityType;

    internal class AbilityUpdater : IDisposable
    {
        private readonly List<uint> addedAbilities = new List<uint>();

        private readonly AllyAbilities allyAbilitiesData;

        private readonly EnemyAbilities enemyAbilitiesData;

        private bool processing = true;

        public AbilityUpdater()
        {
            allyAbilitiesData = new AllyAbilities();
            enemyAbilitiesData = new EnemyAbilities();

            Menu.UsableAbilities.AddAbility(AbilityNames.GoldSpender, AbilityType.Counter, false);

            Debugger.WriteLine("* Evader =>");
            Debugger.WriteLine("* Total abilities countered: " + enemyAbilitiesData.EvadableAbilities.Count);
            Debugger.WriteLine("* Total usable blink abilities: " + allyAbilitiesData.BlinkAbilities.Count);
            Debugger.WriteLine("* Total usable counter abilities: " + allyAbilitiesData.CounterAbilities.Count);
            Debugger.WriteLine("* Total usable disable abilities: " + allyAbilitiesData.DisableAbilities.Count);

            UpdateManager.BeginInvoke(OnUpdate);
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Entity.OnInt64PropertyChange += EntityOnOnInt64PropertyChange;
            Entity.OnInt32PropertyChange += EntityOnOnInt32PropertyChange;
        }

        public List<EvadableAbility> EvadableAbilities { get; } = new List<EvadableAbility>();

        public GoldSpender GoldSpender { get; } = new GoldSpender();

        public List<UsableAbility> UsableAbilities { get; } = new List<UsableAbility>();

        private static Hero Hero => Variables.Hero;

        private static Team HeroTeam => Variables.HeroTeam;

        private static MenuManager Menu => Variables.Menu;

        public void Dispose()
        {
            processing = false;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Entity.OnInt64PropertyChange -= EntityOnOnInt64PropertyChange;
            Entity.OnInt32PropertyChange -= EntityOnOnInt32PropertyChange;

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

        public async void OnUpdate()
        {
            while (processing)
            {
                await Task.Delay(2000);

                foreach (var unit in ObjectManager.GetEntities<Unit>()
                    .Where(
                        x => !(x is Building) && x.IsValid && x.IsAlive && x.IsSpawned
                             && (!x.IsIllusion || x.HasModifiers(
                                     new[]
                                     {
                                         "modifier_arc_warden_tempest_double",
                                         "modifier_vengefulspirit_hybrid_special",
                                         "modifier_morph_hybrid_special"
                                     },
                                     false))))
                {
                    var abilities = new List<Ability>();

                    try
                    {
                        abilities.AddRange(unit.Spellbook.Spells.ToList());

                        if (unit.HasInventory)
                        {
                            abilities.AddRange(
                                unit.Inventory.Items.Where(x => x.Purchaser.Hero?.Handle == unit.Handle));
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (var ability in abilities.Where(
                        x => x.IsValid && !addedAbilities.Contains(x.Handle) && x.Level > 0))
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
                        else if (unit.Team != HeroTeam
                                 || ability.Id == AbilityId.faceless_void_chronosphere)
                        {
                            Func<Ability, EvadableAbility> func;
                            if (enemyAbilitiesData.EvadableAbilities.TryGetValue(ability.Name, out func))
                            {
                                var evadableAbility = func.Invoke(ability);

                                if (Menu.Debug.FastAbilityAdd)
                                {
                                    Menu.EnemiesSettings.AddAbility(evadableAbility);
                                }
                                else
                                {
                                    await Menu.EnemiesSettings.AddAbility(evadableAbility);
                                }

                                EvadableAbilities.Add(evadableAbility);
                            }
                        }

                        addedAbilities.Add(ability.Handle);
                    }
                }
            }
        }

        private void EntityOnOnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue || args.NewValue > 0 || args.PropertyName != "m_iHealth")
            {
                return;
            }

            var creep = sender as Creep;
            if (creep == null || !creep.IsValid)
            {
                return;
            }

            EvadableAbilities.RemoveAll(x => x.OwnerHandle == creep.Handle);
        }

        private async void EntityOnOnInt64PropertyChange(Entity sender, Int64PropertyChangeEventArgs args)
        {
            if (args.OldValue != 0 || args.NewValue != 1 || args.PropertyName != "m_iIsControllableByPlayer64")
            {
                return;
            }

            await Task.Delay(100);

            if (sender.Team == HeroTeam && sender is Creep)
            {
                EvadableAbilities.RemoveAll(x => x.OwnerHandle == sender.Handle);
            }
        }
    }
}