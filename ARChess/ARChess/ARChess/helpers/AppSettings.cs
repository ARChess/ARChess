using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;

namespace ARChess
{
    public class AppSettings
    {
        // Our isolated storage settings
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string AdvancedModeSettingKeyName = "AdvancedMode";
        const string GridAxisMarkersSettingKeyName = "AxisMarkers";
        const string SpeechCommandReminderOnSettingKeyName = "SpeechCommandReminder";
        
        // The default value of our settings
        const bool AdvancedModeSettingsDefault = false;
        const bool GridAxisMarkersDefault = true;
        const bool SpeechCommandReminderDefault = true;

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            try
            {
                // Get the settings for this application.
                settings = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException e)
            {
                // handle exception
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }


        /// <summary>
        /// Property to get and set a Advanced Mode Setting Key.
        /// </summary>
        public bool AdvancedModeSettings
        {
            get
            {
                return GetValueOrDefault<bool>(AdvancedModeSettingKeyName, AdvancedModeSettingsDefault);
            }
            set
            {
                if (AddOrUpdateValue(AdvancedModeSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a Grid Axis Mode Setting Key.
        /// </summary>
        public bool GridAxisMarkersSettings
        {
            get
            {
                return GetValueOrDefault<bool>(GridAxisMarkersSettingKeyName, GridAxisMarkersDefault);
            }
            set
            {
                if (AddOrUpdateValue(GridAxisMarkersSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a Speech Mode Setting Key.
        /// </summary>
        public bool SpeechCommandReminderSetting
        {
            get
            {
                return GetValueOrDefault<bool>(SpeechCommandReminderOnSettingKeyName, SpeechCommandReminderDefault);
            }
            set
            {
                if (AddOrUpdateValue(SpeechCommandReminderOnSettingKeyName, value))
                {
                    Save();
                }
            }
        }
    }
}