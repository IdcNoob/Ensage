namespace Evader.Common
{
    using System;

    using Core;
    using Core.Menus;

    using Ensage;

    using SharpDX;

    internal static class Debugger
    {
        #region Static Fields

        private static ParticleEffect greenCirclePartcile;

        private static ParticleEffect redCirclePartcile;

        #endregion

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

        #region Properties

        private static DebugMenu menu => Variables.Menu.Debug;

        #endregion

        #region Public Methods and Operators

        public static void DrawGreenCircle(Vector3 position)
        {
            if (!menu.DrawMap)
            {
                return;
            }

            if (greenCirclePartcile == null)
            {
                greenCirclePartcile = new ParticleEffect(
                    @"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf",
                    position);
                greenCirclePartcile.SetControlPoint(1, new Vector3(0, 255, 0));
                greenCirclePartcile.SetControlPoint(2, new Vector3(50, 255, 0));
            }

            greenCirclePartcile.SetControlPoint(0, position);
        }

        public static void DrawRedCircle(Vector3 position)
        {
            if (!menu.DrawMap)
            {
                return;
            }

            if (redCirclePartcile == null)
            {
                redCirclePartcile = new ParticleEffect(
                    @"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf",
                    position);
                redCirclePartcile.SetControlPoint(1, new Vector3(255, 0, 0));
                redCirclePartcile.SetControlPoint(2, new Vector3(70, 255, 0));
            }

            redCirclePartcile.SetControlPoint(0, position);
        }

        public static void Write(string text = "", Type type = Type.Random)
        {
            WriteLine(text, type, false);
        }

        public static void WriteLine(string text = "", Type type = Type.Random, bool newLine = true)
        {
            switch (type)
            {
                case Type.Random:
                    if (!menu.LogRandom)
                    {
                        return;
                    }
                    break;
                case Type.Particles:
                    if (!menu.LogParticles)
                    {
                        return;
                    }
                    break;
                case Type.Modifiers:
                    if (!menu.LogModifiers)
                    {
                        return;
                    }
                    break;
                case Type.Units:
                    if (!menu.LogUnits)
                    {
                        return;
                    }
                    break;
                case Type.Projectiles:
                    if (!menu.LogProjectiles)
                    {
                        return;
                    }
                    break;
                case Type.Intersectons:
                    if (!menu.LogIntersection)
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