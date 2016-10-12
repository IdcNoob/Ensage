namespace Evader.EvadableAbilities.Base
{
    using System.Collections.Generic;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using Utils;

    internal abstract class EvadableAbility
    {
        #region Constructors and Destructors

        protected EvadableAbility(Ability ability)
        {
            AbilityOwner = (Unit)ability.Owner;
            OwnerHandle = AbilityOwner.Handle;
            Handle = ability.Handle;
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            OwnerClassID = AbilityOwner.ClassID;
            IsDisable = ability.IsDisable() || ability.IsSilence();
            PiercesMagicImmunity = ability.PiercesMagicImmunity();
            if (IsDisable)
            {
                DisableAbilities.AddRange(Abilities.DisableAbilityNames);
                BlinkAbilities.AddRange(Abilities.BlinkAbilityNames);
            }
            Debugger.WriteLine("///////// " + GetType().Name + " (" + Name + ")");
            Debugger.WriteLine("// Cast point: " + CastPoint);
            Debugger.WriteLine("// Owner: " + AbilityOwner.Name);
            Debugger.WriteLine("// Is disable: " + IsDisable);
            Debugger.WriteLine("// Pierces Magic Immunity: " + PiercesMagicImmunity);
        }

        #endregion

        #region Public Properties

        public Unit AbilityOwner { get; }

        public List<string> BlinkAbilities { get; } = new List<string>();

        public List<string> CounterAbilities { get; } = new List<string>();

        public List<string> DisableAbilities { get; } = new List<string>();

        public bool Enabled { get; set; }

        public float EndCast { get; protected set; }

        public uint Handle { get; }

        public bool IgnorePathfinder { get; protected set; }

        public bool IsDisable { get; }

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public List<string> ModifierAllyCounter { get; } = new List<string>();

        public bool ModifierCounterEnabled { get; set; }

        public List<string> ModifierEnemyCounter { get; } = new List<string>();

        public string ModifierName { get; protected set; }

        public string Name { get; }

        public uint? Obstacle { get; set; }

        public bool ObstacleStays { get; protected set; }

        public ClassID OwnerClassID { get; }

        public uint OwnerHandle { get; }

        public bool PiercesMagicImmunity { get; protected set; }

        public List<Priority> Priority { get; } = new List<Priority>();

        public float StartCast { get; protected set; }

        public bool UseCustomPriority { get; set; }

        #endregion

        #region Properties

        protected Ability Ability { get; }

        protected AbilityDrawer AbilityDrawer { get; set; } = new AbilityDrawer();

        protected float AdditionalDelay { get; set; }

        protected float CastPoint { get; set; }

        protected Hero Hero => Variables.Hero;

        protected Pathfinder Pathfinder => Variables.Pathfinder;

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeStopped()
        {
            return Ability.AbilityState != AbilityState.OnCooldown;
        }

        public abstract void Check();

        public abstract void Draw();

        public virtual void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.Dispose();
            Pathfinder.RemoveObstacle(Obstacle.Value);
            Obstacle = null;
            StartCast = 0;
            EndCast = 0;
        }

        public virtual float GetRemainingDisableTime()
        {
            return StartCast + CastPoint - Game.RawGameTime - 0.05f;
        }

        public abstract float GetRemainingTime(Hero hero = null);

        public float GetSleepTime()
        {
            return (EndCast - Game.RawGameTime) * 1000;
        }

        public virtual bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return false;
        }

        public virtual bool IsStopped()
        {
            return StartCast > 0 && !IsInPhase && CanBeStopped();
        }

        public virtual float ObstacleRemainingTime()
        {
            return EndCast - Game.RawGameTime;
        }

        #endregion
    }
}