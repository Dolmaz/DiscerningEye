using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscerningEyeWPF
{
    class Nodes
    {
        [XmlElement("SetAlarm")]
        public bool SetAlarm { get; set; }

        [XmlElement("Time")]
        public string Time { get; set; }

        [XmlElement("Item")]
        public string Item { get; set; }

        [XmlElement("Slot")]
        public string Slot { get; set; }

        [XmlElement("Star")]
        public string Star { get; set; }

        [XmlElement("ClassJob")]
        public string ClassJob { get; set; }

        [XmlElement("Nodetype")]
        public string Nodetype { get; set; }

        [XmlElement("BlueScripts")]
        public string BlueScripts { get; set; }

        [XmlElement("RedScripts")]
        public string RedScripts { get; set; }

        [XmlElement("notes")]
        public string Notes { get; set; }
    }
}
