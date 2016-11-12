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
        #region Fields

        private readonly List<uint> addedHeroes = new List<uint>();

        private readonly Dictionary<string, Item> addedItems = new Dictionary<string, Item>();

        private readonly Dictionary<Hero, List<AbilityDraw>> drawedAbilities = new Dictionary<Hero, List<AbilityDraw>>();

        private bool delay;

        private MenuManager menu;

        private Sleeper sleeper;

        #endregion

        #region Enums

        public enum CustomRange
        {
            None,

            Expiriece,

            Attack
        }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.OnChange -= OnChange;
            menu.OnClose();
            drawedAbilities.SelectMany(x => x.Value).ForEach(x => x.ParticleEffect?.Dispose());
            addedHeroes.Clear();
            drawedAbilities.Clear();
            addedItems.Clear();
        }

        public void OnDraw()
        {
            foreach (var drawedAbilityPair in drawedAbilities)
            {
                foreach (var drawedAbility in drawedAbilityPair.Value.Where(x => x.RadiusOnly))
                {
                    var unit = drawedAbilityPair.Key;
                    drawedAbility.ParticleEffect?.SetControlPoint(
                        0,
                        unit.Position
                        + (Vector3)(VectorExtensions.FromPolarAngle(unit.RotationRad) * drawedAbility.Range));
                }
            }
        }

        public void OnLoad()
        {
            menu = new MenuManager();
            sleeper = new Sleeper();

            menu.OnChange += OnChange;
            sleeper.Sleep(1000);
        }

        public async void OnUpdate()
        {
            if (sleeper.Sleeping || delay)
            {
                return;
            }

            delay = true;

            try
            {
                var allHeroes = Heroes.All.Where(x => x.IsValid && !x.IsIllusion);

                foreach (var hero in allHeroes)
                {
                    if (!addedHeroes.Contains(hero.Handle))
                    {
                        await menu.AddHeroMenu(hero);
                        addedHeroes.Add(hero.Handle);
                    }

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
                            await menu.AddMenuItem(hero, item);
                        }
                    }

                    foreach (var drawedAbility in
                        drawedAbilities.Where(x => x.Key.Equals(hero))
                            .SelectMany(x => x.Value)
                            .Where(x => (x.ParticleEffect != null || !x.IsValid) && !x.Disabled && x.Level > 0))
                    {
                        switch (drawedAbility.CustomRange)
                        {
                            case CustomRange.Expiriece:
                                continue;
                            case CustomRange.Attack:
                                if (Math.Abs(drawedAbility.Range - hero.GetAttackRange()) > 10)
                                {
                                    Redraw(drawedAbility);
                                }
                                continue;
                        }

                        var newAbility = false;

                        if (!drawedAbility.IsValid)
                        {
                            drawedAbility.FindAbility();

                            if (drawedAbility.IsValid)
                            {
                                newAbility = true;
                            }
                        }

                        if (drawedAbility.Ability == null && drawedAbility.ParticleEffect != null)
                        {
                            drawedAbility.ParticleEffect.Dispose();
                            drawedAbility.ParticleEffect = null;
                        }
                        else if ((newAbility
                                  || (drawedAbility.ParticleEffect != null
                                      && (Math.Abs(drawedAbility.RealCastRange - drawedAbility.Ability.GetRealCastRange())
                                          > 5
                                          || drawedAbility.RadiusOnly
                                          && Math.Abs(drawedAbility.Radius - drawedAbility.Ability.GetRadius() - 35) > 5)))
                                 && !drawedAbility.Disabled)
                        {
                            Redraw(drawedAbility);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            delay = false;
            sleeper.Sleep(3000);
        }

        #endregion

        #region Methods

        private static void Redraw(AbilityDraw drawedAbility)
        {
            if (drawedAbility.ParticleEffect == null)
            {
                drawedAbility.ParticleEffect = drawedAbility.RadiusOnly
                                                   ? new ParticleEffect(
                                                       @"particles\ui_mouseactions\drag_selected_ring.vpcf",
                                                       drawedAbility.Hero.Position)
                                                   : drawedAbility.Hero.AddParticleEffect(
                                                       @"particles\ui_mouseactions\drag_selected_ring.vpcf");
                if (drawedAbility.Level < 1)
                {
                    return;
                }
            }

            drawedAbility.UpdateCastRange();
            drawedAbility.SaveSettings();

            drawedAbility.ParticleEffect.SetControlPoint(
                2,
                new Vector3(
                    (drawedAbility.RadiusOnly ? drawedAbility.Radius : drawedAbility.RealCastRange) * -1,
                    255,
                    0));
            drawedAbility.ParticleEffect.Restart();
        }

        private void OnChange(object sender, AbilityEventArgs args)
        {
            List<AbilityDraw> abilities;

            if (!drawedAbilities.TryGetValue(args.Hero, out abilities))
            {
                drawedAbilities.Add(args.Hero, new List<AbilityDraw>());
            }

            var drawedAbility = abilities?.FirstOrDefault(x => x.Name == args.Name);

            if (drawedAbility == null)
            {
                drawedAbility = new AbilityDraw(args.Hero, args.Name);
                drawedAbilities[args.Hero].Add(drawedAbility);
            }

            drawedAbility.SaveSettings(args.Red, args.Green, args.Blue, args.RadiusOnly);

            if (!args.Redraw)
            {
                return;
            }

            if (args.Enabled)
            {
                drawedAbility.Disabled = false;

                if (!drawedAbility.IsValid)
                {
                    drawedAbility.FindAbility();

                    if (!drawedAbility.IsValid && drawedAbility.CustomRange == CustomRange.None)
                    {
                        return;
                    }
                }

                Redraw(drawedAbility);
            }
            else
            {
                if (drawedAbility.ParticleEffect != null)
                {
                    drawedAbility.ParticleEffect.Dispose();
                    drawedAbility.ParticleEffect = null;
                }

                drawedAbility.Disabled = true;
            }
        }

        #endregion
    }
}