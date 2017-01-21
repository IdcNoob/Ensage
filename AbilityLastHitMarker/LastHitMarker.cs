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
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    using Ability = AbilityLastHitMarker.Classes.Ability;
    using Creep = Ensage.Creep;
    using Tower = AbilityLastHitMarker.Classes.Tower;

    internal class LastHitMarker
    {
        #region Fields

        private readonly List<Ability> abilities = new List<Ability>();

        private readonly List<IKillable> killableList = new List<IKillable>();

        private List<Ability> availableAbilities;

        private Hero hero;

        private bool heroMeepo;

        private Team heroTeam;

        private MenuManager menuManager;

        private bool pause;

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            var creep = args.Entity as Creep;

            if (creep == null || !creep.IsValid|| creep.Team == heroTeam)
            {
                return;
            }

            killableList.Add(new Classes.Creep(creep));

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

            foreach (var killable in killableList.Where(x => x.IsValid && x.DamageCalculated))
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

                if (menuManager.SumEnabled && !savedDamages.Any(x => x.Value >= killable.Health))
                {
                    var abilitiesRequired = 0;
                    var damage = 0f;

                    if (heroMeepo)
                    {
                        var poof = availableAbilities.FirstOrDefault();

                        if (poof == null)
                        {
                            continue;
                        }

                        savedDamages.TryGetValue(poof.Handle, out damage);
                        var availableMeepos =
                            Heroes.GetByTeam(heroTeam)
                                .Count(
                                    x =>
                                    x.IsValid && !x.IsIllusion && x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo
                                    && x.IsAlive && x.CanCast() && x.FindSpell("meepo_poof").CanBeCasted());

                        for (var i = 1; i <= availableMeepos; i++)
                        {
                            damage *= i;
                            abilitiesRequired++;

                            if (damage >= killable.Health)
                            {
                                break;
                            }
                        }
                    }
                    else
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
                            availableAbilities[heroMeepo ? 0 : i].Texture);
                    }
                    continue;
                }

                var ability =
                    availableAbilities.FirstOrDefault(
                        x =>
                        x.Handle
                        == savedDamages.OrderBy(z => z.Value).FirstOrDefault(z => z.Value >= killable.Health).Key);

                var drawBorder = false;

                if (ability == null)
                {
                    ability =
                        availableAbilities.FirstOrDefault(
                            x =>
                            x.Handle
                            == savedDamages.OrderBy(z => z.Value)
                                   .FirstOrDefault(z => z.Value + killable.HeroDamage >= killable.Health)
                                   .Key);

                    if (ability == null)
                    {
                        continue;
                    }

                    drawBorder = true;
                }

                var startPosition = screenPos + new Vector2((killable.HpBarSize - iconSize) / 2, 0);
                Drawing.DrawRect(startPosition, new Vector2(iconSize), ability.Texture);

                //debug
                //Drawing.DrawText(
                //    (int)savedDamages.FirstOrDefault(x => ability.Handle == x.Key).Value + " // "
                //    + (int)killable.HeroDamage,
                //    "Arial",
                //    startPosition + new Vector2(0, -30),
                //    new Vector2(17),
                //    Color.Yellow,
                //    FontFlags.None);

                if (!drawBorder)
                {
                    continue;
                }

                var color = Color.DarkOrange;
                var borderSizeReduction = 14;

                // left
                Drawing.DrawRect(
                    new Vector2(startPosition.X, startPosition.Y),
                    new Vector2(iconSize / borderSizeReduction, iconSize),
                    color);

                // bottom
                Drawing.DrawRect(
                    new Vector2(startPosition.X, startPosition.Y + iconSize),
                    new Vector2(iconSize + iconSize / borderSizeReduction, iconSize / borderSizeReduction),
                    color);

                // right
                Drawing.DrawRect(
                    new Vector2(startPosition.X + iconSize, startPosition.Y),
                    new Vector2(iconSize / borderSizeReduction, iconSize),
                    color);

                // top
                Drawing.DrawRect(
                    new Vector2(startPosition.X, startPosition.Y),
                    new Vector2(iconSize, iconSize / borderSizeReduction),
                    color);
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroMeepo = hero.ClassID == ClassID.CDOTA_Unit_Hero_Meepo;
            heroTeam = hero.Team;
            sleeper = new Sleeper();

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

        private Sleeper sleeper;

        public void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(500);

            if (!menuManager.IsEnabled || !hero.IsAlive || !hero.CanCast() && !heroMeepo)
            {
                pause = true;
                return;
            }

            var autoDisable = menuManager.AutoDisableTime;

            if (autoDisable > 0 && Game.GameTime / 60 >= autoDisable)
            {
                menuManager.Disable();
            }

            if (pause)
            {
                pause = false;
            }

            availableAbilities =
                abilities.Where(x => menuManager.AbilityActive(x.Name) && (x.CanBeCasted || heroMeepo)).ToList();

            //debug
            //foreach (var creep in
            //    from source in Creeps.All.Where(x => x.Team != heroTeam && x.IsValid && x.IsAlive && x.IsSpawned)
            //    let creep = new Classes.Creep(source)
            //    where !killableList.Select(x => x.Handle).Contains(source.Handle)
            //    select creep)
            //{
            //    killableList.Add(creep);
            //}

            var myAutoAttackDamage = hero.MinimumDamage + hero.BonusDamage;

            foreach (var killable in killableList.Where(x => x.IsValid && x.Distance(hero) < 1500))
            {
                var autoAttackDamageDone = killable.Unit.DamageTaken(myAutoAttackDamage, DamageType.Physical, hero);

                var siege = killable.Unit.ClassID == ClassID.CDOTA_BaseNPC_Tower
                            || killable.Unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege;

                if (siege)
                {
                    autoAttackDamageDone /= 2;
                }

                killable.HeroDamage = autoAttackDamageDone;

                foreach (var ability in abilities.Where(x => x.Level > 0))
                {
                    var creep = killable as Classes.Creep;

                    if (creep != null)
                    {
                        var damage = AbilityDamage.CalculateDamage(ability.Source, hero, creep.Unit);

                        switch (ability.Name)
                        {
                            case "meepo_poof":
                                damage *= 2;
                                break;
                            case "ember_spirit_sleight_of_fist":
                                damage = autoAttackDamageDone / 2;
                                break;
                            case "templar_assassin_meld":
                                if (siege)
                                {
                                    var armor = new[] { 2, 4, 6, 8 };
                                    var bonusTemplar = new[] { 50, 100, 150, 200 };

                                    damage =
                                        killable.Unit.SpellDamageTaken(
                                            myAutoAttackDamage + bonusTemplar[ability.Level - 1],
                                            DamageType.Physical,
                                            hero,
                                            ability.Name,
                                            true,
                                            armor[ability.Level - 1]);
                                }
                                break;
                            case "clinkz_searing_arrows":
                                var bonusClinkz = new[] { 30, 40, 50, 60 };

                                var tempDamage =
                                    killable.Unit.SpellDamageTaken(
                                        myAutoAttackDamage + bonusClinkz[ability.Level - 1],
                                        DamageType.Physical,
                                        hero,
                                        ability.Name,
                                        true);
                                if (siege)
                                {
                                    tempDamage /= 2;
                                }
                                damage = tempDamage;

                                break;
                        }

                        creep.SaveDamage(ability.Handle, damage);
                    }

                    if (ability.DoesTowerDamage)
                    {
                        var tower = killable as Tower;
                        tower?.SaveDamage(
                            ability.Handle,
                            AbilityDamage.CalculateDamage(ability.Source, hero, tower.Unit)
                            / ability.TowerDamageReduction);
                    }
                }

                killable.DamageCalculated = true;
            }
        }

        #endregion
    }
}