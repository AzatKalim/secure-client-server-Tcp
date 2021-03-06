﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Commands
{
    [XmlRoot("Registration")]
    public class Registration : BaseCommand
    {
        [XmlElement("login")]
        public string login
        {
            get;
            set;
        }

        [XmlElement("password")]
        public string password
        {
            get;
            set;
        }

        public Registration(string login,string password)
        {
            id = 1;
            this.login= login;
            this.password= password;
        }

        public Registration()
        {
            id = 1;           
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    
}
