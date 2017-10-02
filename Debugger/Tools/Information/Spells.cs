namespace Debugger.Tools.Information
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Spells : IDebuggerTool
    {
        private readonly Player player;

        private MenuItem<bool> autoUpdate;

        private MenuItem<bool> information;

        private uint lastUnitInfo;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private MenuItem<bool> showBehavior;

        private MenuItem<bool> showCastRange;

        private MenuItem<bool> showHidden;

        private MenuItem<bool> showLevel;

        private MenuItem<bool> showManaCost;

        private MenuItem<bool> showSpecialData;

        private MenuItem<bool> showTalents;

        private MenuItem<bool> showTargetType;

        [ImportingConstructor]
        public Spells()
        {
            this.player = ObjectManager.LocalPlayer;
        }

        public int LoadPriority { get; } = 81;

        public void Activate()
        {
            this.menu = this.mainMenu.InformationMenu.Menu("Spells");

            this.information = this.menu.Item("Get", false);
            this.information.PropertyChanged += this.InformationOnPropertyChanged;

            this.autoUpdate = this.menu.Item("Auto update", false);
            this.autoUpdate.PropertyChanged += this.AutoUpdateOnPropertyChanged;

            this.showHidden = this.menu.Item("Show hidden", false);
            this.showTalents = this.menu.Item("Show talents", false);
            this.showLevel = this.menu.Item("Show levels", false);
            this.showManaCost = this.menu.Item("Show mana cost", false);
            this.showCastRange = this.menu.Item("Show cast range", false);
            this.showBehavior = this.menu.Item("Show behavior", false);
            this.showTargetType = this.menu.Item("Show target type", false);
            this.showSpecialData = this.menu.Item("Show all special data", false);

            this.AutoUpdateOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.information.PropertyChanged -= this.InformationOnPropertyChanged;
            this.autoUpdate.PropertyChanged -= this.AutoUpdateOnPropertyChanged;
            Game.OnFireEvent -= this.GameOnFireEvent;
        }

        private void AutoUpdateOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.autoUpdate)
            {
                this.menu.AddAsterisk();
                Game.OnFireEvent += this.GameOnFireEvent;
                this.InformationOnPropertyChanged(null, null);
            }
            else
            {
                this.menu.RemoveAsterisk();
                Game.OnFireEvent -= this.GameOnFireEvent;
            }
        }

        private void GameOnFireEvent(FireEventEventArgs args)
        {
            if (args.GameEvent.Name != "dota_player_update_selected_unit" && args.GameEvent.Name != "dota_player_update_query_unit")
            {
                return;
            }

            var unit = (this.player.QueryUnit ?? this.player.Selection.FirstOrDefault()) as Unit;
            if (unit?.IsValid != true)
            {
                return;
            }

            if (unit.Handle == this.lastUnitInfo)
            {
                return;
            }

            this.InformationOnPropertyChanged(null, null);
        }

        private void InformationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var unit = (this.player.QueryUnit ?? this.player.Selection.FirstOrDefault()) as Unit;
            if (unit?.IsValid != true)
            {
                return;
            }

            this.lastUnitInfo = unit.Handle;

            var item = new LogItem(LogType.Spell, "Spells information", Color.PaleGreen);

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);
            item.AddLine(string.Empty);

            foreach (var ability in unit.Spellbook.Spells)
            {
                if (!this.showHidden && ability.IsHidden)
                {
                    continue;
                }

                if (!this.showTalents && ability.Name.StartsWith("special_"))
                {
                    continue;
                }

                item.AddLine("Name: " + ability.Name, ability.Name);
                item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
                item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);

                if (this.showLevel)
                {
                    item.AddLine("Level: " + ability.Level, ability.Level);
                }

                if (this.showManaCost)
                {
                    item.AddLine("Mana cost: " + ability.ManaCost, ability.ManaCost);
                }

                if (this.showCastRange)
                {
                    item.AddLine("Cast range: " + ability.GetCastRange(), ability.GetCastRange());
                }

                if (this.showBehavior)
                {
                    item.AddLine("Behavior: " + ability.AbilityBehavior, ability.AbilityBehavior);
                }

                if (this.showTargetType)
                {
                    item.AddLine("Target type: " + ability.TargetType, ability.TargetType);
                    item.AddLine("Target team type: " + ability.TargetTeamType, ability.TargetTeamType);
                }

                if (this.showSpecialData)
                {
                    item.AddLine("Special data =>");
                    foreach (var abilitySpecialData in ability.AbilitySpecialData)
                    {
                        item.AddLine("  " + abilitySpecialData.Name + ": " + abilitySpecialData.Value, abilitySpecialData.Name);
                    }
                }

                item.AddLine(string.Empty);
            }

            this.log.Display(item);
        }
    }
}