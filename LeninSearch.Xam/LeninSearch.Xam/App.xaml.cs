using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Core;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Application = Xamarin.Forms.Application;

namespace LeninSearch.Xam
{
    public partial class App : Application
    {
        private State _state;
        public bool AllowBackButton => !_state.IsReading();

        public App(GlobalEvents globalEvents)
        {
            Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            InitializeComponent();
            MainPage = new MainPage(globalEvents);
        }

        protected override void OnStart()
        {
            Debug.WriteLine($"OnStart");
            _state = LoadState(true);
            if (MainPage is MainPage mainPage)
            {
                mainPage.SetState(_state);
            }
        }

        protected override void OnSleep()
        {
            Debug.WriteLine($"OnSleep");
            (MainPage as MainPage)?.CleanCache();
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
            var stateFilePath = Path.Combine(Settings.StateFolder, "state.json");

            if (File.Exists(stateFilePath)) File.Delete(stateFilePath);

            if (!Directory.Exists(Settings.StateFolder)) Directory.CreateDirectory(Settings.StateFolder);

            var json = JsonConvert.SerializeObject(state);

            File.WriteAllText(stateFilePath, json);
        }

        private static State LoadState(bool leaveOnlyCorpusSelection = false)
        {
            var stateFilePath = Path.Combine(Settings.StateFolder, "state.json");

            if (File.Exists(stateFilePath))
            {
                var json = File.ReadAllText(stateFilePath);

                var state = JsonConvert.DeserializeObject<State>(json);

                if (leaveOnlyCorpusSelection)
                {
                    state.PartialParagraphSearchResult = null;
                    state.CurrentParagraphResultIndex = -1;
                    state.SearchQuery = null;
                    state.ReadingFile = null;
                }

                return state;
            }

            var corpusItem = State.GetCorpusItems().FirstOrDefault(ci => ci.Selected);
            if (corpusItem == null)
            {
                corpusItem = State.GetCorpusItems().First();
            }

            return new State
            {
                CorpusId = corpusItem.Id,
                CurrentParagraphResultIndex = -1,
                ReadingFile = null,
                SearchQuery = null
            };
        }
    }
}
