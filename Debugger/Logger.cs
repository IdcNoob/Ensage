namespace Debugger
{
    using System;

    using Ensage;

    internal class Logger
    {
        #region Enums

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
        }

        #endregion

        #region Public Methods and Operators

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
                Console.Write(TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " // ");
            }
            else
            {
                Console.ResetColor();
            }

            Console.WriteLine(text);
        }

        #endregion
    }
}