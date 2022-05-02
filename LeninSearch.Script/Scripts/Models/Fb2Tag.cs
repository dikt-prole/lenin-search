using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Script.Scripts.Models
{
    public class Fb2Tag
    {
        public Fb2Tag Parent { get; set; }
        public List<Fb2Tag> Children { get; set; }
        public Fb2Line Fb2Line { get; set; }
        public int? TitleLevel { get; set; }

        public string GetXml()
        {
            var nl = Environment.NewLine;
            if (Fb2Line.Type == Fb2LineType.Title)
            {
                var childrenSb = new StringBuilder();
                foreach (var tag in Children)
                {
                    childrenSb.Append(tag.GetXml());
                    childrenSb.Append(nl);
                }
                return $"<section>{nl}{Fb2Line.GetXml()}{nl}{childrenSb}{nl}</section>";
            }

            return Fb2Line.GetXml();
        }
    }
}