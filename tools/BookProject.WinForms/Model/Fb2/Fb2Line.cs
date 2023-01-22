using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Model.Fb2
{
    public class Fb2Line
    {
        public int TopLeftY { get; set; }
        public int TitleLevel { get; set; }
        public string TitleText { get; set; }
        public List<BookProjectWord> TitleCommentLinks { get; set; }
        public List<BookProjectLine> Lines { get; set; }
        public Fb2LineType Type { get; set; }
        public int ImageIndex { get; set; }
        public string ImageBase64 { get; set; }
        public string TextPreview => Lines?.Any() == true ? GetText() : null;

        public override string ToString()
        {
            return $"[{Type}]: {TextPreview}";
        }

        public string GetText()
        {
            switch (Type)
            {
                case Fb2LineType.Title:
                    return TitleText;
                case Fb2LineType.Image:
                    return null;
                default:
                    var sb = new StringBuilder();
                    var lastLineEndedWithDash = false;
                    foreach (var line in Lines)
                    {
                        var lastWord = line.Words.Last();
                        var currentLineEndedWithDash = (Type == Fb2LineType.Paragraph || Type == Fb2LineType.Comment) &&
                                                       lastWord.Text.EndsWith('-');

                        if (currentLineEndedWithDash) lastWord.Text = lastWord.Text.TrimEnd('-');

                        if (!lastLineEndedWithDash) sb.Append(" ");

                        sb.Append(string.Join(" ", line.Words.Select(GetWordXml)));

                        lastLineEndedWithDash = currentLineEndedWithDash;
                    }
                    return sb.ToString();
            }
        }

        public string GetWordXml(BookProjectWord word)
        {
            if (Type != Fb2LineType.Comment && word.IsCommentLinkNumber)
                return $"<a l:href=\"#n_{word.Text}\" type=\"note\">[{word.Text}]</a>";

            return word.Text;
        }

        public string GetXml()
        {
            switch (Type)
            {
                case Fb2LineType.Paragraph:
                    return $"<p>{GetText()}</p>";
                case Fb2LineType.Title:
                    var titleCommentLinks = TitleCommentLinks == null
                        ? null
                        : string.Join("", TitleCommentLinks
                            .Select(ttl => $"<a l:href=\"#n_{ttl.Text}\" type=\"note\">[{ttl.Text}]</a>"));
                    return $"<title><p>{GetText()}</p>{titleCommentLinks}</title>";
                case Fb2LineType.Image:
                    return $"<image l:href=\"#i_{ImageIndex}.jpg\"/>";
                case Fb2LineType.Comment:
                    var linkWord = Lines.SelectMany(l => l.Words).FirstOrDefault(w => w.IsCommentLinkNumber);
                    return linkWord == null
                        ? null
                        : $"<p id=\"n_{linkWord.Text}\">[{linkWord.Text}] {GetText()}</p>";
            }

            return null;
        }

        public static Fb2Line Construct(BookProjectLine line)
        {
            var fb2Line = new Fb2Line
            {
                Type = line.Label == BookProjectLabel.Title
                    ? Fb2LineType.Title
                    : line.Label == BookProjectLabel.Comment
                        ? Fb2LineType.Comment
                        : Fb2LineType.Paragraph,
                Lines = new List<BookProjectLine> { line },
                TopLeftY = line.TopLeftY
            };

            return fb2Line;
        }

        public static Fb2Line Construct(BookProjectImageBlock imageBlock, string bookFolder, BookProjectPage page, int imageIndex)
        {
            try
            {
                var fb2Line = new Fb2Line
                {
                    Type = Fb2LineType.Image,
                    TopLeftY = imageBlock.TopLeftY
                };

                var imagePath = Directory.GetFiles(bookFolder).SingleOrDefault(f => Path.GetFileNameWithoutExtension(f) == page.Filename);

                if (imagePath == null) throw new Exception($"Image file '{page.Filename}' not found");

                using var bitmap = new Bitmap(Image.FromFile(imagePath));

                using var imageBitmap = bitmap.Clone(imageBlock.Rectangle, bitmap.PixelFormat);

                using var stream = new MemoryStream();

                imageBitmap.Save(stream, ImageFormat.Jpeg);

                var imageBytes = stream.ToArray();

                fb2Line.ImageBase64 = Convert.ToBase64String(imageBytes);

                fb2Line.ImageIndex = imageIndex;

                return fb2Line;
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"Exception on page {page.ImageIndex}: {exc.Message}");
                throw;
            }
        }

        public static Fb2Line Construct(BookProjectTitleBlock titleBlock)
        {
            return new Fb2Line
            {
                Type = Fb2LineType.Title,
                TitleLevel = titleBlock.TitleLevel,
                TitleText = titleBlock.TitleText,
                TopLeftY = titleBlock.TopLeftY,
                TitleCommentLinks = titleBlock.CommentLinks
            };
        }
    }

    public enum Fb2LineType
    {
        Paragraph, Image, Title, Comment
    }
}