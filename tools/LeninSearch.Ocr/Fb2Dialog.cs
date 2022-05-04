using System;
using System.IO;
using System.Windows.Forms;
using LeninSearch.Ocr.Model.Fb2;

namespace LeninSearch.Ocr
{
    public partial class Fb2Dialog : Form
    {
        public Fb2TemplateData TemplateData => new Fb2TemplateData
        {
            BookId = bookId_tb.Text,
            BookTitle = bookTitle_tb.Text,
            BookAnnotation = annotation_tb.Text,
            BookGenre = genre_tb.Text,
            BookAuthorFirstName = authorFirstName_tb.Text,
            BookAuthorLastName = authorLastName_tb.Text,
            BookAuthorMiddleName = authorMiddleName_tb.Text,
            DocAuthorFirstName = docAuthorFirstName_tb.Text,
            DocAuthorLastName = docAuthorLastName_tb.Text,
            DocId = docId_tb.Text,
            DocVersion = docVersion_tb.Text
        };

        public Fb2Dialog()
        {
            InitializeComponent();
            bookId_tb.Text = Guid.NewGuid().ToString("D");
            docId_tb.Text = Guid.NewGuid().ToString("D");
            docVersion_tb.Text = "1";
            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
