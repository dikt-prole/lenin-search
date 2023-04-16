using System.Text;

namespace BookProject.Core.Models.ToFb2.PageElements
{
    public class ToFb2Word : BookElement
    {
        public string Text { get; set; }
        public override void Render(StringBuilder stringBuilder)
        {
            stringBuilder.Append(Text);
            stringBuilder.Append(" ");
        }
    }
}