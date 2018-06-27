namespace LaneMarker.Renderer
{
    using System;

    using Ensage;
    using Ensage.SDK.Renderer.DX11;
    using Ensage.SDK.Renderer.DX9;

    using SharpDX;

    using Color = System.Drawing.Color;

    public sealed class SimpleRenderer
    {
        private readonly ISimpleRenderMode active;

        private bool disposed;

        public SimpleRenderer()
        {
            switch (Drawing.RenderMode)
            {
                case RenderMode.Dx9:
                {
                    var context = new D3D9Context();
                    var font = new FontCache(context);
                    this.active = new SimpleD3D9Renderer(context, font);
                    break;
                }
                case RenderMode.Dx11:
                {
                    var context = new D3D11Context();
                    var brush = new BrushCache(context);
                    var text = new TextFormatCache(context);
                    this.active = new SimpleD3D11Renderer(context, brush, text);
                    break;
                }
            }
        }

        public event EventHandler Draw
        {
            add
            {
                this.active.Draw += value;
            }
            remove
            {
                this.active.Draw -= value;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DrawText(Vector2 position, string text, Color color, float fontSize = 13f, string fontFamily = "Calibri")
        {
            this.active.DrawText(position, text, color, fontSize, fontFamily);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.active.Dispose();
            }

            this.disposed = true;
        }
    }
}