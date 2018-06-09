using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleWrapper.Settings
{
    public class SettingsBase
    {
        /// <summary>
        /// Safely sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="container"></param>
        protected void SetValue<T>(T value, ref T container)
        {
            if (value.Equals(null))
                throw new ArgumentNullException(nameof(value));
            if (!value.Equals(container))
                container = value;
        }
    }
}
