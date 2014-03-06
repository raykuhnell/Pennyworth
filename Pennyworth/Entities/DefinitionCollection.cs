using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pennyworth.Entities
{
    [XmlRoot("definitions")]
    public class DefinitionCollection
    {
        [XmlElement("definition")]
        public List<Definition> Items;
    }
}
