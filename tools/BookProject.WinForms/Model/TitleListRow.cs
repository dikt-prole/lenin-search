using System.Linq;
using BookProject.Core.Models.Domain;

namespace BookProject.WinForms.Model
{
    public class TitleListRow
    {
        public TitleBlock TitleBlock { get; set; }
        public Page Page { get; set; }

        public override string ToString()
        {
            var prefix = string.Join("", Enumerable.Repeat("    ", TitleBlock.Level));

            var title = string.IsNullOrEmpty(TitleBlock.Text) ? "-" : TitleBlock.Text;

            return $"{prefix} {title}";
        }
    }
}