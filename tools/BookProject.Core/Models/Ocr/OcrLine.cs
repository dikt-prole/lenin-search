﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BookProject.Core.Models.Domain;

namespace BookProject.Core.Models.Ocr
{
    public class OcrLine
    {
        public int TopLeftX { get; set; }

        public int TopLeftY { get; set; }

        public int BottomRightX { get; set; }

        public int BottomRightY { get; set; }

        public List<OcrWord> Words { get; set; }

        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        public string Text => Words == null ? string.Empty : string.Join(" ", Words.Select(w => w.Text));

        public Line ToLine()
        {
            return new Line
            {
                BottomRightY = BottomRightY,
                BottomRightX = BottomRightX,
                TopLeftY = TopLeftY,
                TopLeftX = TopLeftX,
                Type = LineType.Normal,
                Words = Words.Select(w => w.ToWord()).ToList()
            };
        }
    }
}