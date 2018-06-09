using System;

namespace ConsoleWrapper.Settings
{
    public abstract class SettingsBase : ISettings
    {
        public bool IsLocked { get; protected set; }

        internal SettingsBase() => IsLocked = false;

        /// <summary>
        /// Locks this settings instance so that no property may be changed
        /// </summary>
        /// <returns>The unlock key</returns>
        public Guid Lock()
        {
            IsLocked = true;
            return Guid.NewGuid();
        }

        /// <summary>
        /// Unlocks this settings instance
        /// </summary>
        /// <param name="key">The key obtained when locking the instance, used to unlock it</param>
        public void Unlock(Guid key) => IsLocked = false;

        /// <summary>
        /// Safely sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="container"></param>
        public void SetValue<T>(T value, ref T container)
        {
            if (IsLocked)
                throw new InvalidOperationException("This settings instance is locked and may not be changed!");
            if (value.Equals(null))
                throw new ArgumentNullException(nameof(value));
            if (!value.Equals(container))
                container = value;
        }
    }
}
