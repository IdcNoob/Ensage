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

        private bool isRanged;

        private float pause;

        private bool registered;

        #endregion

        #region Constructors and Destructors

        public Controllable(Unit unit, bool isHero = false)
        {
            this.Handle = unit.Handle;
            this.Unit = unit;
            this.IsHero = isHero;
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
            TryingToCheckStacks,
            Done
        }

        #endregion

        #region Public Properties

        public Camp CurrentCamp { get; private set; }

        public Status CurrentStatus { get; private set; }

        public bool EnableHeroStacking { get; set; }

        public uint Handle { get; private set; }

        public bool IsHero { get; private set; }

        public bool IsStacking { get; set; }

        public bool IsValid
        {
            get
            {
                return this.Unit != null && this.Unit.IsValid && this.Unit.IsAlive;
            }
        }

        public Unit Unit { get; private set; }

        #endregion

        #region Properties

        private bool IsUnderCampNameText
        {
            get
            {
                return Utils.IsUnderRectangle(
                    Game.MouseScreenPosition,
                    this.campNameTextPosition.X,
                    this.campNameTextPosition.Y,
                    this.MeasureTextSize.X,
                    this.MeasureTextSize.Y);
            }
        }

        private Vector2 MeasureTextSize
        {
            get
            {
                return Drawing.MeasureText(this.CurrentCamp.Name, "Arial", new Vector2(15), FontFlags.None);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            this.registered = false;
            this.IsStacking = false;
            Game.OnUpdate -= this.Game_OnUpdate;
            Drawing.OnDraw -= this.Drawing_OnDraw;
            Game.OnWndProc -= this.Game_OnWndProc;
        }

        public void Stack(Camp camp, int delay = 0)
        {
            this.CurrentCamp = camp;
            this.CurrentCamp.IsStacking = true;
            this.IsStacking = true;
            this.CurrentStatus = Status.Idle;
            this.isRanged = this.Unit.AttackCapability == AttackCapability.Ranged;
            this.campAvailable = false;
            this.pause = Game.GameTime + delay;

            if (!this.registered)
            {
                Game.OnUpdate += this.Game_OnUpdate;
                Drawing.OnDraw += this.Drawing_OnDraw;
                Game.OnWndProc += this.Game_OnWndProc;
                this.registered = true;
            }
        }

        #endregion

        #region Methods

        protected virtual void OnMenuChanged(List<Camp> camps, Controllable cntrollable)
        {
            var onCampChange = this.OnCampChange;
            if (onCampChange != null)
            {
                onCampChange.Invoke(this, new CampArgs { BlockedCamps = camps, Controllable = cntrollable });
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!this.IsStacking || !this.IsValid)
            {
                return;
            }

            this.campNameTextPosition = HUDInfo.GetHPbarPosition(this.Unit) + new Vector2(10, this.IsHero ? 25 : 0);

            if (this.campNameTextPosition.IsZero)
            {
                return;
            }

            var campName = new DrawText
                               {
                                   Position = this.campNameTextPosition, Text = this.CurrentCamp.Name, Color = Color.White,
                                   TextSize = new Vector2(15)
                               };

            if (this.IsUnderCampNameText && !this.IsHero)
            {
                campName.Color = Color.Orange;
            }

            campName.Draw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Utils.SleepCheck("JungleStacking.Stack." + this.Handle))
            {
                return;
            }

            Utils.Sleep(200, "JungleStacking.Stack." + this.Handle);

            var gameTime = Game.GameTime;

            if (Game.IsPaused || this.pause > gameTime || (!this.EnableHeroStacking && this.IsHero))
            {
                return;
            }

            if (!this.IsValid
                || (this.CurrentCamp.CurrentStacksCount >= this.CurrentCamp.RequiredStacksCount && !this.IsHero))
            {
                if (this.campAvailable)
                {
                    return;
                }
                this.CurrentStatus = Status.Done;
                this.CurrentCamp.IsStacking = false;
                this.IsStacking = false;
                this.campAvailable = true;
                return;
            }

            switch (this.CurrentStatus)
            {
                case Status.Idle:
                    this.Unit.Move(this.CurrentCamp.WaitPosition);
                    this.CurrentStatus = Status.MovingToWaitPosition;
                    return;
                case Status.MovingToWaitPosition:
                    if (this.Unit.Distance2D(this.CurrentCamp.WaitPosition) > 50)
                    {
                        return;
                    }
                    this.CurrentStatus = Status.WaitingStackTime;
                    return;
                case Status.WaitingStackTime:
                    var seconds = gameTime % 60;
                    if (seconds >= 57)
                    {
                        return;
                    }

                    var target =
                        Creeps.All.OrderBy(x => x.Distance2D(this.Unit))
                            .FirstOrDefault(
                                x =>
                                x.Distance2D(this.Unit) <= 600 && x.IsSpawned && x.IsAlive && x.IsNeutral
                                && !x.Equals(this.Unit));

                    if (seconds + (this.CurrentCamp.CurrentStacksCount >= 3 ? 1 : 0)
                        + (this.isRanged && target != null
                               ? this.Unit.AttacksPerSecond
                               : this.CurrentCamp.CampPosition.Distance2D(this.Unit) / this.Unit.MovementSpeed)
                        >= this.CurrentCamp.StackTime)
                    {
                        if (target != null && this.isRanged)
                        {
                            this.Unit.Attack(target);
                        }
                        else
                        {
                            this.Unit.Move(this.CurrentCamp.CampPosition);
                        }
                        this.health = this.Unit.Health;
                        this.CurrentStatus = Status.MovingToCampPosition;
                    }
                    return;
                case Status.MovingToCampPosition:
                    if (this.Unit.Health < this.health)
                    {
                        this.Unit.Move(this.CurrentCamp.StackPosition);
                        this.CurrentStatus = Status.MovingToStackPosition;
                    }
                    else if (this.isRanged)
                    {
                        if (this.attackTime <= 0 && this.Unit.IsAttacking())
                        {
                            this.attackTime = gameTime;
                        }
                        else if (this.attackTime > 0 && gameTime >= this.Unit.AttacksPerSecond / 2 + this.attackTime)
                        {
                            this.attackTime = 0;
                            this.Unit.Move(this.CurrentCamp.StackPosition);
                            this.CurrentStatus = Status.MovingToStackPosition;
                        }
                    }
                    else if (this.Unit.Distance2D(this.CurrentCamp.CampPosition) < 50)
                    {
                        this.Unit.Move(this.CurrentCamp.StackPosition);
                        this.CurrentStatus = Status.MovingToStackPosition;
                    }
                    return;
                case Status.MovingToStackPosition:
                    if (this.CurrentCamp.StackPosition.Distance2D(this.Unit) < 50)
                    {
                        this.CurrentStatus = Status.WaitingOnStackPosition;
                    }
                    return;
                case Status.WaitingOnStackPosition:
                    var time = gameTime % 60;
                    if (time > 5 && time < 10)
                    {
                        this.Unit.Move(this.CurrentCamp.WaitPosition);
                        this.CurrentCamp.CurrentStacksCount++;
                        this.CurrentStatus = Status.TryingToCheckStacks;
                    }
                    return;
                case Status.TryingToCheckStacks:
                    if (this.Unit.Distance2D(this.CurrentCamp.WaitPosition) < 50)
                    {
                        this.CurrentCamp.CurrentStacksCount =
                            Creeps.All.Where(
                                x =>
                                x.Distance2D(this.CurrentCamp.CampPosition) < 1000 && x.IsSpawned && x.IsNeutral
                                && !x.Equals(this.Unit)).ToList().CountStacks();

                        if (this.CurrentCamp.CurrentStacksCount >= this.CurrentCamp.RequiredStacksCount)
                        {
                            this.CurrentCamp.IsStacking = false;
                            this.IsStacking = false;
                            this.CurrentStatus = Status.Done;
                        }
                        else
                        {
                            this.CurrentStatus = Status.Idle;
                        }
                    }
                    return;
            }
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (!this.IsValid || !this.IsStacking)
            {
                return;
            }
            if (this.IsHero && args.Msg == (ulong)Utils.WindowsMessages.WM_RBUTTONDOWN)
            {
                this.IsStacking = false;
                this.EnableHeroStacking = false;
                this.CurrentCamp.IsStacking = false;
            }

            if (!this.IsHero && args.Msg == (ulong)Utils.WindowsMessages.WM_LBUTTONDOWN && this.IsUnderCampNameText)
            {
                this.BlockedCamps.Add(this.CurrentCamp);
                this.IsStacking = false;
                this.CurrentCamp.IsStacking = false;
                this.OnMenuChanged(this.BlockedCamps, this);
            }
        }

        #endregion
    }
}