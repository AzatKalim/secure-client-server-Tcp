using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("keys")]
    public class KeysEchange:BaseCommand
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

        public KeysEchange(string sender, string key)
        {
            id = 5;
            this.key = key;
            this.sender = sender;
        }

        public KeysEchange()
        {
            id = 5;
        }
    }
}
