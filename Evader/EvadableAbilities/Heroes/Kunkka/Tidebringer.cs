namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using Base;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using static Data.AbilityNames;

    internal class Tidebringer : AOE
    {
        private readonly Sleeper attackSleeper;

        private readonly Hero kunkka;

        public Tidebringer(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.Add(Eul);
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);

            kunkka = (Hero)AbilityOwner;
            attackSleeper = new Sleeper();
        }

        public override void Check()
        {
            if (StartCast <= 0 && !attackSleeper.Sleeping && kunkka.IsAttacking() && Ability.CanBeCasted()
                && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                StartPosition = AbilityOwner.InFront(GetRadius());
                EndCast = StartCast + (float)UnitDatabase.GetAttackPoint(kunkka);
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                attackSleeper.Sleep(kunkka.AttacksPerSecond * 1000);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        public override bool IsStopped()
        {
            var isStopped = StartCast > 0 && !kunkka.IsAttacking() && CanBeStopped();

            if (isStopped)
            {
                attackSleeper.Sleep(0);
            }

            return isStopped;
        }
    }
}