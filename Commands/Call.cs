using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("Call")]
    public class Call:BaseCommand
    {
        [XmlElement("sender")]
        public string sender
        {
            get;
            set;
        } 
        public Call(string sender)
        {
            id = 3;
            this.sender = sender;
        }
    }
}
