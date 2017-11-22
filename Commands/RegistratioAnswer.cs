using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("RegistrationAnswer")]
    public class RegistratioAnswer:BaseCommand
    {
        [XmlElement("answer")]
        public string answer
        {
            get;
            set;
        }
        public RegistratioAnswer(string answer)
        {
            id = 2;
            this.answer = answer;
        }
    }
}
