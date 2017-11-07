namespace Debugger.Tools.Information
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Units : IDebuggerTool
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

        private MenuItem<bool> showItemInfo;

        private MenuItem<bool> showLevel;

        private MenuItem<bool> showModifierInfo;

        private MenuItem<bool> showsAbilityInfo;

        private MenuItem<bool> showsHpMp;

        private MenuItem<bool> showState;

        private MenuItem<bool> showTeam;

        private MenuItem<bool> showVision;

        [ImportingConstructor]
        public Units()
        {
            this.player = ObjectManager.LocalPlayer;
        }

        public int LoadPriority { get; } = 78;

        public void Activate()
        {
            this.menu = this.mainMenu.InformationMenu.Menu("Units");

            this.information = this.menu.Item("Get", false);
            this.information.PropertyChanged += this.InformationOnPropertyChanged;

            this.autoUpdate = this.menu.Item("Auto update", false);
            this.autoUpdate.PropertyChanged += this.AutoUpdateOnPropertyChanged;

            this.showTeam = this.menu.Item("Show team", true);
            this.showLevel = this.menu.Item("Show level", true);
            this.showsHpMp = this.menu.Item("Show hp/mp", true);
            this.showVision = this.menu.Item("Show vision", true);
            this.showState = this.menu.Item("Show state", true);
            this.showsAbilityInfo = this.menu.Item("Show ability information", false);
            this.showItemInfo = this.menu.Item("Show item information", false);
            this.showModifierInfo = this.menu.Item("Show modifier information", false);

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

            var item = new LogItem(LogType.Unit, Color.PaleGreen, "Unit information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);
            item.AddLine("Unit type: " + unit.UnitType, unit.UnitType);
            item.AddLine("Unit position: " + unit.Position, unit.Position);
            if (this.showLevel)
            {
                item.AddLine("Unit level: " + unit.Level, unit.Level);
            }

            if (this.showTeam)
            {
                item.AddLine("Unit team: " + unit.Team, unit.Team);
            }

            if (this.showsHpMp)
            {
                item.AddLine("Unit health: " + unit.Health + "/" + unit.MaximumHealth);
                item.AddLine("Unit mana: " + (int)unit.Mana + "/" + (int)unit.MaximumMana);
            }

            item.AddLine("Unit attack capability: " + unit.AttackCapability, unit.AttackCapability);
            if (this.showVision)
            {
                item.AddLine("Unit vision: " + unit.DayVision + "/" + unit.NightVision);
            }

            if (this.showState)
            {
                item.AddLine("Unit state: " + unit.UnitState, unit.UnitState);
            }

            if (this.showsAbilityInfo)
            {
                item.AddLine("Abilities =>");
                item.AddLine("  Talents count: " + unit.Spellbook.Spells.Count(x => x.Name.StartsWith("special_")));
                item.AddLine(
                    "  Active spells count: " + unit.Spellbook.Spells.Count(
                        x => !x.Name.StartsWith("special_") && x.AbilityBehavior != AbilityBehavior.Passive));
                item.AddLine(
                    "  Passive spells count: " + unit.Spellbook.Spells.Count(
                        x => !x.Name.StartsWith("special_") && x.AbilityBehavior == AbilityBehavior.Passive));
            }

            if (this.showItemInfo && unit.HasInventory)
            {
                item.AddLine("Items =>");
                item.AddLine("  Inventory Items count: " + unit.Inventory.Items.Count());
                item.AddLine("  Backpack Items count: " + unit.Inventory.Backpack.Count());
                item.AddLine("  Stash Items count: " + unit.Inventory.Stash.Count());
            }

            if (this.showModifierInfo)
            {
                item.AddLine("Modifiers =>");
                item.AddLine("  Active modifiers count: " + unit.Modifiers.Count(x => !x.IsHidden));
                item.AddLine("  Hidden modifiers count: " + unit.Modifiers.Count(x => x.IsHidden));
            }

            this.log.Display(item);
        }
    }
}