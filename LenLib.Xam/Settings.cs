using System.IO;
using LenLib.Xam.Core;
using Newtonsoft.Json;

namespace LenLib.Xam
{
    public class Settings
    {
        private static readonly string SettingsFileLocation = Path.Combine(Path.GetTempPath(), "settings-9387B828.json");

        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = File.Exists(SettingsFileLocation) 
                        ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFileLocation)) 
                        : Default();
                }

                return _instance;
            }
        }

        public Settings() { }

        public FontSize FontSize  { get; set; }

        public void Save()
        {
            var settingsJson = JsonConvert.SerializeObject(this);
            File.WriteAllText(SettingsFileLocation, settingsJson);
        }

        private static Settings Default()
        {
            return new Settings
            {
                FontSize = FontSize.SmallToMedium
            };
        }
    }
}