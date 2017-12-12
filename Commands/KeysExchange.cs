using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("KeysExchange")]
    public class KeysExchange:BaseCommand
    {
        [XmlElement("sender")]
        public string sender
        {
            get;
            set;
        }

        [XmlElement("param")]
        public string param
        {
            get;
            set;
        }
        public KeysExchange(string sender, string param)
        {
            id = 5;
            this.param = param;
            this.sender = sender;
        }

        public KeysExchange()
        {
            id = 5;
        }
    }
}
