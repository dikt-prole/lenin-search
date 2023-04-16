using System.Text;

namespace BookProject.Core.Models.ToFb2.PageElements
{
    public class ToFb2CommentLink : BookElement
    {
        public int CommentIndex { get; set; }
        public string CommentText { get; set; }
        public override void Render(StringBuilder stringBuilder)
        {
            stringBuilder.Append($"<a l:href=\"#n_{CommentIndex}\" type=\"note\">[{CommentIndex}]</a> ");
        }
    }
}