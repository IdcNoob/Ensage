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

        private MapWard(Entity hero, ClassID wardID) {
            var texture = "materials/ensage_ui/items/";

            switch (wardID) {
                case ClassID.CDOTA_NPC_Observer_Ward: {
                    EndTime = Game.GameTime + 360;
                    var time = TimeSpan.FromSeconds(EndTime - Game.GameTime);
                    TimeLeftString = time.ToString(@"m\:ss");
                    texture += "ward_observer.vmat";
                    break;
                }
                case ClassID.CDOTA_NPC_Observer_Ward_TrueSight: {
                    EndTime = Game.GameTime + 240;
                    var time = TimeSpan.FromSeconds(EndTime - Game.GameTime);
                    TimeLeftString = time.ToString(@"m\:ss");
                    texture += "ward_sentry.vmat";
                    break;
                }
            }

            WardType = wardID;
            Show = true;
            Texture = Drawing.GetTexture(texture);
            Position = new Vector3(hero.Position.X + 300 * (float) Math.Cos(hero.RotationRad),
                hero.Position.Y + 300 * (float) Math.Sin(hero.RotationRad),
                hero.Position.Z);
            MinimapPosition = WorldToMiniMap(Position, Program.Menu.Item("minimapSize").GetValue<Slider>().Value);
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

        private MapWard(Unit ward) {
            var texture = "materials/ensage_ui/items/";

            if (ward.ClassID == ClassID.CDOTA_NPC_Observer_Ward)
                texture += "ward_observer.vmat";
            else
                texture += "ward_sentry.vmat";

            var wardModifier = ward.FindModifier("modifier_item_buff_ward");

            if (wardModifier != null) {
                EndTime = Game.GameTime + wardModifier.RemainingTime;
                var time = TimeSpan.FromSeconds(EndTime - Game.GameTime);
                TimeLeftString = time.ToString(@"m\:ss");
            }
            else {
                TimeLeftString = "unkown";
            }


            WardType = ward.ClassID;
            IsKnown = true;
            Ward = ward;
            Show = true;
            Texture = Drawing.GetTexture(texture);
            Position = ward.Position;
            MinimapPosition = WorldToMiniMap(Position, Program.Menu.Item("minimapSize").GetValue<Slider>().Value);
            DrawRange();
        }

        public DotaTexture Texture { get; private set; }

        private ParticleEffect ParticleEffect { get; set; }

        private bool IsKnown { get; set; }

        public Vector3 Position { get; private set; }

        public Vector2 MinimapPosition { get; private set; }

        private Entity Ward { get; set; }

        public ClassID WardType { get; set; }

        public float EndTime { get; private set; }

        public string TimeLeftString { get; private set; }

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

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);

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
            MapWards.Add(new MapWard(hero, wardID));
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

        public static void ChangeMinimapWardPosition(int value) {
            foreach (var ward in MapWards) {
                ward.MinimapPosition = WorldToMiniMap(ward.Position, value);
            }
        }

        public static void Update() {
            var enemyWards =
                ObjectManager.GetEntities<Unit>()
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
                unknownWard.MinimapPosition = WorldToMiniMap(unknownWard.Position,
                    Program.Menu.Item("minimapSize").GetValue<Slider>().Value);
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

            if (!Utils.SleepCheck("VisionControl.UpdateTime"))
                return;

            foreach (var ward in MapWards) {
                var time = TimeSpan.FromSeconds(ward.EndTime - Game.GameTime);
                ward.TimeLeftString = time.ToString(@"m\:ss");
            }

            Utils.Sleep(1000, "VisionControl.UpdateTime");
        }

        private static Vector2 WorldToMiniMap(Vector3 Pos, int size) {
            const float MapLeft = -8000;
            const float MapTop = 7350;
            const float MapRight = 7500;
            const float MapBottom = -7200;
            var MapWidth = Math.Abs(MapLeft - MapRight);
            var MapHeight = Math.Abs(MapBottom - MapTop);

            var _x = Pos.X - MapLeft;
            var _y = Pos.Y - MapBottom;

            float dx, dy, px, py;
            if (Math.Round((float) Drawing.Width / Drawing.Height, 1) >= 1.7) {
                dx = 272f / 1920f * Drawing.Width;
                dy = 261f / 1080f * Drawing.Height;
                px = 11f / 1920f * Drawing.Width;
                py = 11f / 1080f * Drawing.Height;
            }
            else if (Math.Round((float) Drawing.Width / Drawing.Height, 1) >= 1.5) {
                dx = 267f / 1680f * Drawing.Width;
                dy = 252f / 1050f * Drawing.Height;
                px = 10f / 1680f * Drawing.Width;
                py = 11f / 1050f * Drawing.Height;
            }
            else {
                dx = 255f / 1280f * Drawing.Width;
                dy = 229f / 1024f * Drawing.Height;
                px = 6f / 1280f * Drawing.Width;
                py = 9f / 1024f * Drawing.Height;
            }
            var MinimapMapScaleX = dx / MapWidth;
            var MinimapMapScaleY = dy / MapHeight;

            var scaledX = Math.Min(Math.Max(_x * MinimapMapScaleX, 0), dx);
            var scaledY = Math.Min(Math.Max(_y * MinimapMapScaleY, 0), dy);

            var screenX = px + scaledX;
            var screenY = Drawing.Height - scaledY - py;

            return new Vector2((float) Math.Floor(screenX - size * 1.8), (float) Math.Floor(screenY - size * 2.7));
        }
    }
}
