using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("Stop")]
    public class Stop:BaseCommand
    {       
        public Stop()
        {
            id = 7;
        }
    }
}
