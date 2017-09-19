namespace AdvancedRangeDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Ranges
    {
        public enum CustomRange
        {
            None,

            Experience,

            Attack,

            Aggro
        }

        private readonly List<HeroId> addedHeroes = new List<HeroId>();

        private readonly Dictionary<string, Item> addedItems = new Dictionary<string, Item>();

        private readonly Dictionary<Creep, ParticleEffect> creeps = new Dictionary<Creep, ParticleEffect>();

        private readonly Dictionary<Hero, List<AbilityDraw>> drawnAbilities = new Dictionary<Hero, List<AbilityDraw>>();

        private Team enemyTeam;

        private MenuManager menu;

        private Sleeper sleeper;

        public void OnClose()
        {
            menu.OnChange -= OnChange;
            menu.OnCreepColorChange -= OnCreepColorChange;
            menu.OnCreepChange -= OnCreepChange;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            ObjectManager.OnAddEntity -= OnAddEntity;

            menu.OnClose();
            drawnAbilities.SelectMany(x => x.Value).ForEach(x => x.ParticleEffect?.Dispose());
            addedHeroes.Clear();
            drawnAbilities.Clear();
            addedItems.Clear();
            creeps.ForEach(x => x.Value?.Dispose());
            creeps.Clear();
        }

        public void OnDraw()
        {
            foreach (var drawnAbilityPair in drawnAbilities)
            {
                foreach (var drawnAbility in drawnAbilityPair.Value.Where(x => x.RadiusOnly))
                {
                    var unit = drawnAbilityPair.Key;
                    drawnAbility.ParticleEffect?.SetControlPoint(
                        0,
                        unit.Position
                        + (Vector3)(VectorExtensions.FromPolarAngle(unit.RotationRad) * drawnAbility.Range));
                }
            }
        }

        public void OnLoad()
        {
            enemyTeam = ObjectManager.LocalHero.GetEnemyTeam();
            menu = new MenuManager(addedItems);
            sleeper = new Sleeper();

            menu.OnChange += OnChange;
            menu.OnCreepColorChange += OnCreepColorChange;
            menu.OnCreepChange += OnCreepChange;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            ObjectManager.OnAddEntity += OnAddEntity;

            OnCreepChange(
                null,
                new BoolEventArgs
                {
                    Enabled = menu.ShowCreepAggroRange
                });

            sleeper.Sleep(1000);
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var allHeroes = ObjectManager.GetEntities<Hero>().Where(x => x.IsValid && !x.IsIllusion);

            foreach (var hero in allHeroes)
            {
                if (!addedHeroes.Contains(hero.HeroId))
                {
                    addedHeroes.Add(hero.HeroId);
                    menu.AddHeroMenu(hero);
                }

                if (menu.IsItemsMenuEnabled(hero))
                {
                    foreach (var item in hero.Inventory.Items)
                    {
                        var itemName = item.GetDefaultName();

                        if (string.IsNullOrEmpty(itemName) || addedItems.ContainsKey(hero.StoredName() + itemName))
                        {
                            continue;
                        }

                        addedItems.Add(hero.StoredName() + itemName, item);

                        if (item.GetRealCastRange() > 500)
                        {
                            menu.AddMenuItem(hero, item);
                        }
                    }
                }

                foreach (var drawnAbility in drawnAbilities.Where(x => x.Key.Equals(hero))
                    .SelectMany(x => x.Value)
                    .Where(x => (x.ParticleEffect != null || !x.IsValid) && !x.Disabled && x.Level > 0))
                {
                    switch (drawnAbility.CustomRange)
                    {
                        case CustomRange.Experience:
                        case CustomRange.Aggro:
                            continue;
                        case CustomRange.Attack:
                            if (Math.Abs(drawnAbility.Range - hero.GetAttackRange()) > 10)
                            {
                                Redraw(drawnAbility);
                            }
                            continue;
                    }

                    var newAbility = false;

                    if (!drawnAbility.IsValid)
                    {
                        drawnAbility.FindAbility();

                        if (drawnAbility.IsValid)
                        {
                            newAbility = true;
                        }
                    }

                    if (drawnAbility.Ability == null && drawnAbility.ParticleEffect != null)
                    {
                        drawnAbility.ParticleEffect.Dispose();
                        drawnAbility.ParticleEffect = null;
                    }
                    else if ((newAbility || drawnAbility.ParticleEffect != null
                              && (Math.Abs(drawnAbility.RealCastRange - drawnAbility.Ability.GetRealCastRange()) > 5
                                  || drawnAbility.RadiusOnly && Math.Abs(
                                      drawnAbility.Radius - drawnAbility.Ability.GetRadius() - 35) > 5))
                             && !drawnAbility.Disabled)
                    {
                        Redraw(drawnAbility);
                    }
                }
            }

            sleeper.Sleep(3000);
        }

        private static void Redraw(AbilityDraw drawnAbility)
        {
            if (drawnAbility.ParticleEffect == null)
            {
                drawnAbility.ParticleEffect = drawnAbility.RadiusOnly
                                                  ? new ParticleEffect(
                                                      @"particles\ui_mouseactions\drag_selected_ring.vpcf",
                                                      drawnAbility.Hero.Position)
                                                  : drawnAbility.Hero.AddParticleEffect(
                                                      @"particles\ui_mouseactions\drag_selected_ring.vpcf");
                if (drawnAbility.Level < 1)
                {
                    return;
                }
            }

            drawnAbility.UpdateCastRange();
            drawnAbility.SaveSettings();

            drawnAbility.ParticleEffect.SetControlPoint(
                2,
                new Vector3((drawnAbility.RadiusOnly ? drawnAbility.Radius : drawnAbility.RealCastRange) * -1, 255, 0));
            drawnAbility.ParticleEffect.FullRestart();
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            if (!menu.ShowCreepAggroRange)
            {
                return;
            }

            var creep = args.Entity as Creep;
            if (creep == null || creep.Team != enemyTeam)
            {
                return;
            }

            var effect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", creep);
            effect.SetControlPoint(1, new Vector3(menu.CreepRedColor, menu.CreepGreenColor, menu.CreepBlueColor));
            effect.SetControlPoint(2, new Vector3(525, 255, 0));

            creeps.Add(creep, effect);
        }

        private void OnChange(object sender, AbilityEventArgs args)
        {
            List<AbilityDraw> abilities;

            if (!drawnAbilities.TryGetValue(args.Hero, out abilities))
            {
                drawnAbilities.Add(args.Hero, new List<AbilityDraw>());
            }

            var drawnAbility = abilities?.FirstOrDefault(x => x.Name == args.Name);

            if (drawnAbility == null)
            {
                drawnAbility = new AbilityDraw(args.Hero, args.Name);
                drawnAbilities[args.Hero].Add(drawnAbility);
            }

            drawnAbility.SaveSettings(args.Red, args.Green, args.Blue, args.RadiusOnly);

            if (!args.Redraw)
            {
                return;
            }

            if (args.Enabled)
            {
                drawnAbility.Disabled = false;

                if (!drawnAbility.IsValid)
                {
                    drawnAbility.FindAbility();

                    if (!drawnAbility.IsValid && drawnAbility.CustomRange == CustomRange.None)
                    {
                        return;
                    }
                }

                Redraw(drawnAbility);
            }
            else
            {
                if (drawnAbility.ParticleEffect != null)
                {
                    drawnAbility.ParticleEffect.Dispose();
                    drawnAbility.ParticleEffect = null;
                }

                drawnAbility.Disabled = true;
            }
        }

        private void OnCreepChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                foreach (var creep in ObjectManager.GetEntities<Creep>().Where(x => x.IsAlive && x.Team == enemyTeam))
                {
                    var effect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", creep);
                    effect.SetControlPoint(
                        1,
                        new Vector3(menu.CreepRedColor, menu.CreepGreenColor, menu.CreepBlueColor));
                    effect.SetControlPoint(2, new Vector3(525, 255, 0));

                    creeps.Add(creep, effect);
                }
            }
            else
            {
                creeps.ForEach(x => x.Value?.Dispose());
                creeps.Clear();
            }
        }

        private void OnCreepColorChange(object sender, EventArgs eventArgs)
        {
            foreach (var effect in creeps.Values)
            {
                effect?.SetControlPoint(1, new Vector3(menu.CreepRedColor, menu.CreepGreenColor, menu.CreepBlueColor));
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (!menu.ShowCreepAggroRange || args.NewValue == args.OldValue || args.NewValue > 0
                || args.PropertyName != "m_iHealth")
            {
                return;
            }

            var creep = sender as Creep;
            if (creep != null && creep.Team == enemyTeam)
            {
                ParticleEffect effect;
                creeps.TryGetValue(creep, out effect);
                effect?.Dispose();
                creeps.Remove(creep);
            }
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            if (!menu.ShowCreepAggroRange)
            {
                return;
            }

            var creep = args.Entity as Creep;
            if (creep != null && creep.Team != enemyTeam)
            {
                ParticleEffect effect;
                creeps.TryGetValue(creep, out effect);
                effect?.Dispose();
                creeps.Remove(creep);
            }
        }
    }
}