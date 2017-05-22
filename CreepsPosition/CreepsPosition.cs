namespace CreepsPosition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using SharpDX;
    using SharpDX.Direct3D9;

    [ExportPlugin("Creeps Position", StartupMode.Auto, "IdcNoob")]
    internal class CreepsPosition : Plugin
    {
        private readonly List<Creep> creeps = new List<Creep>();

        private readonly Team myTeam;

        private readonly Font textFont;

        private Config config;

        [ImportingConstructor]
        public CreepsPosition([Import] IServiceContext context)
        {
            myTeam = context.Owner.Team;

            if (Drawing.RenderMode == RenderMode.Dx9)
            {
                textFont = new Font(
                    Drawing.Direct3DDevice9,
                    new FontDescription
                    {
                        FaceName = "Tahoma",
                        Height = 22
                    });
            }

            var cheats = Game.GetConsoleVar("sv_cheats");
            cheats.RemoveFlags(ConVarFlags.Cheat);
            cheats.SetValue(1);
        }

        protected override void OnActivate()
        {
            config = new Config();
            config.Key.Item.ValueChanged += KeyPressed;
            Drawing.OnPreReset += OnPreReset;
            Drawing.OnPostReset += OnPostReset;
        }

        protected override void OnDeactivate()
        {
            config.Key.Item.ValueChanged -= KeyPressed;
            Drawing.OnPreReset -= OnPreReset;
            Drawing.OnPostReset -= OnPostReset;
            Drawing.OnEndScene -= OnEndScene;
            Drawing.OnDraw -= OnDraw;
            ObjectManager.OnAddEntity -= OnAddEntity;
            config.Dispose();
            creeps.Clear();
        }

        private void End()
        {
            config.Key.Item.ValueChanged += KeyPressed;
            ObjectManager.OnAddEntity -= OnAddEntity;
            Drawing.OnEndScene -= OnEndScene;
            Drawing.OnDraw -= OnDraw;
        }

        private void KeyPressed(object sender, OnValueChangeEventArgs onValueChangeEventArgs)
        {
            if (onValueChangeEventArgs.GetNewValue<KeyBind>())
            {
                creeps.Clear();
                config.Key.Item.ValueChanged -= KeyPressed;
                Game.ExecuteCommand("cl_fullupdate");
                ObjectManager.OnAddEntity += OnAddEntity;
                Drawing.OnEndScene += OnEndScene;
                Drawing.OnDraw += OnDraw;
                UpdateManager.BeginInvoke(End, 1000 + config.TimeToShow);
            }
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            var creep = args.Entity as Creep;
            if (creep == null || !creep.IsValid || !creep.IsSpawned || creep.Team == myTeam || creeps.Contains(creep)
                || config.ShowOnlyNeutrals && !creep.IsNeutral)
            {
                return;
            }

            creeps.Add(creep);
        }

        private void OnDraw(EventArgs args)
        {
            // map draw
            foreach (var creep in creeps.Where(x => x.IsValid && !x.IsVisible))
            {
                var position = Drawing.WorldToScreen(creep.Position);
                if (!position.IsZero)
                {
                    Drawing.DrawText(
                        "^",
                        "Tahoma",
                        position,
                        new Vector2(30),
                        creep.IsAlive ? Color.LightGreen : Color.Orange,
                        FontFlags.None);
                }
            }
        }

        private void OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null)
            {
                return;
            }

            try
            {
                // minimap draw
                var validCreeps = creeps.Where(x => x.IsValid && !x.IsVisible).ToList();

                foreach (var stack in validCreeps.GroupBy(
                    x => validCreeps.Where(z => z.Distance2D(x) < 750)
                        .Aggregate(new Vector3(), (sum, creep) => sum + creep.Position)))
                {
                    var position = (stack.Key / stack.Count()).WorldToMinimap() - new Vector2(5, 10);
                    textFont.DrawText(
                        null,
                        stack.Count().ToString(),
                        (int)position.X,
                        (int)position.Y,
                        stack.All(x => x.IsAlive) ? Color.LightGreen : Color.Orange);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void OnPostReset(EventArgs args)
        {
            textFont.OnResetDevice();
        }

        private void OnPreReset(EventArgs args)
        {
            textFont.OnLostDevice();
        }
    }
}