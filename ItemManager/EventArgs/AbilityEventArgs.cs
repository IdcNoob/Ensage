namespace ItemManager.EventArgs
{
    using System;

    using Ensage;

    internal class AbilityEventArgs : EventArgs
    {
        public AbilityEventArgs(Ability ability, bool isMine)
        {
            Ability = ability;
            IsMine = isMine;
        }

        public Ability Ability { get; }

        public bool IsMine { get; }
    }
}