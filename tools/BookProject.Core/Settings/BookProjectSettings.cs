using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace BookProject.Core.Settings
{
    public class BookProjectSettings
    {
        public DetectImageSettings ImageDetection { get; set; }
        public DetectTitleSettings TitleDetection { get; set; }
        public DetectCommentLinkSettings CommentLinkDetection { get; set; }
        public DetectGarbageSettings GarbageDetection { get; set; }

        public static BookProjectSettings Load()
        {
            var settingsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "settings.json");

            if (File.Exists(settingsFile))
            {
                return JsonConvert.DeserializeObject<BookProjectSettings>(File.ReadAllText(settingsFile));
            }

            return Default();
        }

        public void Save()
        {
            var settingsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "settings.json");

            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        private static BookProjectSettings Default()
        {
            return new BookProjectSettings
            {
                ImageDetection = new DetectImageSettings
                {
                    AddPadding = 10,
                    GaussSigma1 = 16,
                    GaussSigma2 = 2,
                    MinHeight = 300,
                    MinBottom = 100,
                    MinLeft = 100,
                    MinRight = 100,
                    MinTop = 100
                },
                TitleDetection = new DetectTitleSettings
                {
                    AddPadding = 10,
                    GaussSigma1 = 16,
                    GaussSigma2 = 1,
                    MaxLineDist = 100,
                    MinRight = 100,
                    MinTop = 100,
                    MinBottom = 100,
                    MinLeft = 100
                },
                CommentLinkDetection = new DetectCommentLinkSettings
                {
                    LineGaussSigma1 = 16,
                    LineGaussSigma2 = 1,
                    TopDeltaMax = 0.15,
                    BottomDeltaMin = 5,
                    AddPadding = 2
                },
                GarbageDetection = new DetectGarbageSettings
                {
                    AddPadding = 10,
                    GaussSigma1 = 16,
                    GaussSigma2 = 2,
                    MaxHeight = 200,
                    MinHeight = 15,
                    MinLeft = 100,
                    MinRight = 100
                }
            };
        }
    }
}