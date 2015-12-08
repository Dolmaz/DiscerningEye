using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscerningEye
{
    class Node
    {
        public bool SetAlarm { get; set; }
        public string Time { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public string Slot { get; set; }
        public string Star { get; set; }

        public string ClassJob { get; set; }
        public string Nodetype { get; set; }
        public string BlueScripts { get; set; }
        public string RedScripts { get; set; }
        public string Notes { get; set; }

        public Node(bool setAlarm, string time, string item, string location, string slot, string star, string classjob, string nodetype, string bluescripts, string redscripts, string notes)
        {
            //  Initialize
            this.SetAlarm = setAlarm;
            this.Time = time;
            this.Location = location;
            this.Item = item;
            this.Slot = slot;
            this.Star = star;
            this.ClassJob = classjob;
            this.Nodetype = nodetype;
            this.BlueScripts = bluescripts;
            this.RedScripts = redscripts;
            this.Notes = notes;
        }
    }
}
