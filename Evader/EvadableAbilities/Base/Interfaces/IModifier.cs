namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Modifiers;

    internal interface IModifier
    {
        #region Public Properties

        EvadableModifier Modifier { get; }

        #endregion
    }
}