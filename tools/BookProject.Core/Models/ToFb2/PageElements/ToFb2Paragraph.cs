using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookProject.Core.Models.ToFb2.PageElements
{
    public class ToFb2Paragraph : BookElement
    {
        public ToFb2Paragraph()
        {
            Elements = new List<BookElement>();
        }

        public List<BookElement> Elements { get; set; }

        public override void Render(StringBuilder stringBuilder)
        {
            if (Elements?.Any() != true)
            {
                return;
            }

            stringBuilder.Append($"{NewLine}<p>{NewLine}");

            foreach (var pageElement in Elements)
            {
                pageElement.Render(stringBuilder);   
            }

            stringBuilder.Append($"{NewLine}</p>");
        }
    }
}