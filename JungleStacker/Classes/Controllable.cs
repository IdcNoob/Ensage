namespace JungleStacker.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.DrawObjects;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    using Utils;

    internal class Controllable
    {
        public enum Status
        {
            Idle,

            MovingToWaitPosition,

            WaitingStackTime,

            MovingToCampPosition,

            MovingToStackPosition,

            WaitingOnStackPosition,

            WaitingOnStackPositionToPreventBlock,

            WaitingOnStackPositionToDropAggro,

            TryingToCheckStacks,

            PreventCampBlock,

            DropAggro,

            Done
        }

        public readonly List<Camp> BlockedCamps = new List<Camp>();

        private readonly float attackAnimationPoint;

        private readonly float attackRange;

        private readonly float attackRate;

        private readonly bool isRanged;

        private readonly float projectileSpeed = float.MaxValue;

        private readonly Sleeper sleeper;

        private float attackTime;

        private bool campAvailable;

        private Vector2 campNameTextPosition;

        private float hPBarSizeX;

        private uint lastHealth;

        private float pause;

        private bool registered;

        private Vector3 targetPosition;

        public Controllable(Unit unit, bool isHero = false)
        {
            Handle = unit.Handle;
            Unit = unit;
            IsHero = isHero;
            attackRange = unit.GetAttackRange();
            sleeper = new Sleeper();

            attackAnimationPoint = Game.FindKeyValues(
                    unit.Name + "/AttackAnimationPoint",
                    unit is Hero ? KeyValueSource.Hero : KeyValueSource.Unit)
                .FloatValue;
            attackRate = Game.FindKeyValues(
                    unit.Name + "/AttackRate",
                    unit is Hero ? KeyValueSource.Hero : KeyValueSource.Unit)
                .FloatValue;
            if (attackRate <= 0)
            {
                attackRate = Game.FindKeyValues(
                        unit.Name + "/AttackRate",
                        unit is Hero ? KeyValueSource.Hero : KeyValueSource.Unit)
                    .IntValue;
            }

            if (Unit.AttackCapability == AttackCapability.Ranged)
            {
                isRanged = true;
                projectileSpeed = Game.FindKeyValues(
                        unit.Name + "/ProjectileSpeed",
                        unit is Hero ? KeyValueSource.Hero : KeyValueSource.Unit)
                    .IntValue;
            }
        }

        public event EventHandler OnCampChange;

        public static bool Debug { set; get; }

        public Camp CurrentCamp { get; private set; }

        public Status CurrentStatus { get; private set; }

        public bool EnableHeroStacking { get; set; }

        public uint Handle { get; }

        public bool IsHero { get; }

        public bool IsStacking { get; set; }

        public bool IsValid => Unit != null && Unit.IsValid && Unit.IsAlive;

        public float Pause
        {
            get
            {
                return pause;
            }
            set
            {
                pause = Game.GameTime + value;
            }
        }

        public Unit Unit { get; }

        private float GetHpBarSizeX
        {
            get
            {
                if (hPBarSizeX <= 0)
                {
                    hPBarSizeX = HUDInfo.GetHPBarSizeX();
                }
                return hPBarSizeX;
            }
        }

        private bool IsUnderCampNameText => Utils.IsUnderRectangle(
            Game.MouseScreenPosition,
            campNameTextPosition.X,
            campNameTextPosition.Y,
            MeasureCampNameTextSize.X,
            MeasureCampNameTextSize.Y);

        private Vector2 MeasureCampNameTextSize => Drawing.MeasureText(
            CurrentCamp.Name,
            "Arial",
            new Vector2(16),
            FontFlags.None);

        public void OnClose()
        {
            registered = false;
            IsStacking = false;
            Game.OnUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnWndProc -= Game_OnWndProc;
        }

        public void OnDisable()
        {
            IsStacking = false;
            CurrentStatus = Status.Idle;
        }

        public void Stack(Camp camp, int delay = 0)
        {
            CurrentCamp = camp;
            CurrentCamp.IsStacking = true;
            IsStacking = true;
            CurrentStatus = Status.Idle;
            campAvailable = false;
            Pause = delay;
            lastHealth = Unit.Health;

            if (!registered)
            {
                Game.OnUpdate += Game_OnUpdate;
                Drawing.OnDraw += Drawing_OnDraw;
                Game.OnWndProc += Game_OnWndProc;
                registered = true;
            }
        }

        private void ChangeCamp()
        {
            BlockedCamps.Add(CurrentCamp);
            IsStacking = false;
            CurrentCamp.IsStacking = false;

            var onCampChange = OnCampChange;
            onCampChange?.Invoke(this, EventArgs.Empty);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsStacking || !IsValid)
            {
                return;
            }

            campNameTextPosition = HUDInfo.GetHPbarPosition(Unit) + new Vector2(
                                       (GetHpBarSizeX - MeasureCampNameTextSize.X) / 2,
                                       IsHero ? 25 : 0);

            if (campNameTextPosition.IsZero)
            {
                return;
            }
            var text = CurrentCamp.Name;

            if (Debug)
            {
                text += " (" + CurrentStatus + ")";
            }

            var campName = new DrawText
            {
                Position = campNameTextPosition,
                Text = text,
                Color = Color.White,
                TextSize = new Vector2(16)
            };

            if (IsUnderCampNameText && !IsHero)
            {
                campName.Color = Color.Orange;
            }

            campName.Draw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(200);

            var gameTime = Game.GameTime;

            if (Game.IsPaused || Pause > gameTime || !EnableHeroStacking && IsHero || !IsStacking)
            {
                return;
            }

            if (!IsValid || CurrentCamp.CurrentStacksCount >= CurrentCamp.RequiredStacksCount && !IsHero)
            {
                if (campAvailable)
                {
                    return;
                }

                CurrentStatus = Status.Done;
                CurrentCamp.IsStacking = false;
                IsStacking = false;
                campAvailable = true;
                return;
            }

            switch (CurrentStatus)
            {
                case Status.Idle:
                    Unit.Move(CurrentCamp.WaitPosition);
                    CurrentStatus = Status.MovingToWaitPosition;
                    return;
                case Status.MovingToWaitPosition:
                    if (Unit.Distance2D(CurrentCamp.WaitPosition) > 50)
                    {
                        if (Unit.NetworkActivity == NetworkActivity.Idle)
                        {
                            Unit.Move(CurrentCamp.WaitPosition);
                        }
                        return;
                    }
                    CurrentStatus = Status.WaitingStackTime;
                    return;
                case Status.WaitingStackTime:
                    if (CurrentCamp.CurrentStacksCount <= 0)
                    {
                        CurrentStatus = Status.PreventCampBlock;
                        return;
                    }

                    if (Unit.NetworkActivity == NetworkActivity.Idle)
                    {
                        if (Unit.Health < lastHealth)
                        {
                            CurrentStatus = Status.DropAggro;
                            return;
                        }
                        if (Unit.Distance2D(CurrentCamp.WaitPosition) > 50)
                        {
                            CurrentStatus = Status.MovingToWaitPosition;
                            return;
                        }
                    }

                    lastHealth = Unit.Health;

                    var seconds = gameTime % 60;
                    if (seconds >= 57)
                    {
                        return;
                    }

                    var target = Creeps.All.OrderBy(x => x.Distance2D(Unit))
                        .FirstOrDefault(
                            x => x.Distance2D(Unit) <= 600 && x.IsSpawned && x.IsAlive && x.IsNeutral
                                 && !x.Equals(Unit));

                    var attackTarget = (isRanged || Game.IsNight) && target != null;
                    targetPosition = target?.Position.Extend(Unit.Position, 50) ?? CurrentCamp.CampPosition;

                    if (seconds
                        + (CurrentCamp.CurrentStacksCount >= CurrentCamp.StackCountTimeAdjustment
                               ? Math.Min(
                                   CurrentCamp.MaxTimeAdjustment,
                                   CurrentCamp.CurrentStacksCount - CurrentCamp.StackCountTimeAdjustment
                                   + CurrentCamp.TimeAdjustment)
                               : 0) + (attackTarget
                                           ? GetAttackPoint() + Unit.Distance2D(target) / projectileSpeed
                                             + Unit.GetTurnTime(target)
                                             + Math.Max(0, Unit.Distance2D(target) - attackRange) / Unit.MovementSpeed
                                           : Unit.Distance2D(targetPosition) / Unit.MovementSpeed)
                        + Unit.GetTurnTime(targetPosition) + Game.Ping / 1000 >= CurrentCamp.StackTime)
                    {
                        if (attackTarget)
                        {
                            Unit.Attack(target);
                        }
                        else
                        {
                            Unit.Move(targetPosition);
                        }

                        CurrentStatus = Status.MovingToCampPosition;
                    }
                    return;
                case Status.PreventCampBlock:
                    Unit.Move(CurrentCamp.StackPosition);
                    CurrentStatus = Status.WaitingOnStackPositionToPreventBlock;
                    return;
                case Status.DropAggro:
                    Unit.Move(CurrentCamp.StackPosition);
                    CurrentStatus = Status.WaitingOnStackPositionToDropAggro;
                    Pause = CurrentCamp.StackPosition.Distance2D(Unit) / Unit.MovementSpeed
                            + Math.Min(6, CurrentCamp.CurrentStacksCount + 2);
                    return;
                case Status.WaitingOnStackPositionToDropAggro:
                    if (CurrentCamp.StackPosition.Distance2D(Unit) < 50)
                    {
                        lastHealth = Unit.Health;
                        CurrentStatus = Status.Idle;
                    }
                    return;
                case Status.MovingToCampPosition:
                    if (Unit.Health < lastHealth)
                    {
                        Unit.Move(CurrentCamp.StackPosition);
                        CurrentStatus = Status.MovingToStackPosition;
                    }
                    else if (isRanged || Game.IsNight)
                    {
                        if (attackTime <= 0 && Unit.IsAttacking())
                        {
                            attackTime = gameTime + Game.Ping / 1000;
                        }
                        else if (attackTime > 0 && gameTime >= attackTime + GetAttackPoint())
                        {
                            attackTime = 0;
                            Unit.Move(CurrentCamp.StackPosition);
                            CurrentStatus = Status.MovingToStackPosition;
                        }
                    }
                    else if (Unit.Distance2D(targetPosition) < 50)
                    {
                        Unit.Move(CurrentCamp.StackPosition);
                        CurrentStatus = Status.MovingToStackPosition;
                    }
                    return;
                case Status.MovingToStackPosition:
                    if (CurrentCamp.StackPosition.Distance2D(Unit) < 50)
                    {
                        CurrentStatus = Status.WaitingOnStackPosition;
                        Pause = CurrentCamp.StackPosition.Distance2D(Unit) / Unit.MovementSpeed
                                + Math.Min(6, CurrentCamp.CurrentStacksCount + 2);
                    }
                    return;
                case Status.WaitingOnStackPositionToPreventBlock:
                    if (CurrentCamp.CurrentStacksCount >= 1)
                    {
                        CurrentStatus = Status.Idle;
                    }
                    return;
                case Status.WaitingOnStackPosition:
                    Unit.Move(CurrentCamp.WaitPosition);
                    CurrentCamp.CurrentStacksCount++;
                    CurrentStatus = Status.TryingToCheckStacks;
                    lastHealth = Unit.Health;
                    return;
                case Status.TryingToCheckStacks:
                    if (Unit.Distance2D(CurrentCamp.WaitPosition) < 50)
                    {
                        CurrentCamp.CurrentStacksCount = Creeps.All.Where(
                                x => x.Distance2D(CurrentCamp.CampPosition) < 800 && x.IsSpawned && x.IsNeutral
                                     && !x.Equals(Unit))
                            .ToList()
                            .CountStacks();

                        if (CurrentCamp.CurrentStacksCount >= CurrentCamp.RequiredStacksCount)
                        {
                            CurrentCamp.IsStacking = false;
                            IsStacking = false;
                            CurrentStatus = Status.Done;
                        }
                        else
                        {
                            CurrentStatus = Status.Idle;
                        }
                    }
                    return;
            }
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (!IsValid || !IsStacking)
            {
                return;
            }

            if (IsHero && args.Msg == (ulong)Utils.WindowsMessages.WM_RBUTTONDOWN)
            {
                IsStacking = false;
                EnableHeroStacking = false;
                CurrentCamp.IsStacking = false;
            }

            if (!IsHero && args.Msg == (ulong)Utils.WindowsMessages.WM_LBUTTONDOWN && IsUnderCampNameText)
            {
                ChangeCamp();
            }
        }

        private double GetAttackPoint()
        {
            return attackAnimationPoint / (1 + (Unit.AttacksPerSecond * attackRate / 0.01 - 100) / 100);
        }
    }
}