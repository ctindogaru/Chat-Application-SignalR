using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplication
{
    public class Message
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public string Username { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}