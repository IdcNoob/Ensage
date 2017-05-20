namespace CreepsAggro
{
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Creeps Aggro", StartupMode.Auto, "IdcNoob")]
    internal class CreepsAggro : Plugin
    {
        private readonly Unit hero;

        private readonly Team heroTeam;

        private Config config;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public CreepsAggro([Import] IServiceContext context)
        {
            hero = context.Owner;
            heroTeam = hero.Team;
        }

        protected override void OnActivate()
        {
            config = new Config();
            config.Aggro.Item.ValueChanged += KeyPressed;
            config.UnAggro.Item.ValueChanged += KeyPressed;
            updateHandler = UpdateManager.Subscribe(OnUpdate, 300, false);
        }

        protected override void OnDeactivate()
        {
            config.Aggro.Item.ValueChanged -= KeyPressed;
            config.UnAggro.Item.ValueChanged -= KeyPressed;
            UpdateManager.Unsubscribe(OnUpdate);
            config.Dispose();
        }

        private void Attack(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            hero.Attack(unit);

            if (config.MoveToMousePosition)
            {
                hero.Move(Game.MousePosition);
            }
            else
            {
                hero.Stop();
            }
        }

        private void KeyPressed(object sender, OnValueChangeEventArgs onValueChangeEventArgs)
        {
            updateHandler.IsEnabled = onValueChangeEventArgs.GetNewValue<KeyBind>();
        }

        private void OnUpdate()
        {
            if (config.Aggro)
            {
                var enemy = EntityManager<Hero>.Entities
                    .Where(x => x.IsValid && x.IsAlive && !x.IsInvul() && x.Team != heroTeam)
                    .OrderBy(x => hero.FindRotationAngle(x.Position))
                    .FirstOrDefault();

                Attack(enemy);
            }
            else if (config.UnAggro)
            {
                var ally = EntityManager<Creep>.Entities
                    .Where(x => x.IsValid && x.IsAlive && x.IsSpawned && !x.IsInvul() && x.Team == heroTeam)
                    .OrderBy(x => hero.FindRotationAngle(x.Position))
                    .FirstOrDefault();

                Attack(ally);
            }
        }
    }
}