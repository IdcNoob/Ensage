namespace BodyBlocker.Modes
{
    using System;
    using System.ComponentModel.Composition;

    [InheritedExport]
    internal interface IBodyBlockMode : IDisposable
    {
        void Activate();
    }
}