namespace LaneMarker.Renderer
{
    using System;

    using Ensage.SDK.Renderer.DX11;

    using SharpDX;
    using SharpDX.DirectWrite;

    using Color = System.Drawing.Color;

    public sealed class SimpleD3D11Renderer : ISimpleRenderMode
    {
        private readonly BrushCache brushCache;

        private readonly ID3D11Context context;

        private readonly TextFormatCache textFormatCache;

        private bool disposed;

        public SimpleD3D11Renderer(ID3D11Context context, BrushCache brushCache, TextFormatCache textFormatCache)
        {
            this.context = context;

            this.brushCache = brushCache;
            this.textFormatCache = textFormatCache;
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
            var font = this.textFormatCache.GetOrCreate(fontFamily, fontSize);
            var brush = this.brushCache.GetOrCreate(color);

            using (var layout = new TextLayout(this.context.DirectWrite, text, font, float.MaxValue, float.MaxValue))
            {
                this.context.RenderTarget.DrawTextLayout(position, layout, brush);
            }
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