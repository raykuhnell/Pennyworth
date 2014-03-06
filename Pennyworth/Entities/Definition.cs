using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pennyworth.Entities
{
    public class Definition
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("command")]
        public string Command;
        [XmlArray("arguments")]
        [XmlArrayItem("argument")]
        public string[] Arguments;
        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        public string[] Tags;
        [XmlElement("icon")]
        public string Icon;
    }
}
