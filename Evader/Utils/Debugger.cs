namespace Evader.Utils
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

            Projectiles
        }

        #endregion

        #region Public Methods and Operators

        public static void Write(string text = "", Type type = Type.Random)
        {
            switch (type)
            {
                case Type.Random:
                    if (!Variables.Menu.DebugConsoleRandom)
                    {
                        return;
                    }
                    break;
                case Type.Particles:
                    if (!Variables.Menu.DebugConsoleParticles)
                    {
                        return;
                    }
                    break;
                case Type.Modifiers:
                    if (!Variables.Menu.DebugConsoleModifiers)
                    {
                        return;
                    }
                    break;
                case Type.Units:
                    if (!Variables.Menu.DebugConsoleUnits)
                    {
                        return;
                    }
                    break;
                case Type.Projectiles:
                    if (!Variables.Menu.DebugConsoleProjectiles)
                    {
                        return;
                    }
                    break;
            }

            Console.Write(text);
        }

        public static void WriteLine(string text = "", Type type = Type.Random)
        {
            switch (type)
            {
                case Type.Random:
                    if (!Variables.Menu.DebugConsoleRandom)
                    {
                        return;
                    }
                    break;
                case Type.Particles:
                    if (!Variables.Menu.DebugConsoleParticles)
                    {
                        return;
                    }
                    break;
                case Type.Modifiers:
                    if (!Variables.Menu.DebugConsoleModifiers)
                    {
                        return;
                    }
                    break;
                case Type.Units:
                    if (!Variables.Menu.DebugConsoleUnits)
                    {
                        return;
                    }
                    break;
                case Type.Projectiles:
                    if (!Variables.Menu.DebugConsoleProjectiles)
                    {
                        return;
                    }
                    break;
            }

            Console.WriteLine(text);
        }

        #endregion
    }
}