namespace InformationPinger
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Modules;

    internal class InformationPinger
    {
        #region Fields

        private readonly ChatWheel chatWheel = new ChatWheel();

        private readonly List<HeroPinger> heroesPinger = new List<HeroPinger>();

        private readonly MenuManager menu = new MenuManager();

        private readonly RoshanPinger roshanPinger = new RoshanPinger();

        private readonly RunePinger runePinger = new RunePinger();

        private CourierPinger courierPinger;

        private bool loadedAfterGameStart;

        private WardPinger wardPinger;

        #endregion

        #region Properties

        private static Team EnemyTeam => Variables.EnemyTeam;

        private static Hero Hero => Variables.Hero;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            heroesPinger.Clear();
            loadedAfterGameStart = false;
        }

        public void OnLoad()
        {
            Variables.Hero = ObjectManager.LocalHero;
            Variables.EnemyTeam = Hero.GetEnemyTeam();
            Variables.Sleeper = new MultiSleeper();
            var heroTeam = Hero.Team;
            wardPinger = new WardPinger(heroTeam);
            courierPinger = new CourierPinger(heroTeam);

            if (Game.GameTime > 0)
            {
                loadedAfterGameStart = true;
            }
        }

        public void OnUpdate()
        {
            if (Variables.Sleeper.Sleeping(this))
            {
                return;
            }

            Variables.Sleeper.Sleep(1000, this);

            if (Game.IsPaused || !menu.Enabled)
            {
                return;
            }

            if (!Variables.Sleeper.Sleeping(heroesPinger))
            {
                foreach (var enemy in
                    ObjectManager.GetEntitiesParallel<Hero>()
                        .Where(
                            x =>
                                x.IsValid && x.Team == EnemyTeam && !x.IsIllusion && x.IsVisible
                                && !heroesPinger.Exists(z => z.Handle == x.Handle)))
                {
                    var heroPinger = new HeroPinger(enemy);
                    if (loadedAfterGameStart)
                    {
                        heroPinger.IgnoreCurrentAbilities();
                    }
                    heroesPinger.Add(heroPinger);
                }

                Variables.Sleeper.Sleep(5000, heroesPinger);
            }

            if (Variables.Sleeper.Sleeping("CanPing"))
            {
                return;
            }

            if (menu.AbilityPingEnabled)
            {
                var doublePing = menu.DoubleAbilityPingEnabled;
                var abilityEnemyCheck = menu.AbilityEnemyCheckEnabled;
                var rubickDisable = menu.RubicksStolenDisable;
                var rubickUltimate = menu.RubicksStolenUltimate;
                if (
                    heroesPinger.Any(
                        x =>
                            x.ShouldPing
                            && x.AbilityPinger(doublePing, abilityEnemyCheck, rubickDisable, rubickUltimate)))
                {
                    return;
                }
            }

            if (menu.ItemPingEnabled)
            {
                var doublePing = menu.DoubleItemPingEnabled;
                var itemEnemyCheck = menu.ItemEnemyCheckEnabled;
                var cost = menu.ItemCostGoldThreshold;
                var forceItems = menu.ForcePingItems();
                var statusCheck = menu.ItemEnemyStatusEnabled;
                if (
                    heroesPinger.Any(
                        x =>
                            (!statusCheck || x.ShouldPing)
                            && x.ItemPinger(doublePing, itemEnemyCheck, cost, forceItems)))
                {
                    return;
                }
            }

            var bottleRunes = menu.BottleRunes();
            if (bottleRunes.Any())
            {
                var doublePing = menu.DoubleItemPingEnabled;
                var itemEnemyCheck = menu.ItemEnemyCheckEnabled;
                if (heroesPinger.Any(x => x.ShouldPing && x.BottledRunePinger(doublePing, itemEnemyCheck, bottleRunes)))
                {
                    return;
                }
            }

            if (roshanPinger.RoshanKilled && menu.RoshanKillTimeEnabled)
            {
                chatWheel.Say(ChatWheel.Phrase.Roshan, true);
                roshanPinger.RoshanKilled = false;
                return;
            }

            if (menu.RuneReminderEnabled
                && (menu.RuneAutoDisableTime == 0 || Game.GameTime / 60 <= menu.RuneAutoDisableTime)
                && runePinger.TimeToSpawn(menu.RuneReminderTime))
            {
                chatWheel.Say(ChatWheel.Phrase.CheckRunes, true);
                return;
            }

            if (wardPinger.ShouldRemind(menu.WardsDelay) && menu.WardsReminderEnabled)
            {
                chatWheel.Say(ChatWheel.Phrase.NeedWards);
                return;
            }

            if (courierPinger.ShouldRemind(menu.CourierUpgradeDelay) && menu.CourierUpgradeReminder)
            {
                chatWheel.Say(ChatWheel.Phrase.UpgradeCourier);
                return;
            }
        }

        #endregion
    }
}