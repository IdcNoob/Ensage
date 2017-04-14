namespace ItemManager.EventArgs
{
    using System;

    using Ensage;

    internal class UnitEventArgs : EventArgs
    {
        public UnitEventArgs(Unit unit)
        {
            Unit = unit;
        }

        public Unit Unit { get; }
    }
}