using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;

namespace VisionControl {
    internal class HeroWard {
        private static readonly Dictionary<Hero, HeroWard> EnemyWards = new Dictionary<Hero, HeroWard>();

        private HeroWard(Unit hero) {
            Hero = hero;
            Observers = ObserversCount;
            Sentries = SentriesCount;
        }

        private uint Observers { get; set; }

        private uint Sentries { get; set; }

        private Unit Hero { get; set; }

        private bool WardGiven {
            get {
                return
                    EnemyWards.Any(
                        x =>
                            !x.Key.Equals(Hero) && x.Key.IsAlive && x.Key.Distance2D(Hero) <= 600 &&
                            x.Value.Observers + x.Value.Sentries < x.Value.ObserversCount + x.Value.SentriesCount);
            }
        }

        private bool WardTaken {
            get {
                return
                    EnemyWards.Any(
                        x =>
                            !x.Key.Equals(Hero) && x.Key.IsAlive && x.Key.Distance2D(Hero) <= 600 &&
                            x.Value.Observers + x.Value.Sentries > x.Value.ObserversCount + x.Value.SentriesCount);
            }
        }

        private uint ObserversCount {
            get {
                var observer = Hero.FindItem("item_ward_observer");

                if (observer != null)
                    return observer.CurrentCharges;

                var dispenser = Hero.FindItem("item_ward_dispenser");

                return dispenser != null ? dispenser.CurrentCharges : 0;
            }
        }

        private uint SentriesCount {
            get {
                var sentries = Hero.FindItem("item_ward_sentry");

                if (sentries != null)
                    return sentries.CurrentCharges;

                var dispenser = Hero.FindItem("item_ward_dispenser");

                return dispenser != null ? dispenser.SecondaryCharges : 0;
            }
        }

        public static void Clear() {
            EnemyWards.Clear();
        }

        private void Reset() {
            if (Observers != 0)
                Observers = 0;
            if (Sentries != 0)
                Sentries = 0;
        }

        public static void Update() {
            var enemies =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.Team == Program.Hero.GetEnemyTeam() && x.IsAlive && !x.IsIllusion);

            foreach (var enemy in enemies) {
                HeroWard enemyWards;

                if (!EnemyWards.TryGetValue(enemy, out enemyWards))
                    EnemyWards.Add(enemy, new HeroWard(enemy));
                else
                    enemyWards.UpdateHero();
            }

            foreach (var heroWard in EnemyWards.Where(x => !x.Key.IsVisible).Select(x => x.Value))
                heroWard.Reset();
        }

        private void UpdateHero() {
            var obsCount = ObserversCount;
            var sentCount = SentriesCount;

            if (Observers > obsCount) {
                if (!WardGiven && !WardDropped(ClassID.CDOTA_Item_ObserverWard))
                    MapWard.Add(Hero, ClassID.CDOTA_NPC_Observer_Ward);
                Observers = obsCount;
            }
            else if (Observers < obsCount && !WardTaken) {
                Observers = obsCount;
            }

            if (Sentries > sentCount) {
                if (!WardGiven && !WardDropped(ClassID.CDOTA_Item_SentryWard))
                    MapWard.Add(Hero, ClassID.CDOTA_NPC_Observer_Ward_TrueSight);
                Sentries = sentCount;
            }
            else if (Sentries < sentCount && !WardTaken) {
                Sentries = sentCount;
            }
        }

        private bool WardDropped(ClassID wardID) {
            return
                ObjectManager.GetEntities<PhysicalItem>()
                    .Any(x => x.IsVisible && x.Distance2D(Hero) <= 100 && x.Item.ClassID == wardID);
        }
    }
}