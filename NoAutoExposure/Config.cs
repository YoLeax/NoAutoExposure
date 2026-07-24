using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NoAutoExposure
{
    internal class Config
    {
        internal event Action? StateChanged;

        public virtual bool Enabled { get; set; } = true;
        public virtual bool DisableToneMapping { get; set; }

        public virtual void Changed()
        {
            var handler = StateChanged;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
