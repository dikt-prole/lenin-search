﻿using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BookProject.WinForms
{
    public partial class ImageScopeDialog : Form
    {
        public int MaxImageIndex => (int)maxImageIndex_nud.Value;
        public int MinImageIndex => (int)minImageIndex_nud.Value;

        public ImageScopeDialog()
        {
            InitializeComponent();

            maxImageIndex_nud.Minimum = 1;
            maxImageIndex_nud.Maximum = 10000;
            maxImageIndex_nud.Value = 1000;

            minImageIndex_nud.Minimum = 0;
            minImageIndex_nud.Maximum = 10000;
            minImageIndex_nud.Value = 0;

            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public bool ImageMatch(int imageIndex)
        {
            return MinImageIndex <= imageIndex && imageIndex <= MaxImageIndex;
        }

        public string[] GetMatchingImages(string bookFolder)
        {
            return Directory.GetFiles(bookFolder, "*.jpg")
                .Where(f => ImageMatch(
                    int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray()))))
                .ToArray();
        }
    }
}
