namespace RJ_VC_Bypass;

/// <summary>
/// Application configuration - API endpoints and settings
/// </summary>
public static class AppConfig
{
    // Supabase API Configuration
    public const string SUPABASE_URL = "https://hxgdamggfoovqyywdtzq.supabase.co";
    public const string SUPABASE_API_KEY = "sb_publishable_GtsMNHTpkfmXUj4Wos0G3g_Zp5jMsaO";
    public const string SUPABASE_ANON_KEY = "sb_publishable_GtsMNHTpkfmXUj4Wos0G3g_Zp5jMsaO";

    // Google Apps Script for license logging
    public const string LICENSE_LOG_URL = "https://script.google.com/macros/s/AKfycbwaSHdtnwmHouHv1E_qQhKOjFq4HYmtqKcbJrOncpQS5UjtD62godbrEeYpsO0HqfyHjA/exec";

    // Portal API endpoint
    public const string PORTAL_AUTH_URL = "https://portal-as.ruijienetworks.com/api/auth/voucher/";

    // User agent for HTTP requests
    public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36";

    // Application paths
    public static string DataPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RJ_VC_Bypass");

    public static string ConfigFile => Path.Combine(DataPath, "config.json");

    // Ensure data directory exists
    public static void EnsureDataPath()
    {
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
    }
}
