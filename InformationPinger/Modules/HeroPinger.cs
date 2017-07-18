namespace InformationPinger.Modules
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Service;

    using Interfaces;

    using PingTypes;

    [Export(typeof(IModule))]
    internal class HeroPinger : IModule
    {
        private readonly Team enemyTeam;

        private readonly IInformationPinger informationPinger;

        private readonly IMenuManager rootMenu;

        private MenuItem<Slider> autoDisableTime;

        private MenuItem<bool> doublePing;

        private MenuItem<bool> enabled;

        private Dictionary<Hero, float> heroes;

        private MenuItem<Slider> missingTime;

        [ImportingConstructor]
        public HeroPinger(
            [Import] IServiceContext context,
            [Import] IMenuManager menu,
            [Import] IInformationPinger pinger)
        {
            enemyTeam = context.Owner.GetEnemyTeam();
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            CreateMenu();

            heroes = new Dictionary<Hero, float>();

            if (enabled)
            {
                foreach (var hero in ObjectManager.GetEntities<Hero>().Concat(ObjectManager.GetDormantEntities<Hero>()))
                {
                    OnEntityAdded(null, hero);
                }

                EntityManager<Hero>.EntityAdded += OnEntityAdded;
                UpdateManager.Subscribe(OnUpdate, 1000);
            }
            enabled.Item.ValueChanged += ItemOnValueChanged;
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            EntityManager<Hero>.EntityAdded -= OnEntityAdded;
            enabled.Item.ValueChanged -= ItemOnValueChanged;
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Heroes");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Ping missing heroes");
            doublePing = menu.Item("Double ping", false);
            doublePing.Item.SetTooltip("Will ping hero 2 times");
            missingTime = menu.Item("Missing time (sec)", new Slider(15, 5, 60));
            missingTime.Item.SetTooltip("How long enemy should not be visible to ping");
            autoDisableTime = menu.Item("Auto disable (mins)", new Slider(10, 4, 30));
            autoDisableTime.Item.SetTooltip("Auto disable after X mins");
        }

        private void ItemOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                EntityManager<Hero>.EntityAdded += OnEntityAdded;
                UpdateManager.Subscribe(OnUpdate, 1000);
            }
            else
            {
                EntityManager<Hero>.EntityAdded -= OnEntityAdded;
                UpdateManager.Unsubscribe(OnUpdate);
            }
        }

        private void OnEntityAdded(object sender, Hero hero)
        {
            if (!hero.IsValid || hero.Team != enemyTeam || hero.IsIllusion || hero.HeroId == HeroId.npc_dota_hero_meepo)
            {
                // fuck meepo :kappa:
                return;
            }

            heroes[hero] = float.MaxValue;
        }

        private void OnUpdate()
        {
            var gameTime = Game.GameTime;
            if (gameTime < 60)
            {
                return;
            }

            if (gameTime > autoDisableTime * 60)
            {
                Dispose();
                return;
            }

            foreach (var pair in heroes.Where(x => x.Key.IsValid).ToList())
            {
                var hero = pair.Key;
                if (hero.IsVisible || !hero.IsAlive)
                {
                    heroes[hero] = gameTime;
                    continue;
                }

                var lastVisibleTime = pair.Value;
                if (gameTime - missingTime < lastVisibleTime)
                {
                    continue;
                }

                informationPinger.AddPing(new HeroChatWheelPing(ChatWheelMessage.MissingHero, hero.HeroId, doublePing));
                heroes[hero] = float.MaxValue;
            }
        }
    }
}