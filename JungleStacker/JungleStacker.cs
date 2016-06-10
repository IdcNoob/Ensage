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

        private bool inGame;

        private bool isEnabled;

        private JungleCamps jungleCamps;

        #endregion

        #region Constructors and Destructors

        public JungleStacker()
        {
            // okay, events was made only for testing purposes...
            menu.OnProgramStateChange += OnProgramStateChange;
            menu.OnHeroStack += OnHeroStack;
            menu.OnForceAdd += OnForceAdd;

            ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalEarth);
            ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalFire);
            ignoredUnits.Add(ClassID.CDOTA_Unit_Brewmaster_PrimalStorm);
        }

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            DelayAction.Add(
                300f + Game.Ping,
                () =>
                    {
                        if (!inGame)
                        {
                            return;
                        }

                        try
                        {
                            var unit = args.Entity as Unit;

                            if (unit == null || !unit.IsControllable || unit.Team != heroTeam
                                || unit.AttackCapability == AttackCapability.None || ignoredUnits.Contains(unit.ClassID))
                            {
                                return;
                            }

                            var contrallable = new Controllable(unit);
                            contrallable.OnCampChange += OnCampChange;
                            controllableUnits.Add(contrallable);
                        }
                        catch (EntityNotFoundException e)
                        {
                            // Console.WriteLine(e);
                        }
                    });
        }

        public void OnClose()
        {
            inGame = false;
            jungleCamps.OnClose();
            controllableUnits.ForEach(x => x.OnClose());
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
                    (float)ability.FindCastPoint() * 1000 + 300 + Game.Ping,
                    () =>
                        {
                            if (inGame && target.IsControllable)
                            {
                                var unit = new Controllable(target);
                                unit.OnCampChange += OnCampChange;
                                controllableUnits.Add(unit);
                            }
                        });
            }
        }

        public void OnLoad()
        {
            inGame = true;
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;
            jungleCamps = new JungleCamps(heroTeam);
            Camp.DisplayOverlay = menu.IsEnabled;
            isEnabled = menu.IsEnabled;
            controllableUnits.Add(new Controllable(hero, true));
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
            if (!Ensage.Common.Utils.SleepCheck("JungleStacking.Sleep"))
            {
                return;
            }

            if (Game.IsPaused || !isEnabled)
            {
                Ensage.Common.Utils.Sleep(1000, "JungleStacking.Sleep");
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

            foreach (var camp in
                jungleCamps.GetCamps.Where(
                    x => !x.IsCleared && x.CurrentStacksCount < x.RequiredStacksCount && !x.IsStacking)
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
                    Creeps.All.Where(x => x.Distance2D(camp.CampPosition) < 600 && x.IsNeutral && x.Team != heroTeam)
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

            Ensage.Common.Utils.Sleep(500, "JungleStacking.Sleep");
        }

        #endregion

        #region Methods

        private void OnCampChange(object sender, CampArgs args)
        {
            var blockedCamps = args.BlockedCamps;
            var contrallable = args.Controllable;

            var allCamps =
                jungleCamps.GetCamps.Where(x => !x.IsCleared && x.CurrentStacksCount < x.RequiredStacksCount)
                    .OrderByDescending(x => x.Ancients);

            var nextCamp = allCamps.FirstOrDefault(x => !blockedCamps.Contains(x));

            if (nextCamp != null)
            {
                if (nextCamp.IsStacking)
                {
                    var change = controllableUnits.FirstOrDefault(x => x.CurrentCamp == nextCamp);
                    change?.Stack(blockedCamps.Last(), 2);
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
            isEnabled = enabled;

            if (enabled)
            {
                return;
            }

            controllableUnits.ForEach(x => x.OnClose());
            jungleCamps.GetCamps.ForEach(x => x.OnDisable());
        }

        #endregion
    }
}