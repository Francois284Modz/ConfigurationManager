
# ConfigurationManager

`ConfigurationManager` is a C# utility class for handling application configuration in JSON format. It provides an easy way to load, read, and modify configuration settings stored in a JSON file.

### Features

- **Load Configuration**: Automatically loads settings from a JSON configuration file when an instance is created.
- **Retrieve Settings**: Access settings by key, or by key and sub-key for nested JSON structures.
- **Update/Add Settings**: Modify or add new settings, with changes saved directly to the JSON file.

### Installation

Include the `ConfigurationManager` class in your project, ensuring you have the `Newtonsoft.Json` package installed:

```bash
dotnet add package Newtonsoft.Json
```

### Usage

#### 1. Create an Instance

```csharp
var configManager = new ConfigurationManager("path/to/config.json");
```

#### 2. Retrieve a Setting by Key

```csharp
string apiUrl = configManager.GetSetting<string>("ApiUrl");
```

#### 3. Retrieve a Nested Setting by Key and Sub-Key

```csharp
int timeout = configManager.GetSetting<int>("ConnectionSettings", "Timeout");
```

#### 4. Update or Add a New Setting

```csharp
configManager.SetSetting("NewSettingKey", "SomeValue");
```

### JSON Configuration Example

Example `config.json` file:

```json
{
  "ApiUrl": "https://api.example.com",
  "ConnectionSettings": {
    "Timeout": 30,
    "RetryCount": 3
  }
}
```

### Exception Handling

The `ConfigurationManager` class throws exceptions for:
- Missing configuration file (`FileNotFoundException`)
- JSON format errors (`JsonReaderException`)
- Missing keys in the configuration (`KeyNotFoundException`)
