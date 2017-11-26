namespace Debugger.Tools.Cheats
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;

    using Menu;

    public class CreateUnits : IDebuggerTool
    {
        private readonly Random random = new Random();

        [Import]
        private IMenu mainMenu;

        private MenuItem<KeyBind> meleeAllyCreep;

        private MenuItem<KeyBind> meleeEnemyCreep;

        private MenuFactory menu;

        private MenuItem<KeyBind> randomAlly;

        private MenuItem<KeyBind> randomEnemy;

        private MenuItem<KeyBind> rangedAllyCreep;

        private MenuItem<KeyBind> rangedEnemyCreep;

        public int LoadPriority { get; } = 5;

        public void Activate()
        {
            this.menu = this.mainMenu.CheatsMenu.Menu("Create unit");

            this.randomAlly = this.menu.Item("Random ally hero", new KeyBind(103));
            this.randomAlly.PropertyChanged += this.RandomAllyOnPropertyChanged;

            this.meleeAllyCreep = this.menu.Item("Melee ally creep", new KeyBind(104));
            this.meleeAllyCreep.PropertyChanged += this.MeleeAllyCreepOnPropertyChanged;

            this.rangedAllyCreep = this.menu.Item("Ranged ally creep", new KeyBind(105));
            this.rangedAllyCreep.PropertyChanged += this.RangedAllyCreepOnPropertyChanged;

            this.randomEnemy = this.menu.Item("Random enemy hero", new KeyBind(100));
            this.randomEnemy.PropertyChanged += this.RandomEnemyOnPropertyChanged;

            this.meleeEnemyCreep = this.menu.Item("Melee enemy creep", new KeyBind(101));
            this.meleeEnemyCreep.PropertyChanged += this.MeleeEnemyCreepOnPropertyChanged;

            this.rangedEnemyCreep = this.menu.Item("Ranged enemy creep", new KeyBind(102));
            this.rangedEnemyCreep.PropertyChanged += this.RangedEnemyCreepOnPropertyChanged;
        }

        public void Dispose()
        {
            this.randomAlly.PropertyChanged -= this.RandomAllyOnPropertyChanged;
            this.meleeAllyCreep.PropertyChanged -= this.MeleeAllyCreepOnPropertyChanged;
            this.rangedAllyCreep.PropertyChanged -= this.RangedAllyCreepOnPropertyChanged;
            this.randomEnemy.PropertyChanged -= this.RandomEnemyOnPropertyChanged;
            this.meleeEnemyCreep.PropertyChanged -= this.MeleeEnemyCreepOnPropertyChanged;
            this.rangedEnemyCreep.PropertyChanged -= this.RangedEnemyCreepOnPropertyChanged;
        }

        private string GetRandomHero()
        {
            var alreadyAdded = EntityManager<Hero>.Entities.Select(x => x.HeroId);
            var heroes = Enum.GetValues(typeof(HeroId)).Cast<HeroId>().Except(alreadyAdded).ToList();
            var randomHero = heroes[this.random.Next(1, heroes.Count - 1)];

            return randomHero.ToString();
        }

        private void MeleeAllyCreepOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.meleeAllyCreep)
            {
                Game.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_melee");
            }
        }

        private void MeleeEnemyCreepOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.meleeEnemyCreep)
            {
                Game.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_melee enemy");
            }
        }

        private void RandomAllyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.randomAlly)
            {
                Game.ExecuteCommand("dota_create_unit " + this.GetRandomHero());
            }
        }

        private void RandomEnemyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.randomEnemy)
            {
                Game.ExecuteCommand("dota_create_unit " + this.GetRandomHero() + " enemy");
            }
        }

        private void RangedAllyCreepOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.rangedAllyCreep)
            {
                Game.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_ranged");
            }
        }

        private void RangedEnemyCreepOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.rangedEnemyCreep)
            {
                Game.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_ranged enemy");
            }
        }
    }
}