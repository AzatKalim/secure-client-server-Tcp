using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("keys")]
    public class KeysExchange:BaseCommand
    {
        [XmlElement("sender")]
        public string sender
        {
            get;
            set;
        }

        [XmlElement("key")]
        public string key
        {
            get;
            set;
        }

        public KeysExchange(string sender, string key)
        {
            id = 5;
            this.key = key;
            this.sender = sender;
        }

        public KeysExchange()
        {
            id = 5;
        }
    }
}
