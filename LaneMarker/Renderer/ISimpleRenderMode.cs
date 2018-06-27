namespace LaneMarker.Renderer
{
    using System;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal interface ISimpleRenderMode : IDisposable
    {
        event EventHandler Draw;

        void DrawText(Vector2 position, string text, Color color, float fontSize = 13f, string fontFamily = "Calibri");
    }
}