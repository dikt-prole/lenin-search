using System;
using System.Diagnostics;
using System.IO;
using LenLib.Xam.State;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Application = Xamarin.Forms.Application;

[assembly: ExportFont("InterBold.ttf", Alias = "InterBold")]
[assembly: ExportFont("InterItalic.ttf", Alias = "InterItalic")]
[assembly: ExportFont("InterRegular.ttf", Alias = "InterRegular")]
namespace LenLib.Xam
{
    public partial class App : Application
    {
        private AppState _state;
        public bool AllowBackButton => true;

        public App(GlobalEvents globalEvents)
        {
            Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            InitializeComponent();
            MainPage = new MainPage(globalEvents);
        }

        protected override void OnStart()
        {
            Debug.WriteLine($"{DateTime.Now} OnStart");
            _state = LoadState();
            if (MainPage is MainPage mainPage)
            {
                mainPage.SetState(_state);
            }
        }

        protected override void OnSleep()
        {
            Debug.WriteLine($"{DateTime.Now} OnSleep");
            (MainPage as MainPage)?.CleanCache();
            SaveState(_state);
        }

        protected override void OnResume()
        {
            Debug.WriteLine($"{DateTime.Now} OnResume");
            _state = LoadState();
            if (MainPage is MainPage mainPage)
            {
                mainPage.SetState(_state);
            }
        }

        private static void SaveState(AppState state)
        {
            var stateFilePath = Path.Combine(Settings.StateFolder, "state.json");

            if (File.Exists(stateFilePath)) File.Delete(stateFilePath);

            if (!Directory.Exists(Settings.StateFolder)) Directory.CreateDirectory(Settings.StateFolder);

            var json = JsonConvert.SerializeObject(state);

            File.WriteAllText(stateFilePath, json);
        }

        private static AppState LoadState()
        {
            var stateFilePath = Path.Combine(Settings.StateFolder, "state.json");

            if (File.Exists(stateFilePath))
            {
                var json = File.ReadAllText(stateFilePath);

                var state = JsonConvert.DeserializeObject<AppState>(json);

                return state;
            }

            return AppState.Default();
        }
    }
}
