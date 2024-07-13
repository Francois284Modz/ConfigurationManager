using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrenchModdingTeam.Misc
{
    /// <summary>
    /// Manages configuration settings from a JSON file.
    /// </summary>
    public class ConfigurationManager
	{
		private readonly string _configFilePath;
		private Dictionary<string, dynamic> _settings;

        /// <summary>
        /// Initializes a new instance of the ConfigurationManager class.
        /// </summary>
        /// <param name="configFilePath">The path to the configuration file.</param>
        public ConfigurationManager(string configFilePath)
		{
			_configFilePath = configFilePath;
			LoadConfiguration(); // Load configuration settings when an instance is created
		}

        /// <summary>
        /// Loads configuration settings from the file specified by _configFilePath.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when the configuration file does not exist.</exception>
        /// <exception cref="Exception">Thrown when the configuration file cannot be deserialized or other errors occur during loading.</exception>

        private void LoadConfiguration()
		{
			if (!File.Exists(_configFilePath))
			{
				throw new FileNotFoundException("Configuration file not found.", _configFilePath);
			}

			try
			{
				string json = File.ReadAllText(_configFilePath);
				_settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
				if (_settings == null)
				{
					throw new Exception("Failed to deserialize the configuration file.");
				}
			}
			catch (JsonReaderException ex)
			{
				throw new Exception("JSON format error in configuration file: " + ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Error loading configuration: " + ex.Message);
			}
		}

        /// <summary>
        /// Retrieves a setting by key.
        /// </summary>
        /// <typeparam name="T">The expected type of the setting value.</typeparam>
        /// <param name="key">The key of the setting to retrieve.</param>
        /// <returns>The value of the setting cast to the type T.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the configuration file does not exist.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the specified key is not found in the configuration.</exception>

        public T GetSetting<T>(string key)
		{
			if (!File.Exists(_configFilePath))
			{
				throw new FileNotFoundException("Configuration file not found.", _configFilePath);
			}

			string json = File.ReadAllText(_configFilePath);
			var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

			if (settings != null && settings.TryGetValue(key, out object value))
			{
				return JsonConvert.DeserializeObject<T>(value.ToString());
			}

			throw new KeyNotFoundException($"Setting '{key}' not found.");
		}

        /// <summary>
        /// Retrieves a setting by key, allowing for nested JSON objects.
        /// </summary>
        /// <typeparam name="T">The type of the nested setting to retrieve.</typeparam>
        /// <param name="key">The main key of the setting.</param>
        /// <param name="subKey">The sub-key of the nested setting.</param>
        /// <returns>The value of the nested setting cast to the type T.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the specified key or sub-key is not found.</exception>

        public T GetSetting<T>(string key, string subKey)
		{
			if (_settings != null && _settings.ContainsKey(key))
			{
				var subSettings = _settings[key] as JObject; // Cast to JObject for nested objects
				if (subSettings != null && subSettings[subKey] != null)
				{
					return subSettings[subKey].ToObject<T>();
				}
			}

			throw new KeyNotFoundException($"Setting '{subKey}' under '{key}' not found.");
		}

        /// <summary>
        /// Sets or updates a setting in the configuration file.
        /// </summary>
        /// <typeparam name="T">The type of the setting value.</typeparam>
        /// <param name="key">The key of the setting to set or update.</param>
        /// <param name="value">The new value for the setting.</param>
        /// <exception cref="Exception">Thrown when an error occurs during file write operation.</exception>
        /* public void SetSetting<T>(string key, T value)
         {
             Dictionary<string, T> settings;
             if (File.Exists(_configFilePath))
             {
                 string json = File.ReadAllText(_configFilePath);
                 settings = JsonConvert.DeserializeObject<Dictionary<string, T>>(json) ?? new Dictionary<string, T>();
             }
             else
             {
                 settings = new Dictionary<string, T>();
             }

             settings[key] = value;
             string updatedJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
             File.WriteAllText(_configFilePath, updatedJson);
         }*/

        public void SetSetting<T>(string key, T value)
        {
            // Read the existing configuration file or create a new dictionary if it doesn't exist
            Dictionary<string, object> settings;
            if (File.Exists(_configFilePath))
            {
                string json = File.ReadAllText(_configFilePath);
                settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }
            else
            {
                settings = new Dictionary<string, object>();
            }

            // Convert value to a JObject if it is not a primitive type, to handle complex objects correctly
            JObject jObjectValue = value as JObject ?? JObject.FromObject(value);

            // Update or add the new setting
            settings[key] = jObjectValue;

            // Serialize the updated dictionary to JSON
            string updatedJson = JsonConvert.SerializeObject(settings, Formatting.Indented);

            // Write the updated JSON back to the file
            File.WriteAllText(_configFilePath, updatedJson);
        }

    }
}
