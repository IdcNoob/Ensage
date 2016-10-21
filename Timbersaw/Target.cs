namespace Timbersaw
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class Target
    {
        #region Fields

        private float lastVisible;

        #endregion

        #region Public Properties

        public ClassID ClassID { get; private set; }

        public uint Handle { get; private set; }

        public uint Health => Hero.Health;

        public Hero Hero { get; private set; }

        public bool IsVsisible => Hero.IsVisible;

        public bool Locked { get; set; }

        public float MovementSpeed { get; private set; }

        public Vector3 Position => Hero.NetworkPosition;

        public float RotationRad => Hero.RotationRad;

        #endregion

        #region Public Methods and Operators

        public double FindAngle(Vector3 position)
        {
            return Hero.FindRelativeAngle(position);
        }

        public Ability FindSpell(string name, bool cache)
        {
            return Hero.FindSpell(name, cache);
        }

        public float GetDistance(Vector3 position)
        {
            return IsVsisible ? position.Distance2D(Hero) : GetPosition().Distance2D(position);
        }

        public float GetInvisTime()
        {
            return Game.RawGameTime - lastVisible;
        }

        public Vector3 GetPosition(bool forDrawing = false)
        {
            if (Hero == null)
            {
                return new Vector3();
            }

            if (IsVsisible)
            {
                MovementSpeed = Hero.MovementSpeed;
                lastVisible = Game.RawGameTime;
                return forDrawing ? Hero.Position : Hero.NetworkPosition;
            }

            return TimberPrediction.IsIdle(Hero) ? Position : Prediction.InFront(Hero, MovementSpeed * GetInvisTime());
        }

        public bool HasModifier(string name)
        {
            return Hero.HasModifier(name);
        }

        public bool IsValid()
        {
            return Hero != null && Hero.IsValid && Hero.IsAlive && !Hero.IsMagicImmune() && !Hero.IsInvul();
        }

        public void NewTarget(Hero target)
        {
            Hero = target;

            if (target == null)
            {
                return;
            }

            Handle = target.Handle;
            ClassID = target.ClassID;
        }

        public Vector3 Vector3FromPolarAngle()
        {
            return Hero.Vector3FromPolarAngle();
        }

        #endregion
    }
}