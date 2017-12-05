using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("Autentification")]
    public class AutentificationAnswer:BaseCommand
    {
        [XmlElement("answer")]
        public bool answer
        {
            private get;
            set;
        }

        public AutentificationAnswer(bool answer)
        {
            id = 9;
            this.answer=answer;
        }

        public AutentificationAnswer()
        {
            id =9;           
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
