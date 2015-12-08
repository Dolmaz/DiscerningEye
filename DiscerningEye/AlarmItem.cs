using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscerningEye
{
    class AlarmItem
    {
        private int _ID;
        private TimeSpan _Time;
        private String _Item;
        private String _Location;
        private String _Slot;
        private String _Star;

        private string _JobClass;
        private string _NodeType;
        private string _BlueScripts;
        private string _RedScripts;
        private string _Notes;

        private bool _Armed;
        private bool _Active;

        public int ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        public TimeSpan Time
        {
            get { return this._Time; }
            set { this._Time = value; }
        }

        public String Item
        {
            get { return this._Item; }
            set { this._Item = value; }
        }

        public String Location
        {
            get { return this._Location; }
            set { this._Location = value; }
        }

        public String Slot
        {
            get { return this._Slot; }
            set { this._Slot = value; }
        }

        public String Star
        {
            get { return this._Star; }
            set { this._Star = value; }
        }
        public String JobClass
        {
            get { return this._JobClass; }
            set { this._JobClass = value; }
        }
        public String NodeType
        {
            get { return this._NodeType; }
            set { this._NodeType = value; }
        }
        public String BlueScripts
        {
            get { return this._BlueScripts; }
            set { this._BlueScripts = value; }
        }
        public String RedScripts
        {
            get { return this._RedScripts; }
            set { this._RedScripts = value; }
        }
        public String Notes
        {
            get { return this._Notes; }
            set { this._Notes = value; }
        }

        public bool Armed
        {
            get { return this._Armed; }
            set
            {
                this._Armed = value;
                if (!value)
                {
                    this.triggerTimer.Start();
                }
            }
        }

        public bool Active
        {
            get { return this._Active; }
            set { this._Active = value; }
        }

        private System.Timers.Timer triggerTimer;

        public AlarmItem(TimeSpan time, string item, string location, string slot, string star, string jobclass, string nodetype, string bluescripts, string redscripts, string notes)
        {
            this.Time = time;
            this.Item = item;
            this.Location = location;
            this.Slot = slot;
            this.Star = star;

            this.JobClass = jobclass;
            this.NodeType = nodetype;
            this.BlueScripts = bluescripts;
            this.RedScripts = redscripts;
            this.Notes = notes;

            this.Armed = true;
            this.Active = false;

            this.triggerTimer = new System.Timers.Timer();
            this.triggerTimer.Interval = 30000;
            this.triggerTimer.Elapsed += TriggerTimer_Elapsed;
        }

        public AlarmItem(Node node):this(TimeSpan.Parse(node.Time), node.Item, node.Location, node.Slot, node.Star, node.ClassJob, node.Nodetype, node.BlueScripts, node.RedScripts, node.Notes)
        {

        }

        private void TriggerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Armed = true;
            this.triggerTimer.Stop();
            //MessageBox.Show("Rearmed");
        }
    }
}
