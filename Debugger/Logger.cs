namespace Debugger
{
    using System;

    using Ensage;

    internal class Logger
    {
        public enum Type
        {
            Modifier,

            Partcile,

            Unit,

            Ability,

            Spell,

            Item,

            Projectile,

            Animation,

            Bool,

            Float,

            Handle,

            Int32,

            Int64,

            String,

            ExecuteOrder
        }

        public void EmptyLine()
        {
            Console.WriteLine();
        }

        public void Write(string text, Type type, ConsoleColor color, bool firstLine = false)
        {
            Console.ForegroundColor = color;
            Console.Write("[" + type + "] ");

            if (firstLine)
            {
                Console.Write(
                    Game.RawGameTime + " (" + TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + ") // ");
            }
            else
            {
                Console.ResetColor();
            }

            Console.WriteLine(text);
        }
    }
}