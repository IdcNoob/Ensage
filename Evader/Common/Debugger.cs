namespace Evader.Common
{
    using System;
    using System.Collections.Generic;

    using Core;
    using Core.Menus;

    using Ensage;

    using SharpDX;

    internal static class Debugger
    {
        public enum Type
        {
            Information,

            AbilityUsage,

            Intersectons
        }

        private static readonly List<ParticleEffect> Particles = new List<ParticleEffect>();

        private static ParticleEffect greenCirclePartcile;

        private static ParticleEffect redCirclePartcile;

        private static DebugMenu Menu => Variables.Menu.Debug;

        public static void AddGreenCircle(Vector3 position, bool force = false)
        {
            if (!Menu.DrawMap && !force)
            {
                return;
            }

            var green = new ParticleEffect(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", position);
            green.SetControlPoint(1, new Vector3(0, 255, 0));
            green.SetControlPoint(2, new Vector3(50, 255, 0));

            Particles.Add(green);
        }

        public static void AddRedCircle(Vector3 position, bool force = false)
        {
            if (!Menu.DrawMap && !force)
            {
                return;
            }

            var red = new ParticleEffect(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", position);
            red.SetControlPoint(1, new Vector3(255, 0, 0));
            red.SetControlPoint(2, new Vector3(70, 255, 0));

            Particles.Add(red);
        }

        public static void ClearCircles()
        {
            foreach (var particleEffect in Particles)
            {
                particleEffect?.Dispose();
            }

            Particles.Clear();
        }

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
    }
}