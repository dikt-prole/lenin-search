using System;

namespace BookProject.Core.Models
{
    public class KeyboardArgs : EventArgs
    {
        public int KeyValue { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Control { get; set; }
    }
}