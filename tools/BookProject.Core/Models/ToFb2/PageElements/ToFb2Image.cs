using System.Text;

namespace BookProject.Core.Models.ToFb2.PageElements
{
    public class ToFb2Image : BookElement
    {
        public int ImageIndex { get; set; }
        public string ImageFile { get; set; }

        public override void Render(StringBuilder stringBuilder)
        {
            stringBuilder.Append($"{NewLine}<image l:href=\"#img_{ImageIndex}.jpg\"/>");
        }
    }
}