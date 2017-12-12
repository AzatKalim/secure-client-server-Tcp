using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;


namespace Commands
{
    [XmlRoot("PreKeyExchange")]
    public class PreKeyExchange : BaseCommand
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

        [XmlElement("q")]
        public string q
        {
            get;
            set;
        }

        [XmlElement("n")]
        public string n
        {
            get;
            set;
        }

        public PreKeyExchange()
        {
            id = 10;
        }

        public PreKeyExchange(string q, string n,string param)
        {
            id = 10;
            this.q = q;
            this.n = n;
            this.param = param;
        }
    }
}
