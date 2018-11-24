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
        private readonly List<Controllable> controllableUnits = new List<Controllable>();

        private readonly List<string> ignoredUnits = new List<string>();

        private readonly MenuManager menu = new MenuManager();

        private Unit delayedUnit;

        private Hero hero;

        private Team heroTeam;

        private JungleCamps jungleCamps;

        private Sleeper sleeper;

        public JungleStacker()
        {
            menu.OnProgramStateChange += OnProgramStateChange;
            menu.OnHeroStack += OnHeroStack;
            menu.OnForceAdd += OnForceAdd;
            menu.OnStacksReset += OnStacksReset;

            ignoredUnits.Add("CDOTA_Unit_Brewmaster_PrimalEarth");
            ignoredUnits.Add("CDOTA_Unit_Brewmaster_PrimalFire");
            ignoredUnits.Add("CDOTA_Unit_Brewmaster_PrimalStorm");
        }

        public void OnAddEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit == null || !unit.IsValid || !unit.IsControllable || unit.Team != heroTeam
                || unit.IsIllusion && unit.Name != hero.Name || unit.AttackCapability == AttackCapability.None
                || ignoredUnits.Contains(unit.NetworkName) || unit.Equals(hero))
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
            var order = args.OrderId;

            if (args.IsPlayerInput && (order == OrderId.Hold || order == OrderId.MoveLocation))
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

            if (ability.Id == AbilityId.item_helm_of_the_dominator
                || ability.Id == AbilityId.chen_holy_persuasion
                || ability.Id == AbilityId.enchantress_enchant)
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

            foreach (var camp in jungleCamps.GetCamps
                .Where(x => x.RequiredStacksCount > 1 && x.CurrentStacksCount < x.RequiredStacksCount && !x.IsStacking)
                .OrderByDescending(x => x.Ancients))
            {
                var unit = controllableUnits.FirstOrDefault(x => x.IsValid && !x.IsStacking && !x.IsHero);

                if (unit == null)
                {
                    break;
                }

                unit.Stack(camp);
            }

            foreach (var camp in jungleCamps.GetCamps.Where(
                x => !controllableUnits.Any(
                         z => z.CurrentCamp == x && (z.CurrentStatus == Controllable.Status.MovingToStackPosition
                                                     || z.CurrentStatus == Controllable.Status.WaitingOnStackPosition
                                                     || z.CurrentStatus == Controllable.Status.TryingToCheckStacks))))
            {
                var campCreeps = ObjectManager.GetEntitiesFast<Creep>()
                    .Where(x => x.IsValid && x.Distance2D(camp.CampPosition) < 600 && x.IsNeutral)
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

            var allCamps = jungleCamps.GetCamps
                .Where(x => x.RequiredStacksCount > 1 && x.CurrentStacksCount < x.RequiredStacksCount)
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
    }
}