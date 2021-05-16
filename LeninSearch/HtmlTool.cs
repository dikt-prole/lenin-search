using System.Linq;
using LeninSearch.Standard.Core.Oprimized;

namespace LeninSearch
{
    public static class HtmlTool
    {
        public static string GetHtml(OptimizedFileData ofd, ushort paragraphIndex, string[] dictionary)
        {
            var headings = ofd.Headings?.OrderBy(h => h.Index).ToList();

            var beforeIndex = headings?.LastOrDefault(h => h.Index <= paragraphIndex)?.Index ?? 0;

            var afterIndex = headings?.FirstOrDefault(h => h.Index >= paragraphIndex)?.Index ?? ushort.MaxValue;

            var paragraphs = ofd.Paragraphs.Where(p => beforeIndex <= p.Index && p.Index < afterIndex).OrderBy(p => p.Index).ToList();

            var tags = paragraphs.Select(p => GetTag(p, ofd, dictionary)).Where(t => !string.IsNullOrEmpty(t)).ToList();
            
            var template = @"<html>
                                <head>
                                    <style>
                                        body 
                                        {
                                            width: 900px;
                                            margin: 0 auto;
                                            text-align: justify;
                                        }
                                        p.page
                                        {
                                            text-align: center;
                                        }                                        
                                    </style>
                                </head>
                                <body>
                                    <h1>[title]</h1>
                                    [text]
                                </body>
                            </html>";

            return template.Replace("[text]", string.Join("", tags)).Replace("[title]", ofd.GetHeadings(paragraphIndex).GetText(dictionary));
        }

        private static string GetTag(OptimizedParagraph p, OptimizedFileData ofd, string[] _dictionary)
        {
            if (ofd.Pages?.ContainsKey(p.Index) == true)
            {
                return $"<p class=\"page\">{ofd.Pages[p.Index]}</p>";
            }

            var pText = p.GetText(_dictionary);

            if (string.IsNullOrWhiteSpace(pText)) return null;

            return $"<p class=\"para\">{pText}</p>";
        }
    }
}