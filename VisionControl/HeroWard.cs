using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;

namespace VisionControl {
    internal class HeroWard {
        private static readonly Dictionary<Hero, HeroWard> EnemyWards = new Dictionary<Hero, HeroWard>();

        private HeroWard(Unit hero) {
            Observers = ObserversCount(hero);
            Sentries = SentriesCount(hero);
        }

        private uint Observers { get; set; }

        private uint Sentries { get; set; }

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
                    enemyWards.Update(enemy);
            }

            foreach (var heroWard in EnemyWards.Where(x => !x.Key.IsVisible).Select(x => x.Value))
                heroWard.Reset();
        }

        public void Update(Unit hero) {
            var obsCount = ObserversCount(hero);
            var sentCount = SentriesCount(hero);

            if (Observers > obsCount) {
                if (!WardGiven(hero))
                    MapWard.Add(new MapWard(ClassID.CDOTA_NPC_Observer_Ward, hero));
                Observers = obsCount;
            }
            else if (Observers < obsCount && !WardTaken(hero)) {
                Observers = obsCount;
            }

            if (Sentries > sentCount) {
                if (!WardGiven(hero))
                    MapWard.Add(new MapWard(ClassID.CDOTA_NPC_Observer_Ward_TrueSight, hero));
                Sentries = sentCount;
            }
            else if (Sentries < sentCount && !WardTaken(hero)) {
                Sentries = sentCount;
            }
        }

        private static bool WardTaken(Unit hero) {
            return
                EnemyWards.Any(
                    x =>
                        !x.Key.Equals(hero) && x.Key.IsAlive && x.Key.Distance2D(hero) <= 600 &&
                        x.Value.Observers + x.Value.Sentries > ObserversCount(x.Key) + SentriesCount(x.Key));
        }

        private static bool WardGiven(Unit hero) {
            return
                EnemyWards.Any(
                    x =>
                        !x.Key.Equals(hero) && x.Key.IsAlive && x.Key.Distance2D(hero) <= 600 &&
                        x.Value.Observers + x.Value.Sentries < ObserversCount(x.Key) + SentriesCount(x.Key));
        }

        private static uint ObserversCount(Unit hero) {
            var observer = hero.FindItem("item_ward_observer");

            if (observer != null)
                return observer.CurrentCharges;

            var dispenser = hero.FindItem("item_ward_dispenser");

            return dispenser != null ? dispenser.CurrentCharges : 0;
        }

        private static uint SentriesCount(Unit hero) {
            var sentries = hero.FindItem("item_ward_sentry");

            if (sentries != null)
                return sentries.CurrentCharges;

            var dispenser = hero.FindItem("item_ward_dispenser");

            return dispenser != null ? dispenser.SecondaryCharges : 0;
        }
    }
}