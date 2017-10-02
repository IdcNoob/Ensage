namespace Debugger.Tools
{
    using System;
    using System.ComponentModel.Composition;

    [InheritedExport]
    internal interface IDebuggerTool : IDisposable
    {
        int LoadPriority { get; }

        void Activate();
    }
}