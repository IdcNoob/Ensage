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

        private Hero hero;

        private Team heroTeam;

        private bool isEnabled;

        private JungleCamps jungleCamps;

        #endregion

        #region Constructors and Destructors

        public JungleStacker()
        {
            // okay, events was made only for testing purposes...
            this.menu.OnProgramStateChange += this.OnProgramStateChange;
            this.menu.OnHeroStack += this.OnHeroStack;
            this.menu.OnForceAdd += this.OnForceAdd;

            this.ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalEarth);
            this.ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalFire);
            this.ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalStorm);
        }

        private void OnForceAdd(object sender, EventArgs e)
        {
            var selected = hero.Player.Selection.FirstOrDefault() as Unit;

            if (selected == null || !selected.IsControllable || selected is Hero)
            {
                return;
            }

            if (!controllableUnits.Exists(x => x.Handle == selected.Handle))
            {
                var ctrl = new Controllable(selected);
                ctrl.OnCampChange += OnCampChange;
                controllableUnits.Add(ctrl);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            DelayAction.Add(
                300f,
                () =>
                    {
                        var unit = args.Entity as Unit;

                        if (unit == null || !unit.IsControllable || unit.Team != this.heroTeam
                            || unit.AttackCapability == AttackCapability.None
                            || this.ignoredUnits.Contains(unit.ClassID))
                        {
                            return;
                        }

                        var contrallable = new Controllable(unit);
                        contrallable.OnCampChange += this.OnCampChange;
                        this.controllableUnits.Add(contrallable);
                    });
        }

        public void OnClose()
        {
            this.jungleCamps.OnClose();
            this.controllableUnits.ForEach(x => x.OnClose());
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
                    (float)ability.FindCastPoint() * 1000 + 500,
                    () =>
                        {
                            if (target.IsControllable)
                            {
                                var unit = new Controllable(target);
                                unit.OnCampChange += this.OnCampChange;
                                this.controllableUnits.Add(unit);
                            }
                        });
            }
        }

        public void OnLoad()
        {
            this.hero = ObjectManager.LocalHero;
            this.heroTeam = this.hero.Team;
            this.jungleCamps = new JungleCamps(this.heroTeam);
            Camp.DisplayOverlay = this.menu.IsEnabled;
            this.isEnabled = this.menu.IsEnabled;
            this.controllableUnits.Add(new Controllable(this.hero, true));
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit == null || unit.Team != this.heroTeam)
            {
                return;
            }

            var removeUnit = this.controllableUnits.FirstOrDefault(x => x.Handle == unit.Handle);

            if (removeUnit == null)
            {
                return;
            }

            removeUnit.OnCampChange -= this.OnCampChange;
            removeUnit.OnClose();
            this.controllableUnits.Remove(removeUnit);
        }

        public void OnUpdate()
        {
            if (!Ensage.Common.Utils.SleepCheck("JungleStacking.Sleep"))
            {
                return;
            }

            if (Game.IsPaused || !this.isEnabled)
            {
                Ensage.Common.Utils.Sleep(1000, "JungleStacking.Sleep");
                return;
            }

            var heroControl =
                this.controllableUnits.FirstOrDefault(x => x.IsHero && !x.IsStacking && x.EnableHeroStacking);

            if (heroControl != null)
            {
                var closestCamp =
                    this.jungleCamps.GetCamps.OrderBy(x => x.CampPosition.Distance2D(this.hero)).FirstOrDefault();
                if (closestCamp != null)
                {
                    heroControl.Stack(closestCamp);
                }
            }

            foreach (var camp in
                this.jungleCamps.GetCamps.Where(
                    x => !x.IsCleared && x.CurrentStacksCount < x.RequiredStacksCount && !x.IsStacking)
                    .OrderByDescending(x => x.Ancients))
            {
                var unit = this.controllableUnits.FirstOrDefault(x => x.IsValid && !x.IsStacking && !x.IsHero);

                if (unit == null)
                {
                    break;
                }

                unit.Stack(camp);
            }

            foreach (var camp in
                this.jungleCamps.GetCamps.Where(
                    x =>
                    !this.controllableUnits.Any(
                        z =>
                        z.CurrentCamp == x
                        && (z.CurrentStatus == Controllable.Status.MovingToStackPosition
                            || z.CurrentStatus == Controllable.Status.WaitingOnStackPosition
                            || z.CurrentStatus == Controllable.Status.TryingToCheckStacks))))
            {
                var creeps =
                    Creeps.All.Where(x => x.Distance2D(camp.CampPosition) < 1000 && x.IsSpawned && x.IsNeutral && x.Team != this.heroTeam).ToList();

                if (creeps.Any(x => x.IsAlive))
                {
                    camp.CurrentStacksCount = Math.Max(creeps.CountStacks(), 1);
                    camp.IsCleared = false;
                }
                else if (!camp.IsCleared && creeps.All(x => !x.IsAlive) && creeps.Any())
                {
                    camp.IsCleared = true;
                    camp.CurrentStacksCount = 0;
                }
                else if ((int)Game.GameTime % 60 == 0 && camp.CurrentStacksCount <= 0)
                {
                    camp.CurrentStacksCount = 1;
                    camp.IsCleared = false;
                }
            }

            Ensage.Common.Utils.Sleep(500, "JungleStacking.Sleep");
        }

        #endregion

        #region Methods

        private void OnCampChange(object sender, CampArgs args)
        {
            var blockedCamps = args.BlockedCamps;
            var contrallable = args.Controllable;

            var allCamps =
                this.jungleCamps.GetCamps.Where(x => !x.IsCleared && x.CurrentStacksCount < x.RequiredStacksCount)
                    .OrderByDescending(x => x.Ancients);

            var nextCamp = allCamps.FirstOrDefault(x => !blockedCamps.Contains(x));

            if (nextCamp != null)
            {
                if (nextCamp.IsStacking)
                {
                    var change = this.controllableUnits.FirstOrDefault(x => x.CurrentCamp == nextCamp);
                    if (change != null)
                    {
                        change.Stack(blockedCamps.Last(), 2);
                    }
                }

                contrallable.Stack(nextCamp, 2);
                return;
            }

            var available = allCamps.FirstOrDefault(x => !x.IsStacking);

            if (available != null)
            {
                contrallable.BlockedCamps.Clear();
                contrallable.Stack(available, 2);
            }
        }

        private void OnHeroStack(object sender, EventArgs e)
        {
            var heroCtrl = this.controllableUnits.FirstOrDefault(x => x.Handle == this.hero.Handle);

            if (heroCtrl != null)
            {
                heroCtrl.EnableHeroStacking = true;
            }
        }

        private void OnProgramStateChange(object sender, MenuArgs args)
        {
            var enabled = args.Enabled;

            Camp.DisplayOverlay = enabled;
            this.isEnabled = enabled;

            if (enabled)
            {
                return;
            }

            this.controllableUnits.ForEach(x => x.OnClose());
            this.jungleCamps.GetCamps.ForEach(x => x.IsStacking = false);
        }

        #endregion
    }
}