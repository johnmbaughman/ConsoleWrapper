using System;
using System.Collections.Generic;

namespace ConsoleWrapper.Settings
{
    public abstract class SettingsBase : ISettings
    {
        public bool IsLocked { get; protected set; }

        protected SettingsBase() => IsLocked = false;

        /// <summary>
        /// Locks this settings instance so that no property may be changed
        /// </summary>
        /// <returns>The unlock key</returns>
        public void Lock() => IsLocked = true;

        /// <summary>
        /// Unlocks this settings instance
        /// </summary>
        public void Unlock() => IsLocked = false;

        /// <summary>
        /// Safely sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="value"></param>
        public void SetProperty<T>(ref T container, T value)
        {
            if (IsLocked)
                throw new InvalidOperationException("This settings instance is locked and may not be changed");

            if (EqualityComparer<T>.Default.Equals(container, value))
                return;
            else
                container = value;
        }
    }
}
