namespace AdvancedRangeDisplay
{
    using System;

    using Ensage;

    internal class AbilityArgs : EventArgs
    {
        #region Public Properties

        public int? Blue { get; set; } = null;

        public bool Enabled { get; set; }

        public int? Green { get; set; } = null;

        public Hero Hero { get; set; }

        public string Name { get; set; }

        public int? Red { get; set; } = null;

        public bool Redraw { get; set; }

        #endregion
    }
}