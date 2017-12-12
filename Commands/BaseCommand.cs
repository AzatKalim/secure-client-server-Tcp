using System;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Xml;

namespace Commands
{
    public class BaseCommand
    {
        [XmlAttribute("id")]
        public int id
        {
            get;
            set;
        }

        public BaseCommand()
        {
        }

        public static Type ReturnTypeOfCommand(int id)
        {
            switch (id)
            {
                case 1:
                    {
                        return typeof(Registration);
                    }
                case 2:
                    {
                        return typeof(RegistratioAnswer);
                    }
                case 3:
                    {
                        return typeof(Call);
                    }
                case 4:
                    {
                        return typeof(CallAnswer);
                    }
                case 5:
                    {
                        return typeof(KeysExchange);
                    }
                case 6:
                    {
                        return typeof(Chat);
                    }
                case 7:
                    {
                        return typeof(Stop);
                    }
                case 8:
                    {
                        return typeof(Autentification);
                    }
                case 9:
                    {
                        return typeof(AutentificationAnswer);
                    }
                case 10:
                    {
                        return typeof(PreKeyExchange);
                    }
                default: return null;
            }
        }

        public string Serialize()
        {
            XmlSerializer xmler = new XmlSerializer(GetType());
            StringWriter writer = new StringWriter();
            xmler.Serialize(writer, this);
            return writer.ToString();
        }

        public static BaseCommand Deserialize(int id, string cmd)
        {
            try
            {
                Type typeOfCommand = ReturnTypeOfCommand(id);
                XmlSerializer xmler = new XmlSerializer(typeOfCommand);
                StringReader reader = new StringReader(cmd);
                return (BaseCommand)xmler.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
