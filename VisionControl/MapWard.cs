using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Objects;
using SharpDX;

namespace VisionControl {
    internal class MapWard {
        public static readonly List<MapWard> MapWards = new List<MapWard>();

        private MapWard(ClassID ward, Entity hero) {
            var texture = "materials/ensage_ui/items/";

            switch (ward) {
                case ClassID.CDOTA_NPC_Observer_Ward: {
                    EndTime = Game.GameTime + 420;
                    texture += "ward_observer.vmat";
                    break;
                }
                case ClassID.CDOTA_NPC_Observer_Ward_TrueSight: {
                    EndTime = Game.GameTime + 240;
                    texture += "ward_sentry.vmat";
                    break;
                }
            }

            WardType = ward;
            Show = true;
            Texture = Drawing.GetTexture(texture);
            Position = new Vector3(hero.Position.X + 300 * (float) Math.Cos(hero.RotationRad),
                hero.Position.Y + 300 * (float) Math.Sin(hero.RotationRad),
                hero.Position.Z);
            DrawRange();

            if (!Program.Menu.Item("notification").GetValue<bool>())
                return;

            var notification = new SideMessage("VC.enemyPlacedWard", new Vector2(226, 59));

            notification.AddElement(new Vector2(9, 9), new Vector2(73, 41),
                Drawing.GetTexture("materials/ensage_ui/heroes_horizontal/" +
                                   hero.StoredName().Substring("npc_dota_hero_".Length) + ".vmat"));
            notification.AddElement(new Vector2(97, 2), new Vector2(50, 50),
                Drawing.GetTexture("materials/ensage_ui/other/arrow_usual_left.vmat"));
            notification.AddElement(new Vector2(163, 9), new Vector2(75, 41), Texture);

            notification.CreateMessage();
        }

        private MapWard(Entity ward) {
            var texture = "materials/ensage_ui/items/";

            switch (ward.ClassID) {
                case ClassID.CDOTA_NPC_Observer_Ward: {
                    EndTime = Game.GameTime + 360;
                    texture += "ward_observer.vmat";
                    break;
                }
                case ClassID.CDOTA_NPC_Observer_Ward_TrueSight: {
                    EndTime = Game.GameTime + 240;
                    texture += "ward_sentry.vmat";
                    break;
                }
            }

            WardType = ward.ClassID;
            IsKnown = true;
            Ward = ward;
            Show = true;
            Texture = Drawing.GetTexture(texture);
            Position = ward.Position;
            DrawRange();
        }

        public DotaTexture Texture { get; private set; }

        private ParticleEffect ParticleEffect { get; set; }

        private bool IsKnown { get; set; }

        public Vector3 Position { get; private set; }

        private Entity Ward { get; set; }

        private ClassID WardType { get; set; }

        public float EndTime { get; private set; }

        public bool Show { get; private set; }

        public static void ChangeColor(ClassID wardID, Color color, int value) {
            foreach (var mapWard in MapWards.Where(x => x.ParticleEffect != null && x.WardType == wardID)) {
                var sentry = "";

                if (wardID == ClassID.CDOTA_NPC_Observer_Ward_TrueSight)
                    sentry = "S";

                mapWard.ParticleEffect.SetControlPoint(1, new Vector3(
                    color == Color.Red ? value : Program.Menu.Item("red" + sentry).GetValue<Slider>().Value,
                    color == Color.Green ? value : Program.Menu.Item("green" + sentry).GetValue<Slider>().Value,
                    color == Color.Blue ? value : Program.Menu.Item("blue" + sentry).GetValue<Slider>().Value));
            }
        }

        private void DrawRange(bool update = false, bool force = false) {
            if (!Program.Menu.Item("enabledRanges").GetValue<bool>() && !force)
                return;

            if (update)
                ParticleEffect.Dispose();

            ParticleEffect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", Position);

            var range = 1790;
            var red = "red";
            var green = "green";
            var blue = "blue";

            if (WardType == ClassID.CDOTA_NPC_Observer_Ward_TrueSight) {
                range = 950;
                red += "S";
                green += "S";
                blue += "S";
            }

            ParticleEffect.SetControlPoint(1, new Vector3(
                Program.GetMenuSliderValue(red),
                Program.GetMenuSliderValue(green),
                Program.GetMenuSliderValue(blue)));

            ParticleEffect.SetControlPoint(2, new Vector3(range * -1, 255, 0));
        }

        public static void Clear() {
            foreach (var mapWard in MapWards.Where(x => x.ParticleEffect != null))
                mapWard.ParticleEffect.Dispose();
            MapWards.Clear();
        }

        private static void ClearParticles() {
            foreach (var mapWard in MapWards.Where(x => x.ParticleEffect != null))
                mapWard.ParticleEffect.Dispose();
        }

        private static void DrawParticles(bool force) {
            foreach (var mapWard in MapWards)
                mapWard.DrawRange(force: force);
        }

        public static void ChangeParticles(bool enabled, bool force = false) {
            if (enabled) DrawParticles(force);
            else ClearParticles();
        }

        public static void Add(Unit hero, ClassID wardID) {
            MapWards.Add(new MapWard(wardID, hero));
        }

        public static void ResetIconHide(bool enabled) {
            if (enabled) SmartHideEnable();
            else SmartHideDisable();
        }

        private static void SmartHideEnable() {
            foreach (var ward in MapWards.Where(x => x.Show && x.Ward != null && x.Ward.IsVisible))
                ward.Show = false;
        }

        private static void SmartHideDisable() {
            foreach (var ward in MapWards.Where(x => !x.Show))
                ward.Show = true;
        }

        public static void Update() {
            var enemyWards =
                ObjectManager.GetEntities<Entity>()
                    .Where(
                        x =>
                            x.IsAlive && x.IsVisible && x.Team == Program.Hero.GetEnemyTeam() &&
                            (x.ClassID == ClassID.CDOTA_NPC_Observer_Ward ||
                             x.ClassID == ClassID.CDOTA_NPC_Observer_Ward_TrueSight));

            foreach (var ward in enemyWards) {
                if (MapWards.Any(x => x.Ward != null && x.Ward.Equals(ward)))
                    continue;

                var unknownWard =
                    MapWards.Where(x => !x.IsKnown && x.Position.Distance2D(ward) <= 500 && x.WardType == ward.ClassID)
                        .OrderBy(x => x.Position.Distance2D(ward))
                        .FirstOrDefault();

                if (unknownWard == null) {
                    MapWards.Add(new MapWard(ward));
                    continue;
                }

                unknownWard.Position = ward.Position;
                unknownWard.Ward = ward;
                unknownWard.IsKnown = true;
                unknownWard.DrawRange(true);
            }

            if (Program.Menu.Item("hide").GetValue<bool>())
                foreach (var ward in MapWards.Where(x => x.IsKnown))
                    ward.Show = !ward.Ward.IsVisible;

            var removeWard =
                MapWards.FirstOrDefault(
                    x => (x.IsKnown && x.Ward != null && !x.Ward.IsAlive) || Game.GameTime > x.EndTime);

            if (removeWard != null) {
                removeWard.ParticleEffect.Dispose();
                MapWards.Remove(removeWard);
            }
        }
    }
}