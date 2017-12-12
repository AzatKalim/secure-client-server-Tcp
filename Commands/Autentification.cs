using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("Autentification")]
    public class Autentification:BaseCommand
    {
        [XmlElement("login")]
        public string login
        {
            get;
            set;
        }

        [XmlElement("passwordHash")]
        public byte[] passwordHash
        {
            get;
            set;
        }

        public Autentification(string login, byte[] passwordHash)
        {
            id = 8;
            this.login= login;
            this.passwordHash = passwordHash;
        }

        public Autentification()
        {
            id = 8;           
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
