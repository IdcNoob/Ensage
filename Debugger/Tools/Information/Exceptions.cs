namespace Debugger.Tools.Information
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Exceptions : IDebuggerTool
    {
        private TextWriter defaultOutput;

        private MenuItem<bool> enabled;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private StringWriter output;

        private IUpdateHandler updateHandler;

        public int LoadPriority { get; } = 1;

        public void Activate()
        {
            this.menu = this.mainMenu.InformationMenu.Menu("Exceptions");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.defaultOutput = Console.Out;

            this.updateHandler = UpdateManager.Subscribe(this.Check, 200, false);
            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            UpdateManager.Unsubscribe(this.Check);
            Console.SetOut(this.defaultOutput);
            this.output?.Dispose();
        }

        private void Check()
        {
            var text = this.output.ToString();
            if (!text.Any())
            {
                return;
            }

            try
            {
                var match = Regex.Match(text, @"(.*?exception)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var item = new LogItem(LogType.Exception, Color.Red, "Exception");
                    item.AddLine(match.Value.Split(' ').Last());
                    this.log.Display(item);

                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            finally
            {
                Console.SetOut(this.defaultOutput);
                Console.Write(text);
                Console.ResetColor();
                this.output.Dispose();
                Console.SetOut(this.output = new StringWriter());
            }
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                this.updateHandler.IsEnabled = true;
                this.output?.Dispose();
                Console.SetOut(this.output = new StringWriter());
            }
            else
            {
                this.menu.RemoveAsterisk();
                this.updateHandler.IsEnabled = false;
                Console.SetOut(this.defaultOutput);
            }
        }
    }
}