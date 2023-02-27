using System;

namespace BookProject.Core.Models
{
    public class MessageArgs : EventArgs
    {
        public string Text { get; set; }
        public MessageType MessageType { get; set; }
    }
}