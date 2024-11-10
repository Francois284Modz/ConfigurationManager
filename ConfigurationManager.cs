using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrenchModdingTeam.Misc
{
	/// <summary>
	/// ConfigurationManager provides functionality to manage application configurations in JSON format.
	/// </summary>
	public class ConfigurationManager
	{
		private readonly string _configFilePath;
		private Dictionary<string, dynamic> _settings;

		/// <summary>
		/// Initializes a new instance of the ConfigurationManager class with a specified configuration file path.
		/// </summary>
		/// <param name="configFilePath">The path to the configuration JSON file.</param>
		public ConfigurationManager(string configFilePath)
		{
			_configFilePath = configFilePath;
			LoadConfiguration(); // Load configuration settings when an instance is created
		}

		/// <summary>
		/// Loads the configuration settings from the JSON file.
		/// </summary>
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
		/// Retrieves a configuration setting by its key.
		/// </summary>
		/// <typeparam name="T">The expected type of the setting.</typeparam>
		/// <param name="key">The key of the setting.</param>
		/// <returns>The value of the setting.</returns>
		/// <exception cref="KeyNotFoundException">Thrown when the key is not found in the configuration.</exception>
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
		/// Retrieves a nested configuration setting by its key and sub-key.
		/// </summary>
		/// <typeparam name="T">The expected type of the nested setting.</typeparam>
		/// <param name="key">The main key of the setting.</param>
		/// <param name="subKey">The sub-key of the nested setting.</param>
		/// <returns>The value of the nested setting.</returns>
		/// <exception cref="KeyNotFoundException">Thrown when the key or sub-key is not found.</exception>
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
		/// Updates or adds a setting to the configuration file.
		/// </summary>
		/// <typeparam name="T">The type of the setting.</typeparam>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value to set.</param>
		public void SetSetting<T>(string key, T value)
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
		}
	}
}
