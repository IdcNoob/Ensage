namespace PredictedCreepsLocation.Core
{
    using System;
    using System.Collections.Generic;

    using Creeps;

    using PlaySharp.Toolkit.Helper;

    internal interface IWaveManager : IDisposable, IActivatable
    {
        List<CreepWave> CreepWaves { get; }
    }
}