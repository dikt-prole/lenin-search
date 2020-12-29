using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.Core.Oprimized;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public partial class App : Application
    {
        private State _state;
        public bool AllowBackButton => !_state.IsReading();

        public App(GlobalEvents globalEvents)
        {
            InitializeComponent();

            MainPage = new MainPage(globalEvents);
        }

        protected override void OnStart()
        {
            Debug.WriteLine($"OnStart");
            _state = LoadState();
            if (MainPage is MainPage mainPage)
            {
                mainPage.SetState(_state);
            }
        }

        protected override void OnSleep()
        {
            Debug.WriteLine($"OnSleep");
            OptimizedFileData.Clear();
            OptimizedDictionary.Clear();
            SaveState(_state);
        }

        protected override void OnResume()
        {
            Debug.WriteLine($"OnResume");
            _state = LoadState();
            if (MainPage is MainPage mainPage)
            {
                mainPage.SetState(_state);
            }
        }

        private static void SaveState(State state)
        {
            var stateFilePath = $"{FileUtil.StateFolder}/state.json";

            if (File.Exists(stateFilePath)) File.Delete(stateFilePath);

            if (!Directory.Exists(FileUtil.StateFolder)) Directory.CreateDirectory(FileUtil.StateFolder);

            var json = JsonConvert.SerializeObject(state);

            File.WriteAllText(stateFilePath, json);
        }

        private static State LoadState()
        {
            var stateFilePath = $"{FileUtil.StateFolder}/state.json";

            if (File.Exists(stateFilePath))
            {
                var json = File.ReadAllText(stateFilePath);
                return JsonConvert.DeserializeObject<State>(json);
            }

            var corpusItem = State.CorpusItems.First(ci => ci.Selected);
            return new State
            {
                CorpusName = corpusItem.Name,
                CurrentParagraphResultIndex = -1,
                ParagraphResults = new List<SearchParagraphResult>(),
                ReadingFile = null,
                SearchOptions = null
            };
        }
    }
}
