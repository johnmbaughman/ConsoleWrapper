using System;

namespace ConsoleWrapper.Settings
{
    public abstract class SettingsBase
    {
        private bool _isLocked;

        internal SettingsBase() => _isLocked = false;

        /// <summary>
        /// Locks this settings instance so that no property may be changed
        /// </summary>
        public void Lock() => _isLocked = true;

        /// <summary>
        /// Safely sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="container"></param>
        protected void SetValue<T>(T value, ref T container)
        {
            if (_isLocked)
                throw new InvalidOperationException("This settings instance is locked and may not be changed!");
            if (value.Equals(null))
                throw new ArgumentNullException(nameof(value));
            if (!value.Equals(container))
                container = value;
        }
    }
}
