namespace AbilityLastHitMarker
{
    using System.Collections.Generic;
    using System.Linq;

    using AbilityLastHitMarker.Classes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using SharpDX;

    using Ability = AbilityLastHitMarker.Classes.Ability;
    using Creep = Ensage.Creep;

    internal class LastHitMarker
    {
        #region Fields

        private readonly List<Ability> abilities = new List<Ability>();

        private readonly List<IKillable> killableList = new List<IKillable>();

        private List<Ability> availableAbilities;

        private Hero hero;

        private Team heroTeam;

        private MenuManager menuManager;

        private bool pause;

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            DelayAction.Add(
                1000f,
                () =>
                    {
                        try
                        {
                            var unit = args.Entity as Creep;

                            if (unit == null || unit.Team == heroTeam)
                            {
                                return;
                            }

                            killableList.Add(new Classes.Creep(unit));
                        }
                        catch (EntityNotFoundException)
                        {
                            // ignored
                        }
                    });
        }

        public void OnClose()
        {
            abilities.Clear();
            killableList.Clear();
            menuManager.OnClose();
        }

        public void OnDraw()
        {
            if (pause)
            {
                return;
            }

            var iconSize = menuManager.Size;
            var yPos = menuManager.Yposition;

            foreach (var killable in killableList.Where(x => x.IsValid && x.Distance(hero) < 1000))
            {
                var screenPos = HUDInfo.GetHPbarPosition(killable.Unit);

                if (screenPos.IsZero)
                {
                    continue;
                }

                screenPos += new Vector2(0, yPos + (killable is Tower ? -50 : -25));

                var savedDamages =
                    killable.GetSavedDamage.OrderByDescending(x => x.Value)
                        .Where(x => availableAbilities.Select(z => z.Handle).Contains(x.Key))
                        .ToDictionary(x => x.Key, x => x.Value);

                if (menuManager.SumEnabled)
                {
                    var abilitiesRequired = 0;
                    var damage = 0f;

                    if (!savedDamages.Any(x => x.Value >= killable.Health))
                    {
                        foreach (var availableAbility in availableAbilities)
                        {
                            var savedDamage = 0f;
                            savedDamages.TryGetValue(availableAbility.Handle, out savedDamage);

                            damage += savedDamage;
                            abilitiesRequired++;

                            if (damage >= killable.Health)
                            {
                                break;
                            }
                        }

                        if (damage < killable.Health)
                        {
                            continue;
                        }

                        screenPos += new Vector2((killable.HpBarSize - iconSize * abilitiesRequired) / 2, 0);

                        for (var i = 0; i < abilitiesRequired; i++)
                        {
                            Drawing.DrawRect(
                                screenPos + new Vector2(iconSize * i, 0),
                                new Vector2(iconSize),
                                availableAbilities[i].Textre);
                        }
                        continue;
                    }
                }

                var ability =
                    availableAbilities.FirstOrDefault(
                        x =>
                        x.Handle
                        == savedDamages.OrderBy(z => z.Value).FirstOrDefault(z => z.Value >= killable.Health).Key);

                if (ability == null)
                {
                    continue;
                }

                Drawing.DrawRect(
                    screenPos + new Vector2((killable.HpBarSize - iconSize) / 2, 0),
                    new Vector2(iconSize),
                    ability.Textre);
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;

            foreach (var ability in
                hero.Spellbook.Spells.Where(
                    x =>
                    x.IsNuke() && !AbilityAdjustments.IgnoredAbilities.Contains(x.Name)
                    || AbilityAdjustments.IncludedAbilities.Contains(x.Name)))
            {
                abilities.Add(new Ability(ability));
            }

            foreach (var tower in Towers.GetByTeam(hero.GetEnemyTeam()))
            {
                killableList.Add(new Tower(tower));
            }

            menuManager = new MenuManager(abilities.Select(x => x.Name).ToList(), hero.Name);
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            if (args.Entity.Team == heroTeam)
            {
                return;
            }

            var remove = killableList.FirstOrDefault(x => x.Handle == args.Entity.Handle);

            if (remove == null)
            {
                return;
            }

            killableList.Remove(remove);
        }

        public void OnUpdate()
        {
            if (!Utils.SleepCheck("AbilityLastHit.Sleep"))
            {
                return;
            }

            Utils.Sleep(500, "AbilityLastHit.Sleep");

            if (!menuManager.IsEnabled || !hero.IsAlive)
            {
                pause = true;
                return;
            }

            if (pause)
            {
                pause = false;
            }

            availableAbilities = abilities.Where(x => menuManager.AbilityActive(x.Name) && x.CanBeCasted).ToList();

            foreach (var killable in killableList.Where(x => x.IsValid && x.Distance(hero) < 2000))
            {
                foreach (var ability in abilities.Where(x => x.Level > 0))
                {
                    var creep = killable as Classes.Creep;
                    creep?.SaveDamage(ability.Handle, AbilityDamage.CalculateDamage(ability.Source, hero, creep.Unit));

                    if (ability.DoesTowerDamage)
                    {
                        var tower = killable as Tower;
                        tower?.SaveDamage(
                            ability.Handle,
                            AbilityDamage.CalculateDamage(ability.Source, hero, tower.Unit)
                            / ability.TowerDamageReduction);
                    }
                }
            }
        }

        #endregion
    }
}