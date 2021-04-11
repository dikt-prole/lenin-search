using System.IO;

namespace LeninSearch
{
    public class Constants
    {
        public static string SettingsJsonPath => $"{Path.GetTempPath()}\\lenin_search_settings.json";
    }
}