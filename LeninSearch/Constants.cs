using System.IO;

namespace LeninSearch
{
    public class Constants
    {
        public static readonly string TempFolder = $"{Path.GetTempPath()}\\LeninSearch";

        public static readonly string SettingsJsonPath = $"{Path.GetTempPath()}\\LeninSearch\\lenin_search_settings.json";
    }
}