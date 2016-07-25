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

        private MenuManager menu;

        private Sleeper sleeper;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.OnChange -= OnChange;
            menu.OnClose();
            drawedAbilities.SelectMany(x => x.Value).ForEach(x => x.ParticleEffect?.Dispose());
            addedHeroes.Clear();
            drawedAbilities.Clear();
        }

        public void OnLoad()
        {
            menu = new MenuManager();
            sleeper = new Sleeper();

            menu.OnChange += OnChange;
            sleeper.Sleep(2000);
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(3000);

            var allHeroes = Heroes.All.Where(x => x.IsValid && !x.IsIllusion);

            foreach (var hero in allHeroes)
            {
                if (!addedHeroes.Contains(hero.Handle))
                {
                    menu.AddHeroMenu(hero);
                    addedHeroes.Add(hero.Handle);
                    return;
                }

                foreach (var item in hero.Inventory.Items)
                {
                    var itemName = item.GetDefaultName();

                    if (string.IsNullOrEmpty(itemName) || addedItems.ContainsKey(hero.StoredName() + itemName))
                    {
                        continue;
                    }

                    if (item.GetRealCastRange() > 200)
                    {
                        menu.AddMenuItem(hero, item);
                    }

                    addedItems.Add(hero.StoredName() + itemName, item);
                }

                foreach (var drawedAbility in
                    drawedAbilities.Where(x => x.Key.Equals(hero))
                        .SelectMany(x => x.Value)
                        .Where(x => (x.ParticleEffect != null || !x.IsValid) && !x.Disabled))
                {
                    if (drawedAbility.IsValid && drawedAbility.Ability.ClassID == ClassID.CDOTA_Ability_AttributeBonus)
                    {
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
                                  && Math.Abs(drawedAbility.CastRange - drawedAbility.Ability.GetRealCastRange()) > 5))
                             && !drawedAbility.Disabled)
                    {
                        Redraw(drawedAbility);
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void OnChange(object sender, AbilityArgs args)
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

            drawedAbility.SetColor(args.Red, args.Green, args.Blue);

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

                    if (!drawedAbility.IsValid)
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

        private void Redraw(AbilityDraw drawedAbility)
        {
            if (drawedAbility.ParticleEffect == null)
            {
                drawedAbility.ParticleEffect =
                    drawedAbility.Hero.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
            }

            drawedAbility.UpdateCastRange();
            drawedAbility.SetColor();
            if (drawedAbility.CastRange > 200)
            {
                drawedAbility.ParticleEffect.SetControlPoint(2, new Vector3(drawedAbility.CastRange, 255, 0));
                drawedAbility.ParticleEffect.Restart();
            }
        }

        #endregion
    }
}