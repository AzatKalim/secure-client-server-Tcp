using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("message")]
    public class Chat : BaseCommand
    {
        [XmlElement("sender")]
        public string sender
        {
            get;
            set;
        }

        [XmlElement("text")]
        public string text
        {
            get;
            set;
        }

        public Chat(string sender, string msg)
        {
            id = 6;
            text = msg;
            this.sender = sender;
        }

        public Chat()
        {
            id = 6;
        }
    }
}
