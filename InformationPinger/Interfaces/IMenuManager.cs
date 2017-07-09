namespace InformationPinger.Interfaces
{
    using System;

    using Ensage.SDK.Menu;

    using PlaySharp.Toolkit.Helper;

    internal interface IMenuManager : IDisposable, IActivatable
    {
        MenuItem<bool> Enabled { get; }

        MenuFactory MenuFactory { get; }
    }
}