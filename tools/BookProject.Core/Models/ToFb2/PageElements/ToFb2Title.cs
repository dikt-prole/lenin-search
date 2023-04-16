using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookProject.Core.Models.ToFb2.PageElements
{
    public class ToFb2Title : BookElement
    {
        public ToFb2Title()
        {
            Elements = new List<BookElement>();
        }

        public List<BookElement> Elements { get; set; }

        public override void Render(StringBuilder stringBuilder)
        {
            if (!Elements.Any())
            {
                return;
            }

            stringBuilder.AppendLine($"{NewLine}<title>{NewLine}");

            foreach (var element in Elements)
            {
                element.Render(stringBuilder);
            }

            stringBuilder.AppendLine($"{NewLine}<title>");
        }
    }
}