namespace JungleStacker.Classes
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.DrawObjects;

    using SharpDX;

    internal class Controllable
    {
        #region Fields

        private float attackTime;

        private bool campAvailable;

        private uint health;

        private bool isRanged;

        private bool registered;

        #endregion

        #region Constructors and Destructors

        public Controllable(Unit unit)
        {
            this.Handle = unit.Handle;
            this.Unit = unit;
        }

        #endregion

        #region Enums

        public enum Status
        {
            Idle,
            MovingToWaitPosition,
            WaitingStackTime,
            MovingToStackPosition,
            MovingToCampPosition,
            WaitingOnStackPosition,
            TryingToCheckStacks,
            Done
        }

        #endregion

        #region Public Properties

        public Camp CurrentCamp { get; private set; }

        public Status CurrentStatus { get; private set; }

        public uint Handle { get; private set; }

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

        #region Public Methods and Operators

        public void OnClose()
        {
            this.registered = false;
            Game.OnUpdate -= this.Game_OnUpdate;
            Drawing.OnDraw -= this.Drawing_OnDraw;
        }

        public void Stack(Camp camp)
        {
            this.CurrentCamp = camp;
            this.CurrentCamp.IsStacking = true;
            this.IsStacking = true;
            this.CurrentStatus = Status.Idle;
            this.isRanged = this.Unit.AttackCapability == AttackCapability.Ranged;

            if (!this.registered)
            {
                Game.OnUpdate += this.Game_OnUpdate;
                Drawing.OnDraw += this.Drawing_OnDraw;
                this.registered = true;
            }
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!this.IsStacking || !this.IsValid)
            {
                return;
            }

            var hpBar = HUDInfo.GetHPbarPosition(this.Unit) + new Vector2(10, 0);

            if (hpBar.IsZero)
            {
                return;
            }

            var text = new DrawText
                           {
                               Position = hpBar, Text = this.CurrentCamp.Name, Color = Color.White,
                               TextSize = new Vector2(15)
                           };
            text.Draw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Utils.SleepCheck("JungleStacking.Stack." + this.Handle))
            {
                return;
            }

            Utils.Sleep(200, "JungleStacking.Stack." + this.Handle);

            if (Game.IsPaused)
            {
                return;
            }

            if (!this.IsValid)
            {
                if (this.campAvailable)
                {
                    return;
                }
                this.CurrentCamp.IsStacking = false;
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
                    var target =
                        Creeps.All.OrderBy(x => x.Distance2D(this.Unit))
                            .FirstOrDefault(
                                x => x.Distance2D(this.Unit) <= 600 && x.IsSpawned && x.IsAlive && x.IsNeutral);

                    if (Game.GameTime % 60
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
                            this.attackTime = Game.GameTime;
                        }
                        else if (this.attackTime > 0 && Game.GameTime >= this.Unit.AttacksPerSecond / 2 + this.attackTime)
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
                    var time = Game.GameTime % 60;
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
                        this.CurrentCamp.IsStacking = false;
                        this.IsStacking = false;
                        this.CurrentStatus = Status.Done;
                    }
                    return;
            }
        }

        #endregion
    }
}