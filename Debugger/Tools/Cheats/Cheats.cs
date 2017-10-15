namespace Debugger.Tools.Cheats
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    using Menu;

    internal class Cheats : IDebuggerTool
    {
        private bool allVisionEnabled;

        private MenuItem<KeyBind> bot25Lvl;

        private MenuItem<KeyBind> creeps;

        private bool creepsEnabled;

        private MenuItem<KeyBind> hero25Lvl;

        private MenuItem<KeyBind> heroGold;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private MenuItem<KeyBind> refresh;

        private MenuItem<KeyBind> vision;

        private MenuItem<KeyBind> wtf;

        private bool wtfEnabled;

        public int LoadPriority { get; } = 5;

        public void Activate()
        {
            this.menu = this.mainMenu.CheatsMenu;

            this.refresh = this.menu.Item("Refresh", new KeyBind(99));
            this.refresh.PropertyChanged += this.RefreshOnPropertyChanged;

            this.wtf = this.menu.Item("Change wtf", new KeyBind(111));
            this.wtf.PropertyChanged += this.WtfOnPropertyChanged;

            this.vision = this.menu.Item("Change vision", new KeyBind(106));
            this.vision.PropertyChanged += this.VisionOnPropertyChanged;
            this.allVisionEnabled = Game.GetConsoleVar("dota_all_vision").GetInt() == 1;

            this.creeps = this.menu.Item("Change creeps spawn", new KeyBind(96));
            this.creeps.PropertyChanged += this.CreepsOnPropertyChanged;

            this.hero25Lvl = this.menu.Item("Hero 25lvl", new KeyBind(97));
            this.hero25Lvl.PropertyChanged += this.Hero25LvlOnPropertyChanged;

            this.heroGold = this.menu.Item("Hero gold", new KeyBind(97));
            this.heroGold.PropertyChanged += this.HeroGoldOnPropertyChanged;

            this.bot25Lvl = this.menu.Item("Bot 25lvl", new KeyBind(98));
            this.bot25Lvl.PropertyChanged += this.Bot25LvlOnPropertyChanged;
        }

        public void Dispose()
        {
            this.refresh.PropertyChanged -= this.RefreshOnPropertyChanged;
            this.wtf.PropertyChanged -= this.WtfOnPropertyChanged;
            this.vision.PropertyChanged -= this.VisionOnPropertyChanged;
            this.creeps.PropertyChanged -= this.CreepsOnPropertyChanged;
            this.hero25Lvl.PropertyChanged -= this.Hero25LvlOnPropertyChanged;
            this.heroGold.PropertyChanged -= this.HeroGoldOnPropertyChanged;
            this.bot25Lvl.PropertyChanged -= this.Bot25LvlOnPropertyChanged;
        }

        private void Bot25LvlOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.bot25Lvl)
            {
                Game.ExecuteCommand("dota_bot_give_level 25");
            }
        }

        private void CreepsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.creeps)
            {
                if (this.creepsEnabled)
                {
                    Game.ExecuteCommand("dota_creeps_no_spawning_disable");
                    this.creepsEnabled = false;
                }
                else
                {
                    Game.ExecuteCommand("dota_creeps_no_spawning_enable");
                    this.creepsEnabled = true;
                }
            }
        }

        private void Hero25LvlOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.hero25Lvl)
            {
                Game.ExecuteCommand("dota_hero_level 25");
            }
        }

        private void HeroGoldOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.heroGold)
            {
                Game.ExecuteCommand("dota_give_gold 99999");
            }
        }

        private void RefreshOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.refresh)
            {
                Game.ExecuteCommand("dota_hero_refresh");
            }
        }

        private void VisionOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.vision)
            {
                if (this.allVisionEnabled)
                {
                    Game.ExecuteCommand("dota_all_vision_disable");
                    this.allVisionEnabled = false;
                }
                else
                {
                    Game.ExecuteCommand("dota_all_vision_enable");
                    this.allVisionEnabled = true;
                }
            }
        }

        private void WtfOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.wtf)
            {
                if (this.wtfEnabled)
                {
                    Game.ExecuteCommand("dota_ability_debug_disable");
                    this.wtfEnabled = false;
                }
                else
                {
                    Game.ExecuteCommand("dota_ability_debug_enable");
                    this.wtfEnabled = true;
                }
            }
        }
    }
}