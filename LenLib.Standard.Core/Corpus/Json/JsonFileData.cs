﻿using System.Collections.Generic;

namespace LenLib.Standard.Core.Corpus.Json
{
    public class JsonFileData
    {
        public List<JsonHeading> Headings { get; set; }
        public Dictionary<ushort, ushort> Pages { get; set; }
        public List<JsonParagraph> Pars { get; set; }
    }
}