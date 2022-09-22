using System;

namespace Assets.Scripts.Model
{
    public class Message
    {
        public DateTime TimeStamp { get; set; }
        public string Owner { get; set; }
        public string Text { get; set; }
    }
}