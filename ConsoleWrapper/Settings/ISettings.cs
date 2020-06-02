using System;

namespace ConsoleWrapper.Settings
{
    internal interface ISettings
    {
        bool IsLocked { get; }

        void Lock();
        void Unlock();
        void SetProperty<T>(ref T container, T value);
    }
}
