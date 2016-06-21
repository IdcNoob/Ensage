namespace JungleStacker.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.DrawObjects;

    using global::JungleStacker.Utils;

    using SharpDX;

    internal class CampArgs : EventArgs
    {
        #region Fields

        public List<Camp> BlockedCamps = new List<Camp>();

        #endregion

        #region Public Properties

        public Controllable Controllable { get; set; }

        #endregion
    }

    internal class Controllable
    {
        #region Fields

        public readonly List<Camp> BlockedCamps = new List<Camp>();

        private float attackTime;

        private bool campAvailable;

        private Vector2 campNameTextPosition;

        private uint health;

        private float hPBarSizeX;

        private bool isRanged;

        private float pause;

        private bool registered;

        #endregion

        #region Constructors and Destructors

        public Controllable(Unit unit, bool isHero = false)
        {
            Handle = unit.Handle;
            Unit = unit;
            IsHero = isHero;
        }

        #endregion

        #region Public Events

        public event EventHandler<CampArgs> OnCampChange;

        #endregion

        #region Enums

        public enum Status
        {
            Idle,

            MovingToWaitPosition,

            WaitingStackTime,

            MovingToCampPosition,

            MovingToStackPosition,

            WaitingOnStackPosition,

            WaitingOnStackPositionToPreventBlock,

            TryingToCheckStacks,

            PreventCampBlock,

            Done
        }

        #endregion

        #region Public Properties

        public Camp CurrentCamp { get; private set; }

        public Status CurrentStatus { get; private set; }

        public bool EnableHeroStacking { get; set; }

        public uint Handle { get; }

        public bool IsHero { get; }

        public bool IsStacking { get; set; }

        public bool IsValid => Unit != null && Unit.IsValid && Unit.IsAlive;

        public Unit Unit { get; }

        #endregion

        #region Properties

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

        private bool IsUnderCampNameText
            =>
                Utils.IsUnderRectangle(
                    Game.MouseScreenPosition,
                    campNameTextPosition.X,
                    campNameTextPosition.Y,
                    MeasureCampNameTextSize.X,
                    MeasureCampNameTextSize.Y);

        private Vector2 MeasureCampNameTextSize
            => Drawing.MeasureText(CurrentCamp.Name, "Arial", new Vector2(16), FontFlags.None);

        #endregion

        #region Public Methods and Operators

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
            isRanged = Unit.AttackCapability == AttackCapability.Ranged;
            campAvailable = false;
            pause = Game.GameTime + delay;

            if (!registered)
            {
                Game.OnUpdate += Game_OnUpdate;
                Drawing.OnDraw += Drawing_OnDraw;
                Game.OnWndProc += Game_OnWndProc;
                registered = true;
            }
        }

        #endregion

        #region Methods

        protected virtual void OnMenuChanged(List<Camp> camps, Controllable cntrollable)
        {
            var onCampChange = OnCampChange;
            onCampChange?.Invoke(this, new CampArgs { BlockedCamps = camps, Controllable = cntrollable });
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsStacking || !IsValid)
            {
                return;
            }

            campNameTextPosition = HUDInfo.GetHPbarPosition(Unit)
                                   + new Vector2((GetHpBarSizeX - MeasureCampNameTextSize.X) / 2, IsHero ? 25 : 0);

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
                                   Position = campNameTextPosition, Text = text, Color = Color.White,
                                   TextSize = new Vector2(16)
                               };

            if (IsUnderCampNameText && !IsHero)
            {
                campName.Color = Color.Orange;
            }

            campName.Draw();
        }

        public static bool Debug { set; get; }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Utils.SleepCheck("JungleStacking.Stack." + Handle))
            {
                return;
            }

            Utils.Sleep(200, "JungleStacking.Stack." + Handle);

            var gameTime = Game.GameTime;

            if (Game.IsPaused || pause > gameTime || (!EnableHeroStacking && IsHero) || !IsStacking)
            {
                return;
            }

            if (!IsValid || (CurrentCamp.CurrentStacksCount >= CurrentCamp.RequiredStacksCount && !IsHero))
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

                    var seconds = gameTime % 60;
                    if (seconds >= 57)
                    {
                        return;
                    }

                    var target =
                        Creeps.All.OrderBy(x => x.Distance2D(Unit))
                            .FirstOrDefault(
                                x =>
                                x.Distance2D(Unit) <= 600 && x.IsSpawned && x.IsAlive && x.IsNeutral && !x.Equals(Unit));

                    if (seconds + (CurrentCamp.CurrentStacksCount >= 3 ? 1 : 0)
                        + (isRanged && target != null
                               ? Unit.AttacksPerSecond
                               : CurrentCamp.CampPosition.Distance2D(Unit) / Unit.MovementSpeed)
                        >= CurrentCamp.StackTime)
                    {
                        if (target != null && isRanged)
                        {
                            Unit.Attack(target);
                        }
                        else
                        {
                            Unit.Move(CurrentCamp.CampPosition);
                        }
                        health = Unit.Health;
                        CurrentStatus = Status.MovingToCampPosition;
                    }
                    return;
                case Status.PreventCampBlock:
                    Unit.Move(CurrentCamp.StackPosition);
                    CurrentStatus = Status.WaitingOnStackPositionToPreventBlock;
                    return;
                case Status.MovingToCampPosition:
                    if (Unit.Health < health)
                    {
                        Unit.Move(CurrentCamp.StackPosition);
                        CurrentStatus = Status.MovingToStackPosition;
                    }
                    else if (isRanged)
                    {
                        if (attackTime <= 0 && Unit.IsAttacking())
                        {
                            attackTime = gameTime;
                        }
                        else if (attackTime > 0 && gameTime >= Unit.AttacksPerSecond / 2 + attackTime)
                            //else if (this.attackTime > 0
                            //         && gameTime
                            //         >= this.Unit.SecondsPerAttack - this.Unit.BaseAttackTime / 3 + this.attackTime)
                        {
                            attackTime = 0;
                            Unit.Move(CurrentCamp.StackPosition);
                            CurrentStatus = Status.MovingToStackPosition;
                        }
                    }
                    else if (Unit.Distance2D(CurrentCamp.CampPosition) < 50)
                    {
                        Unit.Move(CurrentCamp.StackPosition);
                        CurrentStatus = Status.MovingToStackPosition;
                    }
                    return;
                case Status.MovingToStackPosition:
                    if (CurrentCamp.StackPosition.Distance2D(Unit) < 50)
                    {
                        CurrentStatus = Status.WaitingOnStackPosition;
                    }
                    return;
                case Status.WaitingOnStackPositionToPreventBlock:
                    if (CurrentCamp.CurrentStacksCount >= 1)
                    {
                        CurrentStatus = Status.Idle;
                    }
                    return;
                case Status.WaitingOnStackPosition:
                    var time = gameTime % 60;
                    if (time > 5 && time < 10)
                    {
                        Unit.Move(CurrentCamp.WaitPosition);
                        CurrentCamp.CurrentStacksCount++;
                        CurrentStatus = Status.TryingToCheckStacks;
                    }
                    return;
                case Status.TryingToCheckStacks:
                    if (Unit.Distance2D(CurrentCamp.WaitPosition) < 50)
                    {
                        CurrentCamp.CurrentStacksCount =
                            Creeps.All.Where(
                                x =>
                                x.Distance2D(CurrentCamp.CampPosition) < 1000 && x.IsSpawned && x.IsNeutral
                                && !x.Equals(Unit)).ToList().CountStacks();

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
                BlockedCamps.Add(CurrentCamp);
                IsStacking = false;
                CurrentCamp.IsStacking = false;
                OnMenuChanged(BlockedCamps, this);
            }
        }

        #endregion
    }
}