namespace InformationPinger
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using Modules;

    internal class InformationPinger
    {
        #region Fields

        private readonly ChatWheel chatWheel = new ChatWheel();

        private readonly List<HeroPinger> heroesPinger = new List<HeroPinger>();

        private readonly MenuManager menuManager = new MenuManager();

        private readonly RoshanPinger roshanPinger = new RoshanPinger();

        private readonly RunePinger runePinger = new RunePinger();

        private CourierPinger courierPinger;

        private Team enemyTeam;

        private Hero hero;

        private bool loadedAfterGameStart;

        private WardPinger wardPinger;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            heroesPinger.Clear();
            loadedAfterGameStart = false;
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            enemyTeam = hero.GetEnemyTeam();
            Variables.Sleeper = new MultiSleeper();
            var heroTeam = hero.Team;
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

            Variables.Sleeper.Sleep(500, this);

            if (Game.IsPaused || !menuManager.Enabled)
            {
                return;
            }

            if (!Variables.Sleeper.Sleeping(heroesPinger))
            {
                foreach (var enemy in
                    Heroes.GetByTeam(enemyTeam)
                        .Where(
                            x => !x.IsIllusion && x.IsVisible && !heroesPinger.Select(z => z.Handle).Contains(x.Handle))
                    )
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

            if (menuManager.AbilityPingEnabled)
            {
                var doublePing = menuManager.DoubleAbilityPingEnabled;
                if (heroesPinger.Any(x => x.ShouldPing && x.AbilityPinger(doublePing)))
                {
                    return;
                }
            }

            if (menuManager.ItemPingEnabled)
            {
                var doublePing = menuManager.DoubleItemPingEnabled;
                var wards = menuManager.ItemWardsEnabled;
                if (heroesPinger.Any(x => x.ShouldPing && x.ItemPinger(wards, doublePing)))
                {
                    return;
                }
            }

            if (roshanPinger.RoshanKilled && menuManager.RoshanKillTimeEnabled)
            {
                chatWheel.Say(ChatWheel.Phrase.Roshan, true);
                roshanPinger.RoshanKilled = false;
                return;
            }

            if (menuManager.RuneReminderEnabled
                && (menuManager.RuneAutoDisableTime == 0 || Game.GameTime / 60 <= menuManager.RuneAutoDisableTime)
                && runePinger.TimeToSpawn(menuManager.RuneReminderTime))
            {
                chatWheel.Say(ChatWheel.Phrase.CheckRunes, true);
                return;
            }

            if (wardPinger.ShouldRemind(menuManager.WardsDelay) && menuManager.WardsReminderEnabled)
            {
                chatWheel.Say(ChatWheel.Phrase.NeedWards);
                return;
            }

            if (courierPinger.ShouldRemind(menuManager.CourierUpgradeDelay) && menuManager.CourierUpgradeReminder)
            {
                chatWheel.Say(ChatWheel.Phrase.UpgradeCourier);
                return;
            }
        }

        #endregion
    }
}