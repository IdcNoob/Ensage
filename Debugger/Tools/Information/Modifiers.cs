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

    internal class Modifiers : IDebuggerTool
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

        private MenuItem<bool> showElapsedTime;

        private MenuItem<bool> showHidden;

        private MenuItem<bool> showRemainingTime;

        private MenuItem<bool> showTextureName;

        [ImportingConstructor]
        public Modifiers()
        {
            this.player = ObjectManager.LocalPlayer;
        }

        public int LoadPriority { get; } = 79;

        public void Activate()
        {
            this.menu = this.mainMenu.InformationMenu.Menu("Modifiers");

            this.information = this.menu.Item("Get", false);
            this.information.PropertyChanged += this.InformationOnPropertyChanged;

            this.autoUpdate = this.menu.Item("Auto update", false);
            this.autoUpdate.PropertyChanged += this.AutoUpdateOnPropertyChanged;

            this.showHidden = this.menu.Item("Show hidden", false);
            this.showTextureName = this.menu.Item("Show texture name", false);
            this.showRemainingTime = this.menu.Item("Show remaining time", false);
            this.showElapsedTime = this.menu.Item("Show elapsed time", false);

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

            var item = new LogItem(LogType.Modifier, Color.PaleGreen, "Modifiers information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);

            foreach (var modifier in unit.Modifiers)
            {
                if (!this.showHidden && modifier.IsHidden)
                {
                    continue;
                }

                var modifierItem = new LogItem(LogType.Modifier, Color.PaleGreen);

                modifierItem.AddLine("Name: " + modifier.Name, modifier.Name);

                if (this.showTextureName)
                {
                    modifierItem.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
                }

                if (this.showHidden)
                {
                    modifierItem.AddLine("Is hidden: " + modifier.IsHidden, modifier.IsHidden);
                }

                if (this.showElapsedTime)
                {
                    modifierItem.AddLine("Elapsed time: " + modifier.ElapsedTime, modifier.ElapsedTime);
                }

                if (this.showRemainingTime)
                {
                    modifierItem.AddLine("Remaining time: " + modifier.RemainingTime, modifier.RemainingTime);
                }

                this.log.Display(modifierItem);
            }

            this.log.Display(item);
        }
    }
}