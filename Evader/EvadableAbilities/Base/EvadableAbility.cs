namespace Evader.EvadableAbilities.Base
{
    using System.Collections.Generic;
    using System.Linq;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

    using Utils;

    internal abstract class EvadableAbility
    {
        #region Constructors and Destructors

        protected EvadableAbility(Ability ability)
        {
            Owner = (Unit)ability.Owner;
            OwnerHandle = Owner.Handle;
            Handle = ability.Handle;
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            OwnerClassID = Owner.ClassID;
            IsDisable = ability.IsDisable() || ability.IsSilence();
            PiercesMagicImmunity = ability.PiercesMagicImmunity();
            if (IsDisable)
            {
                DisableAbilities.AddRange(Abilities.DisableAbilityNames);
                BlinkAbilities.AddRange(Abilities.BlinkAbilityNames);
            }
            Debugger.WriteLine("///////// " + GetType().Name + " (" + Name + ")");
            Debugger.WriteLine("// Cast point: " + CastPoint);
            Debugger.WriteLine("// Owner: " + Owner.Name);
            Debugger.WriteLine("// Is disable: " + IsDisable);
            Debugger.WriteLine("// Pierces Magic Immunity: " + PiercesMagicImmunity);
        }

        #endregion

        #region Public Properties

        public List<string> BlinkAbilities { get; } = new List<string>();

        public List<string> CounterAbilities { get; } = new List<string>();

        public List<string> DisableAbilities { get; } = new List<string>();

        public bool Enabled { get; set; }

        public float EndCast { get; protected set; }

        public uint Handle { get; }

        public bool IgnorePathfinder { get; protected set; }

        public bool IsDisable { get; }

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public List<string> ModifierEnemyCounter { get; } = new List<string>();

        public string ModifierName { get; protected set; }

        public List<string> ModifierSelfCounter { get; } = new List<string>();

        public string Name { get; }

        public uint? Obstacle { get; set; }

        public bool ObstacleStays { get; protected set; }

        public Unit Owner { get; }

        public ClassID OwnerClassID { get; }

        public uint OwnerHandle { get; }

        public bool PiercesMagicImmunity { get; protected set; }

        public PriorityChanger PriorityChanger { get; set; }

        private float startCast;

        public float StartCast
        {
            get
            {
                return startCast;
            }
            protected set
            {
                startCast = value - 0.01f;
            }
        }

        public bool UseCustomPriority { get; set; }

        #endregion

        #region Properties

        protected Ability Ability { get; }

        protected float CastPoint { get; set; }

        protected Hero Hero => Variables.Hero;

        protected ParticleEffect Particle { get; set; }

        protected Pathfinder Pathfinder => Variables.Pathfinder;

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeStopped()
        {
            return StartCast + CastPoint > Game.RawGameTime;
        }

        public abstract void Check();

        public abstract void Draw();

        public virtual void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            Particle?.Dispose();
            Particle = null;
            Pathfinder.RemoveObstacle(Obstacle.Value);
            Obstacle = null;
            EndCast = 0;
            StartCast = 0;
        }

        public IEnumerable<Priority> GetPriority()
        {
            //todo optimize

            var priority = new List<Priority>();
            foreach (var item in
                PriorityChanger.Dictionary.Select(x => x.Key)
                    .Where(x => PriorityChanger.AbilityToggler.IsEnabled(x))
                    .Reverse())
            {
                switch (item)
                {
                    case "item_sheepstick":
                        priority.Add(Priority.Disable);
                        break;
                    case "item_cyclone":
                        priority.Add(Priority.Counter);
                        break;
                    case "item_blink":
                        priority.Add(Priority.Blink);
                        break;
                    case "centaur_stampede":
                        priority.Add(Priority.Walk);
                        break;
                }
            }

            return priority;
        }

        public virtual float GetRemainingDisableTime()
        {
            return StartCast + CastPoint - Game.RawGameTime - 0.05f;
        }

        public virtual float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        public float GetSleepTime()
        {
            return (EndCast - Game.RawGameTime) * 1000;
        }

        public virtual bool IgnoreRemainingTime(float remainingTime = 0)
        {
            return false;
        }

        public virtual bool IsStopped()
        {
            if (!IsInPhase && CanBeStopped())
            {
                End();
                return true;
            }

            return false;
        }

        public virtual float ObstacleRemainingTime()
        {
            return EndCast - Game.RawGameTime;
        }

        #endregion
    }
}