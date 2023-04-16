using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookProject.Core.Models.ToFb2
{
    public class ToFb2Page
    {
        public int ReorderPoint { get; set; }
        public List<BookElement> Elements { get; set; }
        public void Render(StringBuilder stringBuilder)
        {
            if (Elements?.Any() != true)
            {
                return;
            }

            stringBuilder.Append("<section>");

            foreach (var element in Elements.OrderBy(e => e.ReorderPoint))
            {
                element.Render(stringBuilder);
            }

            stringBuilder.Append("</section>");
        }
    }
}