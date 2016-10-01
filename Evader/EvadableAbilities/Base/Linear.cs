namespace Evader.EvadableAbilities.Base
{
    using Ensage;

    using SharpDX;

    using Utils;

    internal abstract class Linear : EvadableAbility
    {
        #region Constructors and Destructors

        protected Linear(Ability ability)
            : base(ability)
        {
            Debugger.WriteLine("// Cast range: " + Ability.GetRealCastRange());
        }

        #endregion

        #region Properties

        protected Vector3 EndPosition { get; set; }

        protected Vector3 StartPosition { get; set; }

        #endregion

        #region Methods

        protected virtual float GetCastRange()
        {
            return Ability.GetRealCastRange();
        }

        #endregion
    }
}