using System;

namespace Sockethead.Razor.Forms
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