using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("CallAnswer")]
    public class CallAnswer:BaseCommand
    {
        [XmlElement("answer")]
        public string answer
        {
            get;
            set;
        }
        public CallAnswer(string answer)
        {
            id = 4;
            this.answer = answer;
        }
        public CallAnswer()
        {
            id = 4;
        }
    }
}
