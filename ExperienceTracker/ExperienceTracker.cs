namespace ExperienceTracker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using SharpDX;

    [ExportPlugin("Experience Tracker", StartupMode.Auto, "IdcNoob")]
    internal class ExperienceTracker : Plugin
    {
        private readonly Dictionary<Creep, int> deadCreeps = new Dictionary<Creep, int>();

        private readonly List<Enemy> enemies = new List<Enemy>();

        private readonly Team heroTeam;

        private MenuManager menu;

        [ImportingConstructor]
        public ExperienceTracker([Import] IServiceContext context)
        {
            heroTeam = context.Owner.Team;
        }

        protected override void OnActivate()
        {
            menu = new MenuManager();

            EntityManager<Hero>.EntityAdded += OnEntityAdded;
            Entity.OnInt32PropertyChange += EntityOnInt32PropertyChange;
            Drawing.OnDraw += OnDraw;

            foreach (var entity in ObjectManager.GetEntities<Hero>().Concat(ObjectManager.GetDormantEntities<Hero>()))
            {
                OnEntityAdded(null, entity);
            }
        }

        protected override void OnDeactivate()
        {
            Entity.OnInt32PropertyChange -= EntityOnInt32PropertyChange;
            Drawing.OnDraw -= OnDraw;
            EntityManager<Hero>.EntityAdded -= OnEntityAdded;

            menu.Dispose();
            enemies.Clear();
            deadCreeps.Clear();
        }

        private void CheckExperience()
        {
            foreach (var pair in deadCreeps)
            {
                var deadCreep = pair.Key;
                var enemiesInExpRange = enemies.Where(x => x.IsInExperienceRange(deadCreep)).ToList();

                foreach (var enemy in enemiesInExpRange)
                {
                    var totalExp = enemy.CalculateGainedExperience(
                        deadCreeps.Where(x => enemy.IsInExperienceRange(x.Key)).Sum(x => x.Value));
                    if (enemy.OldExperience + totalExp / enemiesInExpRange.Count != enemy.NewExperience)
                    {
                        enemy.SetWarning(totalExp, enemiesInExpRange.Count, menu.WarningTime);
                    }
                }
            }

            deadCreeps.Clear();
        }

        private void EntityOnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            if (args.PropertyName == "m_iCurrentXP")
            {
                enemies.FirstOrDefault(x => x.Handle == sender.Handle)?.SetExperience(args.OldValue, args.NewValue);
                return;
            }

            if (!menu.Enabled || args.NewValue > 0 || args.PropertyName != "m_iHealth")
            {
                return;
            }

            var creep = sender as Creep;
            if (creep == null || !creep.IsValid || creep.Team != heroTeam)
            {
                return;
            }

            var exp = creep.GetGrantedExperience();
            if (exp <= 0 || deadCreeps.ContainsKey(creep))
            {
                return;
            }

            deadCreeps.Add(creep, exp);
            UpdateManager.BeginInvoke(CheckExperience, 150);
        }

        private void OnDraw(EventArgs args)
        {
            foreach (var enemy in enemies.Where(x => x.WarningIsActive))
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

        private void OnEntityAdded(object sender, Hero enemy)
        {
            if (!enemy.IsValid || enemy.Team == heroTeam || enemy.IsIllusion
                || enemies.Any(x => x.Handle == enemy.Handle))
            {
                return;
            }

            enemies.Add(new Enemy(enemy));
        }
    }
}