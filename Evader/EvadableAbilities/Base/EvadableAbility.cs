namespace Evader.EvadableAbilities.Base
{
    using System;
    using System.Collections.Generic;

    using Common;

    using Core;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal abstract class EvadableAbility
    {
        protected EvadableAbility(Ability ability)
        {
            AbilityOwner = (Unit)ability.Owner;
            OwnerHandle = AbilityOwner.Handle;
            Handle = ability.Handle;
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            OwnerName = AbilityOwner.Name;
            IsDisable = ability.IsDisable() || ability.IsSilence();
            PiercesMagicImmunity = ability.PiercesMagicImmunity();
            CreateTime = Game.RawGameTime;

            if (IsDisable)
            {
                DisableAbilities.AddRange(DisableAbilityNames);
                BlinkAbilities.AddRange(BlinkAbilityNames);
            }
            Debugger.WriteLine("///////// Evadable ability // " + GetType().Name + " (" + Name + ")");
            Debugger.WriteLine("// Cast point: " + CastPoint);
            Debugger.WriteLine("// Owner: " + AbilityOwner.GetName());
            Debugger.WriteLine("// Is disable: " + IsDisable);
            Debugger.WriteLine("// Pierces Magic Immunity: " + PiercesMagicImmunity);
        }

        public Ability Ability { get; }

        public int AbilityLevelIgnore { get; set; }

        public Unit AbilityOwner { get; }

        public int AbilityTimeIgnore { get; set; }

        public float AdditionalDelay { get; protected set; }

        public int AllyHpIgnore { get; set; }

        public List<string> BlinkAbilities { get; } = new List<string>();

        public List<string> CounterAbilities { get; } = new List<string>();

        public float CreateTime { get; }

        public List<string> DisableAbilities { get; } = new List<string>();

        public bool DisablePathfinder { get; protected set; }

        public bool DisableTimeSinceCastCheck { get; protected set; }

        public bool Enabled { get; set; }

        public float EndCast { get; protected set; }

        public uint Handle { get; }

        public int HeroMpIgnore { get; set; }

        public bool IsDisable { get; protected set; }

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public uint Level => Ability.Level;

        public bool ModifierCounterEnabled { get; set; }

        public string Name { get; protected set; }

        public uint? Obstacle { get; set; }

        public bool ObstacleStays { get; protected set; }

        public string OwnerName { get; }

        public uint OwnerHandle { get; }

        public bool PiercesMagicImmunity { get; protected set; }

        public List<EvadePriority> Priority { get; } = new List<EvadePriority>();

        public float StartCast { get; protected set; }

        public bool UseCustomPriority { get; set; }

        protected AbilityDrawer AbilityDrawer { get; set; } = new AbilityDrawer();

        protected float CastPoint { get; set; }

        protected Team EnemyTeam => Variables.EnemyTeam;

        protected Hero Hero => Variables.Hero;

        protected Team HeroTeam => Variables.HeroTeam;

        protected Pathfinder Pathfinder => Variables.Pathfinder;

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
            return Math.Max((EndCast - Game.RawGameTime) * 1000, 300);
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

        public virtual float TimeSinceCast()
        {
            if (Ability.Level <= 0 || !AbilityOwner.IsVisible)
            {
                return float.MaxValue;
            }

            var cooldownLength = Ability.CooldownLength;
            return cooldownLength <= 0 ? float.MaxValue : cooldownLength - Ability.Cooldown;
        }
    }
}