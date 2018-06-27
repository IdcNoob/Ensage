namespace LaneMarker.Renderer
{
    using System;

    using Ensage.SDK.Renderer.DX9;

    using SharpDX;

    using Color = System.Drawing.Color;

    public sealed class SimpleD3D9Renderer : ISimpleRenderMode
    {
        private readonly ID3D9Context context;

        private readonly FontCache fontCache;

        private bool disposed;

        public SimpleD3D9Renderer(ID3D9Context context, FontCache fontCache)
        {
            this.context = context;
            this.fontCache = fontCache;
        }

        public event EventHandler Draw
        {
            add
            {
                this.context.Draw += value;
            }

            remove
            {
                this.context.Draw -= value;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DrawText(Vector2 position, string text, Color color, float fontSize = 13f, string fontFamily = "Calibri")
        {
            var font = this.fontCache.GetOrCreate(fontFamily, fontSize);
            font.DrawText(null, text, (int)position.X, (int)position.Y, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.context.Dispose();
            }

            this.disposed = true;
        }
    }
}