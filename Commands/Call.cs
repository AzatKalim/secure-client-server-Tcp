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
        [XmlElement("login")]
        public string login
        {
            get;
            set;
        } 
        public Call(string login)
        {
            id = 3;
            this.login = login;
        }
        public Call()
        {
            id = 3;
        }
    }
}
