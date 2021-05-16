using System;
using System.IO;
using System.Windows.Forms;
using LeninSearch.Standard.Core;
using Newtonsoft.Json;

namespace LeninSearch
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            if (File.Exists(Constants.SettingsJsonPath))
            {
                var settings = JsonConvert.DeserializeObject<LeninSearchSettings>(File.ReadAllText(Constants.SettingsJsonPath));
                preloadFiles_chb.Checked = settings.PreloadFiles;
            }
            else
            {
                preloadFiles_chb.Checked = false;
            }
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            var settings = new LeninSearchSettings { PreloadFiles = preloadFiles_chb.Checked };
            if (!Directory.Exists(Constants.TempFolder))
            {
                Directory.CreateDirectory(Constants.TempFolder);
            }
            File.WriteAllText(Constants.SettingsJsonPath, JsonConvert.SerializeObject(settings));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
