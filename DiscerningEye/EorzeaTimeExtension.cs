using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscerningEye
{
    class EorzeaTimeExtension
    {

        private DateTime currentEorzeaTime;
        private DateTime currentLocalTime;
        private DateTime currentUTCTime;
        private System.Timers.Timer updateTImer;

        public DateTime CurrentEorzeaTime
        {
            get { return this.currentEorzeaTime; }
            set
            {
                this.currentEorzeaTime = value;
                this.EorzeaTimeUpdate(new EorzeaTimeEventArgs(value));
            }
        }

        public DateTime CurrentLocalTime
        {
            get { return this.currentLocalTime; }
            set
            {
                this.currentLocalTime = value;
                this.LocalTimeUpdate(new EorzeaTimeEventArgs(value));
            }
        }

        public DateTime CurrentUTCTime
        {
            get { return this.currentUTCTime; }
            set
            {
                this.currentUTCTime = value;
                this.UTCTimeUpdate(new EorzeaTimeEventArgs(value));
            }
        }




        public EorzeaTimeExtension()
        {
            //  Initizlie
            updateTImer = new System.Timers.Timer();
            updateTImer.Interval = 1;
            updateTImer.Elapsed += UpdateTImer_Elapsed;
            updateTImer.Start();
        }

        private void UpdateTImer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.CurrentLocalTime = DateTime.Now;
            this.CurrentUTCTime = DateTime.UtcNow;
            this.CurrentEorzeaTime = GetEorzeaTime();
        }

        private DateTime GetEorzeaTime()
        {
            const double EORZEA_MULTIPLIER = 3600D / 175D;
            //const double EORZEA_MULTIPLIER = 20.5714285714;

            //  Calculate how many ticks have elapsed since Epoch Time
            long epochTicks = this.CurrentUTCTime.Ticks - (new DateTime(1970, 1, 1).Ticks);

            //  Multiply those ticks by the Eorzea Multipler (approx 20.5x)
            long eorzeaTicks = (long)Math.Round(epochTicks * EORZEA_MULTIPLIER);

            return new DateTime(eorzeaTicks);



        }

        public delegate void EorzeaTimeUpdateHandler(object sender, EorzeaTimeEventArgs args);
        public event EorzeaTimeUpdateHandler OnEorzeaTimeUpdate;
        private void EorzeaTimeUpdate(EorzeaTimeEventArgs args)
        {
            if (this.OnEorzeaTimeUpdate != null)
                this.OnEorzeaTimeUpdate(this, args);
        }

        public delegate void LocalTimeUpdateHandler(object sender, EorzeaTimeEventArgs args);
        public event LocalTimeUpdateHandler OnLocalTimeUpdate;
        private void LocalTimeUpdate(EorzeaTimeEventArgs args)
        {
            if (this.OnLocalTimeUpdate != null)
                this.OnLocalTimeUpdate(this, args);
        }

        public delegate void UTCTimeUpdateHandler(object sender, EorzeaTimeEventArgs args);
        public event UTCTimeUpdateHandler OnUTCTimeUpdate;
        private void UTCTimeUpdate(EorzeaTimeEventArgs args)
        {
            if (this.OnUTCTimeUpdate != null)
                this.OnUTCTimeUpdate(this, args);
        }

        public TimeSpan EorzeaTimeSpan()
        {
            return new TimeSpan(this.CurrentEorzeaTime.Hour, this.CurrentEorzeaTime.Minute, this.CurrentEorzeaTime.Second);
        }

    }

    public class EorzeaTimeEventArgs : EventArgs
    {
        private DateTime _time;
        public DateTime Time
        {
            get { return this._time; }
            set { this._time = value; }
        }

        public EorzeaTimeEventArgs(DateTime time)
        {
            this.Time = time;
        }
    }
}
