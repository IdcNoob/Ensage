namespace Debugger.Logger
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    using SharpDX;

    internal class LogItem
    {
        public LogItem(LogType type, Color color, string firstLine = null)
        {
            if (firstLine != null)
            {
                this.FirstLine = firstLine + " // " + Game.RawGameTime + " (" + TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss")
                                 + ")";
            }

            this.Type = type;
            this.Color = color;
            this.Time = Game.RawGameTime;
        }

        public Color Color { get; }

        public string FirstLine { get; }

        public List<Tuple<string, string>> Lines { get; } = new List<Tuple<string, string>>();

        public float Time { get; }

        public LogType Type { get; }

        public void AddLine(string text, object param = null)
        {
            this.Lines.Add(Tuple.Create(text, param.ToCopyFormat()));
        }
    }
}