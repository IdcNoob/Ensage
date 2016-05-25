namespace JungleStacker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using global::JungleStacker.Classes;
    using global::JungleStacker.Utils;

    internal class JungleStacker
    {
        #region Fields

        private readonly List<Controllable> controllableUnits = new List<Controllable>();

        private readonly List<ClassID> ignoredUnits = new List<ClassID>();

        private readonly MenuManager menu = new MenuManager();

        private CampsInfo campsInfo;

        #endregion

        #region Constructors and Destructors

        public JungleStacker()
        {
            // okay, this one was made only for testing purposes...
            this.menu.MenuChanged += this.OnMenuChange;

            this.ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalEarth);
            this.ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalFire);
            this.ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalStorm);
        }

        #endregion

        #region Properties

        private Hero Hero { get; set; }

        private Team HeroTeam { get; set; }

        private bool IsEnabled { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            DelayAction.Add(
                300f,
                () =>
                    {
                        var unit = args.Entity as Unit;

                        if (unit == null || !unit.IsControllable || unit.Team != this.HeroTeam
                            || unit.AttackCapability == AttackCapability.None
                            || this.ignoredUnits.Contains(unit.ClassID))
                        {
                            return;
                        }

                        this.controllableUnits.Add(new Controllable(unit));
                    });
        }

        public void OnClose()
        {
            this.campsInfo.OnClose();

            foreach (var controllableUnit in this.controllableUnits)
            {
                controllableUnit.OnClose();
            }
        }

        public void OnExecuteAction(Ability ability, Entity entity)
        {
            if (ability == null)
            {
                return;
            }

            var target = entity as Unit;

            if (target == null)
            {
                return;
            }

            if (ability.StoredName() == "item_helm_of_the_dominator"
                || ability.ClassID == ClassID.CDOTA_Ability_Chen_HolyPersuasion
                || ability.ClassID == ClassID.CDOTA_Ability_Enchantress_Enchant)
            {
                DelayAction.Add(
                    (float)ability.FindCastPoint() * 1000 + Game.Ping,
                    () => { this.controllableUnits.Add(new Controllable(target)); });
            }
        }

        public void OnLoad()
        {
            this.Hero = ObjectManager.LocalHero;
            this.HeroTeam = this.Hero.Team;
            this.campsInfo = new CampsInfo(this.HeroTeam);
            Camp.DisplayOverlay = this.menu.IsEnabled;
            this.IsEnabled = this.menu.IsEnabled;
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit == null || unit.Team != this.HeroTeam)
            {
                return;
            }

            var removeUnit = this.controllableUnits.FirstOrDefault(x => x.Handle == unit.Handle);

            if (removeUnit == null)
            {
                return;
            }

            removeUnit.OnClose();
            this.controllableUnits.Remove(removeUnit);
        }

        public void OnUpdate()
        {
            if (!Ensage.Common.Utils.SleepCheck("JungleStacking.Sleep"))
            {
                return;
            }

            if (Game.IsPaused || !this.IsEnabled)
            {
                Ensage.Common.Utils.Sleep(1000, "JungleStacking.Sleep");
                return;
            }

            foreach (var camp in
                this.campsInfo.GetCamps.Where(
                    x => !x.Cleared && x.CurrentStacksCount < x.RequiredStacksCount && !x.IsStacking)
                    .OrderByDescending(x => x.Ancients))
            {
                var unit = this.controllableUnits.FirstOrDefault(x => x.IsValid && !x.IsStacking);

                if (unit == null)
                {
                    break;
                }

                unit.Stack(camp);
            }

            foreach (var camp in
                this.campsInfo.GetCamps.Where(
                    x =>
                    !this.controllableUnits.Any(
                        z =>
                        z.CurrentCamp == x
                        && (z.CurrentStatus == Controllable.Status.MovingToStackPosition
                            || z.CurrentStatus == Controllable.Status.WaitingOnStackPosition
                            || z.CurrentStatus == Controllable.Status.TryingToCheckStacks))))
            {
                var creeps =
                    Creeps.All.Where(x => x.Distance2D(camp.CampPosition) < 1000 && x.IsSpawned && x.IsNeutral).ToList();

                if (creeps.Any(x => x.IsAlive))
                {
                    camp.CurrentStacksCount = Math.Max(creeps.CountStacks(), 1);
                    camp.Cleared = false;
                }
                else if (!camp.Cleared && creeps.All(x => !x.IsAlive) && creeps.Any())
                {
                    camp.Cleared = true;
                    camp.CurrentStacksCount = 0;
                }
                else if ((int)Game.GameTime % 60 == 0 && camp.CurrentStacksCount <= 0)
                {
                    camp.CurrentStacksCount = 1;
                    camp.Cleared = false;
                }
            }

            Ensage.Common.Utils.Sleep(500, "JungleStacking.Sleep");
        }

        #endregion

        #region Methods

        private void OnMenuChange(object sender, MenuArgs args)
        {
            var enabled = args.Enabled;

            Camp.DisplayOverlay = enabled;
            this.IsEnabled = enabled;

            if (enabled)
            {
                return;
            }

            foreach (var unit in this.controllableUnits)
            {
                unit.IsStacking = false;
                unit.OnClose();
            }
            foreach (var camp in this.campsInfo.GetCamps)
            {
                camp.IsStacking = false;
            }
        }

        #endregion
    }
}