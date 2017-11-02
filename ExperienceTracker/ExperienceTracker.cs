namespace ExperienceTracker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using SharpDX;

    [ExportPlugin("Experience Tracker", StartupMode.Auto, "IdcNoob")]
    internal class ExperienceTracker : Plugin
    {
        private readonly Dictionary<Creep, int> deadCreeps = new Dictionary<Creep, int>();

        private readonly List<TrackedHero> heroes = new List<TrackedHero>();

        private MenuManager menu;

        protected override void OnActivate()
        {
            menu = new MenuManager();
            menu.Enabled.Item.ValueChanged += EnabledOnValueChanged;

            if (!menu.Enabled)
            {
                return;
            }

            Entity.OnParticleEffectAdded += OnParticleEffectAdded;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnDeactivate()
        {
            menu.Enabled.Item.ValueChanged -= EnabledOnValueChanged;
            Entity.OnParticleEffectAdded -= OnParticleEffectAdded;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            Drawing.OnDraw -= OnDraw;

            menu.Dispose();
            heroes.Clear();
            deadCreeps.Clear();
        }

        private void CheckExperience()
        {
            foreach (var pair in deadCreeps)
            {
                var heroesInExpRange = heroes.Where(x => x.IsInExperienceRange(pair.Key)).ToList();

                foreach (var hero in heroesInExpRange)
                {
                    var totalExp = hero.CalculateGainedExperience(deadCreeps.Where(x => hero.IsInExperienceRange(x.Key)).Sum(x => x.Value));

                    if (hero.OldExperience + (totalExp / heroesInExpRange.Count) != hero.NewExperience)
                    {
                        hero.SetWarning(totalExp, heroesInExpRange.Count, menu.WarningTime);
                    }
                }
            }

            deadCreeps.Clear();
        }

        private void EnabledOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                Entity.OnParticleEffectAdded += OnParticleEffectAdded;
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
                Drawing.OnDraw += OnDraw;
            }
            else
            {
                Entity.OnParticleEffectAdded -= OnParticleEffectAdded;
                Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void OnDraw(EventArgs args)
        {
            foreach (var enemy in heroes.Where(x => x.IsValid && x.WarningIsActive))
            {
                var warningText = enemy.EnemiesWarning.ToString();

                if (!menu.SimplifiedWarning)
                {
                    warningText += " enem" + (enemy.EnemiesWarning > 1 ? "ies" : "y") + " near";
                }

                Drawing.DrawText(
                    warningText,
                    "Arial",
                    HUDInfo.GetHPbarPosition(enemy.Hero) + new Vector2(menu.WarningX, menu.WarningY),
                    new Vector2(menu.WarningSize),
                    new Color(menu.WarningRedColor, menu.WarningGreenColor, menu.WarningBlueColor),
                    FontFlags.None);
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            if (args.PropertyName == "m_iCurrentXP")
            {
                var hero = heroes.FirstOrDefault(x => x.Handle == sender.Handle);
                if (hero == null)
                {
                    var newHero = sender as Hero;
                    if (newHero == null || !newHero.IsValid || newHero.IsIllusion)
                    {
                        return;
                    }

                    heroes.Add(hero = new TrackedHero(newHero));
                }

                hero.SetExperience(args.OldValue, args.NewValue);
                return;
            }

            if (args.NewValue > 0 || args.PropertyName != "m_iHealth")
            {
                return;
            }

            var creep = sender as Creep;
            if (creep == null || !creep.IsValid || !creep.IsVisible || deadCreeps.ContainsKey(creep))
            {
                return;
            }

            deadCreeps.Add(creep, creep.GetGrantedExperience());
            UpdateManager.BeginInvoke(CheckExperience, 150);
        }

        private void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            if (args.Name != "particles/msg_fx/msg_deny.vpcf")
            {
                return;
            }

            UpdateManager.BeginInvoke(
                () =>
                    {
                        var creep = deadCreeps.FirstOrDefault(
                            x => x.Key.IsValid && x.Key.Position.Distance2D(args.ParticleEffect.GetControlPoint(0)) < 20);

                        if (creep.Key != null)
                        {
                            deadCreeps[creep.Key] = (int)(creep.Value * 0.25);
                        }
                    },
                10);
        }
    }
}