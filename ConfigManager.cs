using System.Text.Json;

namespace RJ_VC_Bypass;

/// <summary>
/// Manages application configuration and user preferences
/// </summary>
public class ConfigManager
{
    private static ConfigManager? _instance;
    private readonly string _configPath;
    private ConfigData _data;

    public static ConfigManager Instance => _instance ??= new ConfigManager();

    public ConfigManager()
    {
        AppConfig.EnsureDataPath();
        _configPath = AppConfig.ConfigFile;
        _data = LoadConfig();
    }

    // User properties
    public string SavedUsername
    {
        get => _data.SavedUsername;
        set { _data.SavedUsername = value; SaveConfig(); }
    }

    public string SavedKey
    {
        get => _data.SavedKey;
        set { _data.SavedKey = value; SaveConfig(); }
    }

    public string ExpireDate
    {
        get => _data.ExpireDate;
        set { _data.ExpireDate = value; SaveConfig(); }
    }

    public string DeviceId
    {
        get => _data.DeviceId;
        set { _data.DeviceId = value; SaveConfig(); }
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(SavedKey);

    public string UsedKeys
    {
        get => _data.UsedKeys;
        set { _data.UsedKeys = value; SaveConfig(); }
    }

    public string SavedSid
    {
        get => _data.SavedSid;
        set { _data.SavedSid = value; SaveConfig(); }
    }

    private ConfigData LoadConfig()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                return JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
        }
        return new ConfigData();
    }

    private void SaveConfig()
    {
        try
        {
            string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config: {ex.Message}");
        }
    }

    public void Clear()
    {
        _data = new ConfigData();
        SaveConfig();
    }

    public void AddUsedKey(string key)
    {
        if (!UsedKeys.Contains($"[{key}]"))
        {
            UsedKeys = UsedKeys + $"[{key}]";
        }
    }

    public bool HasUsedKey(string key)
    {
        return UsedKeys.Contains($"[{key}]");
    }
}

/// <summary>
/// Configuration data model
/// </summary>
public class ConfigData
{
    public string SavedUsername { get; set; } = string.Empty;
    public string SavedKey { get; set; } = string.Empty;
    public string ExpireDate { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string UsedKeys { get; set; } = string.Empty;
    public string SavedSid { get; set; } = string.Empty;
}
