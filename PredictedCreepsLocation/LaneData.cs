namespace PredictedCreepsLocation
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class LaneData
    {
        public List<Vector3> Points = new List<Vector3>();

        public LanePosition LanePosition { get; set; }

        public Team Team { get; set; }
    }
}