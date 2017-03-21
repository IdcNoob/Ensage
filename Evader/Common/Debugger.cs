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
            Information,

            AbilityUsage,

            Intersectons
        }

        #endregion

        #region Properties

        private static DebugMenu Menu => Variables.Menu.Debug;

        #endregion

        #region Public Methods and Operators

        public static void DeleteGreenCircle()
        {
            greenCirclePartcile?.Dispose();
            greenCirclePartcile = null;
        }

        public static void DeleteRedCircle()
        {
            redCirclePartcile?.Dispose();
            redCirclePartcile = null;
        }

        public static void DrawGreenCircle(Vector3 position, bool force = false)
        {
            if (!Menu.DrawMap && !force)
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

        public static void DrawRedCircle(Vector3 position, bool force = false)
        {
            if (!Menu.DrawMap && !force)
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

        public static void Write(string text = "", Type type = Type.Information, bool showType = true)
        {
            WriteLine(text, type, false, showType);
        }

        public static void WriteLine(
            string text = "",
            Type type = Type.Information,
            bool newLine = true,
            bool showType = true)
        {
            switch (type)
            {
                case Type.Information:
                    if (!Menu.LogInformation)
                    {
                        return;
                    }
                    if (showType)
                    {
                        text = "[Evader][Info] " + text;
                    }
                    break;
                case Type.AbilityUsage:
                    if (!Menu.LogAbilityUsage)
                    {
                        return;
                    }
                    if (showType)
                    {
                        text = "[Evader][Abilities] " + text;
                    }
                    break;
                case Type.Intersectons:
                    if (!Menu.LogIntersection)
                    {
                        return;
                    }
                    if (showType)
                    {
                        text = "[Evader][Intersectons] " + text;
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