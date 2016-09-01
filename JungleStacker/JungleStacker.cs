namespace JungleStacker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Classes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using Utils;

    internal class JungleStacker
    {
        #region Fields

        private readonly List<Controllable> controllableUnits = new List<Controllable>();

        private readonly List<ClassID> ignoredUnits = new List<ClassID>();

        private readonly MenuManager menu = new MenuManager();

        private Unit delayedUnit;

        private Hero hero;

        private Team heroTeam;

        private JungleCamps jungleCamps;

        private Sleeper sleeper;

        #endregion

        #region Constructors and Destructors

        public JungleStacker()
        {
            // okay, events was made only for testing purposes...
            menu.OnProgramStateChange += OnProgramStateChange;
            menu.OnHeroStack += OnHeroStack;
            menu.OnForceAdd += OnForceAdd;
            menu.OnStacksReset += OnStacksReset;

            ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalEarth);
            ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalFire);
            ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalStorm);
        }

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit == null || !unit.IsValid || !unit.IsControllable || unit.Team != heroTeam
                || (unit.IsIllusion && unit.ClassID != hero.ClassID) || unit.AttackCapability == AttackCapability.None
                || ignoredUnits.Contains(unit.ClassID) || unit.Equals(hero))
            {
                return;
            }

            var contrallable = new Controllable(unit);
            contrallable.OnCampChange += OnCampChange;
            controllableUnits.Add(contrallable);
        }

        public void OnClose()
        {
            jungleCamps.OnClose();

            foreach (var unit in controllableUnits)
            {
                unit.OnCampChange -= OnCampChange;
                unit.OnClose();
            }

            controllableUnits.Clear();
            delayedUnit = null;
        }

        public void OnExecuteAction(ExecuteOrderEventArgs args)
        {
            var order = args.Order;

            if (order == Order.Hold || order == Order.MoveLocation)
            {
                foreach (var entity in args.Entities)
                {
                    var controlable = controllableUnits.FirstOrDefault(x => x.Handle == entity.Handle);
                    if (controlable != null)
                    {
                        controlable.Pause = 3;
                    }
                }
                return;
            }

            var ability = args.Ability;

            if (ability == null)
            {
                return;
            }

            var target = args.Target as Unit;

            if (target == null)
            {
                return;
            }

            if (ability.StoredName() == "item_helm_of_the_dominator"
                || ability.ClassID == ClassID.CDOTA_Ability_Chen_HolyPersuasion
                || ability.ClassID == ClassID.CDOTA_Ability_Enchantress_Enchant)
            {
                delayedUnit = target;
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;
            jungleCamps = new JungleCamps(heroTeam);
            Camp.DisplayOverlay = menu.IsEnabled;
            Camp.Debug = Controllable.Debug = menu.IsDebugEnabled;
            controllableUnits.Add(new Controllable(hero, true));
            sleeper = new Sleeper();
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit == null || unit.Team != heroTeam)
            {
                return;
            }

            var removeUnit = controllableUnits.FirstOrDefault(x => x.Handle == unit.Handle);

            if (removeUnit == null)
            {
                return;
            }

            removeUnit.OnCampChange -= OnCampChange;
            removeUnit.OnClose();
            controllableUnits.Remove(removeUnit);
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            if (Game.IsPaused || !menu.IsEnabled)
            {
                sleeper.Sleep(1000);
                return;
            }

            var heroControl = controllableUnits.FirstOrDefault(x => x.IsHero && !x.IsStacking && x.EnableHeroStacking);

            if (heroControl != null)
            {
                var closestCamp = jungleCamps.GetCamps.OrderBy(x => x.CampPosition.Distance2D(hero)).FirstOrDefault();
                if (closestCamp != null)
                {
                    heroControl.Stack(closestCamp);
                }
            }

            if (delayedUnit != null && delayedUnit.IsControllable)
            {
                var unit = new Controllable(delayedUnit);
                unit.OnCampChange += OnCampChange;
                controllableUnits.Add(unit);
                delayedUnit = null;
            }

            foreach (var camp in
                jungleCamps.GetCamps.Where(
                    x => x.RequiredStacksCount > 1 && x.CurrentStacksCount < x.RequiredStacksCount && !x.IsStacking)
                    .OrderByDescending(x => x.Ancients))
            {
                var unit = controllableUnits.FirstOrDefault(x => x.IsValid && !x.IsStacking && !x.IsHero);

                if (unit == null)
                {
                    break;
                }

                unit.Stack(camp);
            }

            foreach (var camp in
                jungleCamps.GetCamps.Where(
                    x =>
                    !controllableUnits.Any(
                        z =>
                        z.CurrentCamp == x
                        && (z.CurrentStatus == Controllable.Status.MovingToStackPosition
                            || z.CurrentStatus == Controllable.Status.WaitingOnStackPosition
                            || z.CurrentStatus == Controllable.Status.TryingToCheckStacks))))
            {
                var campCreeps =
                    Creeps.All.Where(
                        x => x.IsValid && x.Distance2D(camp.CampPosition) < 600 && x.IsNeutral && x.Team != heroTeam)
                        .ToList();

                var aliveCampCreeps = campCreeps.Where(x => x.IsSpawned && x.IsAlive).ToList();

                if (aliveCampCreeps.Any())
                {
                    camp.CurrentStacksCount = Math.Max(aliveCampCreeps.CountStacks(), 1);
                    camp.IsCleared = false;
                }
                else if (!camp.IsCleared && campCreeps.Any() && campCreeps.All(x => !x.IsSpawned || !x.IsAlive))
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

            sleeper.Sleep(500);
        }

        #endregion

        #region Methods

        private void ChangeCamp(Controllable controllable, Camp camp)
        {
            if (camp.IsStacking)
            {
                var usedControllable = controllableUnits.FirstOrDefault(x => x.CurrentCamp == camp);
                usedControllable?.Stack(controllable.BlockedCamps.Last(), 2);
            }

            controllable.Stack(camp, 2);
        }

        private void OnCampChange(object sender, EventArgs e)
        {
            var contrallable = sender as Controllable;

            if (contrallable == null)
            {
                return;
            }

            var allCamps =
                jungleCamps.GetCamps.Where(
                    x => x.RequiredStacksCount > 1 && x.CurrentStacksCount < x.RequiredStacksCount)
                    .OrderByDescending(x => x.Ancients);

            var nextCamp = allCamps.FirstOrDefault(x => !contrallable.BlockedCamps.Contains(x));

            if (nextCamp != null)
            {
                ChangeCamp(contrallable, nextCamp);
            }
            else
            {
                ChangeCamp(contrallable, contrallable.BlockedCamps.First());
                contrallable.BlockedCamps.Clear();
            }
        }

        private void OnForceAdd(object sender, EventArgs e)
        {
            var selected = hero.Player.Selection.FirstOrDefault() as Unit;

            if (selected == null || !selected.IsControllable || selected is Hero)
            {
                return;
            }

            var controllable = controllableUnits.FirstOrDefault(x => x.Handle == selected.Handle);

            if (controllable == null)
            {
                var ctrl = new Controllable(selected);
                ctrl.OnCampChange += OnCampChange;
                controllableUnits.Add(ctrl);
            }
            else
            {
                if (controllable.CurrentCamp != null)
                {
                    controllable.CurrentCamp.IsStacking = false;
                }
                controllable.OnCampChange -= OnCampChange;
                controllable.OnClose();
                controllableUnits.Remove(controllable);
            }
        }

        private void OnHeroStack(object sender, EventArgs e)
        {
            var heroCtrl = controllableUnits.FirstOrDefault(x => x.Handle == hero.Handle);

            if (heroCtrl != null)
            {
                heroCtrl.EnableHeroStacking = true;
            }
        }

        private void OnProgramStateChange(object sender, MenuArgs args)
        {
            var enabled = args.Enabled;
            Camp.DisplayOverlay = enabled;

            if (enabled)
            {
                return;
            }

            controllableUnits.ForEach(x => x.OnDisable());
            jungleCamps.GetCamps.ForEach(x => x.OnDisable());
        }

        private void OnStacksReset(object sender, EventArgs e)
        {
            jungleCamps.GetCamps.ForEach(x => x.ResetStacks());
        }

        #endregion
    }
}