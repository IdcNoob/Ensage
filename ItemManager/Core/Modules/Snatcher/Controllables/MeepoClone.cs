namespace ItemManager.Core.Modules.Snatcher.Controllables
{
    using Ensage;
    using Ensage.Common.Extensions;

    using Menus.Modules.Snatcher;

    internal class MeepoClone : Controllable
    {
        public MeepoClone(Unit unit)
            : base(unit)
        {
        }

        public override bool CanPick(PhysicalItem physicalItem, Manager manager, SnatcherMenu menu)
        {
            return false;
        }

        public override bool CanPick(Rune rune)
        {
            return Unit.Distance2D(rune) < 400;
        }
    }
}