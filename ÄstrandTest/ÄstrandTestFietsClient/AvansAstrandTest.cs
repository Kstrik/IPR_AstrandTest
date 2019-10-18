using ÄstrandTestFietsClient.BikeCommunication;
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

        private int elapsedSeconds;

        private int currentHeartrate;
        private List<int> heartrates;

        private RealBike bike;
        private int currentResistance;

        private bool isMan;
        private int age;

        private int workload;
        private bool hasSteadyState;
        private double averageHeartrate;
        private double vo2;

        public AvansAstrandTest(RealBike bike, bool isMan, int age)
        {
            this.timer = new Timer();
            this.timer.Elapsed += Timer_Elapsed;
            this.state = State.NONE;

            this.elapsedSeconds = 0;

            this.heartrates = new List<int>();

            this.currentResistance = 10;
            this.bike = bike;

            this.isMan = isMan;
            this.age = age;

            this.workload = 0;
            this.hasSteadyState = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
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
                        this.elapsedSeconds += 60;
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
                        this.elapsedSeconds += 15;
                        int heartrateReading = this.currentHeartrate;
                        this.heartrates.Add(heartrateReading);

                        if (this.elapsedSeconds >= 240)
                        {
                            if(HasSteadyState(this.heartrates.GetRange(2, this.heartrates.Count - 2)))
                            {
                                this.hasSteadyState = true;
                                this.averageHeartrate = GetAverageHeartrate(this.heartrates.GetRange(2, this.heartrates.Count - 2));
                            }
                            this.heartrates.Clear();

                            this.elapsedSeconds = 0;
                            this.state = State.COOLDOWN;
                            this.timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                            this.currentResistance = 40;
                            this.bike.SetResistence((byte)this.currentResistance);
                        }
                        break;
                    }
                case State.COOLDOWN:
                    {
                        int workloadReading = this.workload;
                        
                        if (!this.hasSteadyState)
                            this.averageHeartrate = GetAverageHeartrate(this.heartrates);

                        if (this.isMan)
                            this.vo2 = (0.00212 * workloadReading + 0.299) / (0.769 * this.averageHeartrate - 48.5) * 1000;
                        else
                            this.vo2 = (0.00193 * workloadReading + 0.326) / (0.769 * this.averageHeartrate - 56.1) * 1000;

                        if (this.age >= 35)
                            this.vo2 *= GetAgeFactor();

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
        }

        public void Start()
        {
            //this.timer.Interval = TimeSpan.FromMinutes(2).TotalMilliseconds;
            this.bike.SetResistence((byte)this.currentResistance);
            this.timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            this.state = State.WARMUP;
            this.timer.Start();
        }

        public void Stop()
        {
            this.state = State.NONE;
            this.timer.Stop();
        }

        public void OnHeartrateReceived(int heartrate)
        {
            this.currentHeartrate = heartrate;
        }

        public void OnDistanceRecieved(int distance)
        {
            this.workload = distance;
        }

        public void OnCycleRyhthmReceived(int cycleRyhthm)
        {
            if(this.state == State.TESTFASE1)
            {
                if (cycleRyhthm >= 60)
                {
                    if (this.currentResistance < 65)
                        this.currentResistance += 5;
                    this.bike.SetResistence((byte)this.currentResistance);
                }
                else if(cycleRyhthm <= 50)
                {
                    if (this.currentResistance > 10)
                        this.currentResistance -= 5;
                    this.bike.SetResistence((byte)this.currentResistance);
                }
                else if(cycleRyhthm > 50 && cycleRyhthm < 60)
                {
                    if (this.currentResistance < 30)
                        this.currentResistance += 5;
                    else if (this.currentResistance > 30)
                        this.currentResistance -= 5;
                    this.bike.SetResistence((byte)this.currentResistance);
                }
            }
        }

        private bool HasSteadyState(List<int> heartrates)
        {
            int min = heartrates[0];
            int max = heartrates[0];

            foreach (int heartrate in heartrates)
            {
                if (heartrate > max)
                    max = heartrate;
                if (heartrate < min)
                    min = heartrate;
            }

            return ((max - min) <= 5);
        }

        private double GetAverageHeartrate(List<int> heartrates)
        {
            double total = 0;
            foreach (int heartrate in heartrates)
                total += heartrate;

            return total / heartrates.Count;
        }

        private double GetAgeFactor()
        {
            List<(int age, double factor)> factors = new List<(int age, double factor)>()
            {
                (15, 1.10),
                (25, 1.00),
                (35, 0.87),
                (40, 0.83),
                (45, 0.78),
                (50, 0.75),
                (55, 0.71),
                (60, 0.68),
                (65, 0.65)
            };

            double factor = 0;
            foreach((int age, double factor) ageFactor in factors)
            {
                if (this.age >= ageFactor.age)
                    factor = ageFactor.factor;
                else
                    break;
            }

            return factor;
        }
    }
}
