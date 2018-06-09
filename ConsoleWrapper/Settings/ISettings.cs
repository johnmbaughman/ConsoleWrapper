using System;

namespace ConsoleWrapper.Settings
{
    internal interface ISettings
    {
        bool IsLocked { get; }

        Guid Lock();
        void Unlock(Guid key);
        void SetValue<T>(T value, ref T container);
    }
}
