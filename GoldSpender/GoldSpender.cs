namespace GoldSpender
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using global::GoldSpender.Modules;

    internal class GoldSpender
    {
        #region Fields

        private readonly List<GoldManager> modules = new List<GoldManager>();

        private Sleeper sleeper;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            modules.Clear();
        }

        public void OnLoad()
        {
            Variables.Hero = ObjectManager.LocalHero;
            sleeper = new Sleeper();

            modules.Add(new NearDeath());
            modules.Add(new AutoPurchase());
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(100);

            if (Game.IsPaused || !Variables.Hero.IsAlive)
            {
                return;
            }

            foreach (var module in modules.Where(x => x.ShouldSpendGold()))
            {
                module.BuyItems();
            }
        }

        #endregion
    }
}