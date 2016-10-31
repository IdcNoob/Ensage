namespace Evader.Common
{
    using System;

    using Core;

    internal static class Debugger
    {
        #region Enums

        public enum Type
        {
            Random,

            Particles,

            Modifiers,

            Units,

            Projectiles,

            Intersectons
        }

        #endregion

        #region Public Methods and Operators

        public static void Write(string text = "", Type type = Type.Random)
        {
            WriteLine(text, type, false);
        }

        public static void WriteLine(string text = "", Type type = Type.Random, bool newLine = true)
        {
            switch (type)
            {
                case Type.Random:
                    if (!Variables.Menu.Debug.LogRandom)
                    {
                        return;
                    }
                    break;
                case Type.Particles:
                    if (!Variables.Menu.Debug.LogParticles)
                    {
                        return;
                    }
                    break;
                case Type.Modifiers:
                    if (!Variables.Menu.Debug.LogModifiers)
                    {
                        return;
                    }
                    break;
                case Type.Units:
                    if (!Variables.Menu.Debug.LogUnits)
                    {
                        return;
                    }
                    break;
                case Type.Projectiles:
                    if (!Variables.Menu.Debug.LogProjectiles)
                    {
                        return;
                    }
                    break;
                case Type.Intersectons:
                    if (!Variables.Menu.Debug.LogIntersection)
                    {
                        return;
                    }
                    break;
            }

            if (newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
        }

        #endregion
    }
}