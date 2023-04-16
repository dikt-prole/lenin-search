using System;
using System.Text;

namespace BookProject.Core.Models.ToFb2
{
    public abstract class BookElement
    {
        protected string NewLine => Environment.NewLine;
        public int ReorderPoint { get; set; }
        public abstract void Render(StringBuilder stringBuilder);
    }
}