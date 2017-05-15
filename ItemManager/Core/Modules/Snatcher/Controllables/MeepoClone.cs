namespace ItemManager.Core.Modules.Snatcher.Controllables
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class MeepoClone : Controllable
    {
        public MeepoClone(Unit unit)
            : base(unit)
        {
        }

        public override bool CanPick(PhysicalItem physicalItem, int costThreshold)
        {
            return false;
        }

        public override bool CanPick(Rune rune)
        {
            return !Sleeper.Sleeping && Unit.Distance2D(rune) < 400;
        }
    }
}