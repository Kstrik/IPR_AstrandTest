using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ÄstrandTestFietsClient
{
    public class AvansAstrandTest
    {
        public enum State
        {
            NONE,
            WARMUP,
            TESTFASE1,
            TESTFASE2,
            COOLDOWN
        }

        private Timer timer;
        private State state;

        private int previousElapsed;
        private int elapsedSeconds;

        private int currentHeartrate;
        private List<int> heartrates;

        public AvansAstrandTest()
        {
            this.timer = new Timer();
            this.timer.Elapsed += Timer_Elapsed;
            this.state = State.NONE;

            this.previousElapsed = 0;
            this.elapsedSeconds = 0;

            this.heartrates = new List<int>();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.elapsedSeconds += e.SignalTime.Second - this.previousElapsed;

            switch (this.state)
            {
                case State.WARMUP:
                    {
                        this.elapsedSeconds = 0;
                        this.state = State.TESTFASE1;
                        this.timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                        break;
                    }
                case State.TESTFASE1:
                    {
                        this.heartrates.Add(this.currentHeartrate);

                        if(this.elapsedSeconds >= 120)
                        {
                            this.elapsedSeconds = 0;
                            this.state = State.TESTFASE2;
                            this.timer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
                        }
                        break;
                    }
                case State.TESTFASE2:
                    {
                        int heartrateReading = this.currentHeartrate;
                        this.heartrates.Add(heartrateReading);



                        if (this.elapsedSeconds >= 240)
                        {
                            this.elapsedSeconds = 0;
                            this.state = State.COOLDOWN;
                            this.timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                        }
                        break;
                    }
                case State.COOLDOWN:
                    {
                        this.elapsedSeconds = 0;
                        this.state = State.NONE;
                        this.timer.Stop();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            this.previousElapsed = e.SignalTime.Second;
        }

        public void Start()
        {
            this.timer.Interval = TimeSpan.FromMinutes(2).TotalMilliseconds;
            this.state = State.WARMUP;
        }

        public void OnHeartreaeReceived(int heartrate)
        {
            this.currentHeartrate = heartrate;
        }
    }
}
