using System;

namespace Sockethead.Razor.Utilities
{
    internal class Scope : IDisposable
    {
        private Action OnDispose { get; }

        public Scope(Action onBegin, Action onEnd)
        {
            onBegin();
            OnDispose = onEnd;
        }

        public void Dispose() => OnDispose();
    }
}