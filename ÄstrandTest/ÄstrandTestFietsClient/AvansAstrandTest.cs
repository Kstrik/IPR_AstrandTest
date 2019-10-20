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

        private Timer displayTimer;
        private TimeSpan timeSpan;

        private Timer testTimer;
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

        public bool IsRunning;
        private IAvansAstrandTestConnector connector;

        public AvansAstrandTest(RealBike bike, bool isMan, int age, IAvansAstrandTestConnector connector)
        {
            this.displayTimer = new Timer(1000);
            this.displayTimer.Elapsed += DisplayTimer_Elapsed;
            this.timeSpan = new TimeSpan(0, 7, 0);

            this.testTimer = new Timer();
            this.testTimer.Elapsed += Timer_Elapsed;
            this.state = State.NONE;

            this.elapsedSeconds = 0;

            this.heartrates = new List<int>();

            this.currentResistance = 10;
            this.bike = bike;

            this.isMan = isMan;
            this.age = age;

            this.workload = 0;
            this.hasSteadyState = false;

            this.connector = connector;
            this.connector?.OnAstrandTestLogStateAndCountdown(this.state, this.timeSpan.ToString());
            this.connector?.OnAstrandTestSetResistance(this.currentResistance);
        }

        private void DisplayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timeSpan -= new TimeSpan(0, 0, 1);
            this.connector?.OnAstrandTestLogStateAndCountdown(this.state, this.timeSpan.ToString());
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            switch (this.state)
            {
                case State.WARMUP:
                    {
                        this.elapsedSeconds = 0;
                        this.state = State.TESTFASE1;
                        this.testTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                        break;
                    }
                case State.TESTFASE1:
                    {
                        this.elapsedSeconds += 60;
                        this.heartrates.Add(this.currentHeartrate);
                        this.connector?.OnAstrandTestLastHeartrateMeasured(this.heartrates.Last());

                        if (this.elapsedSeconds >= 120)
                        {
                            this.elapsedSeconds = 0;
                            this.state = State.TESTFASE2;
                            this.testTimer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
                        }
                        break;
                    }
                case State.TESTFASE2:
                    {
                        this.elapsedSeconds += 15;
                        int heartrateReading = this.currentHeartrate;
                        this.heartrates.Add(heartrateReading);
                        this.connector?.OnAstrandTestLastHeartrateMeasured(this.heartrates.Last());

                        if (this.elapsedSeconds >= 120)
                        {
                            if(HasSteadyState(this.heartrates.GetRange(2, this.heartrates.Count - 2)))
                            {
                                this.hasSteadyState = true;
                                //this.averageHeartrate = GetAverageHeartrate(this.heartrates.GetRange(2, this.heartrates.Count - 2));
                                this.connector?.OnAstrandTestSteadyStateReached();
                            }
                            this.averageHeartrate = GetAverageHeartrate(this.heartrates.GetRange(2, this.heartrates.Count - 2));
                            this.heartrates.Clear();

                            this.elapsedSeconds = 0;
                            this.state = State.COOLDOWN;
                            this.testTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                            this.currentResistance = 40;
                            this.bike?.SetResistence((byte)this.currentResistance);
                            this.connector?.OnAstrandTestSetResistance(this.currentResistance);
                        }
                        break;
                    }
                case State.COOLDOWN:
                    {
                        int workloadReading = this.workload;
                       

                        if (this.isMan)
                            this.vo2 = (0.00212 * workloadReading + 0.299) / (0.769 * this.averageHeartrate - 48.5) * 1000;
                        else
                            this.vo2 = (0.00193 * workloadReading + 0.326) / (0.769 * this.averageHeartrate - 56.1) * 1000;

                        if (this.age >= 35)
                            this.vo2 *= GetAgeFactor();

                        this.elapsedSeconds = 0;
                        this.IsRunning = false;
                        this.state = State.NONE;
                        this.displayTimer.Stop();
                        this.testTimer.Stop();
                        this.connector?.OnAstrandTestEnd(this.hasSteadyState, this.vo2);
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
            if(!this.IsRunning)
            {
                this.IsRunning = true;
                this.bike?.SetResistence((byte)this.currentResistance);
                this.testTimer.Interval = TimeSpan.FromMinutes(2).TotalMilliseconds;
                this.state = State.WARMUP;
                this.displayTimer.Start();
                this.testTimer.Start();
                this.connector?.OnAstrandTestStart();
            }
        }

        public void Stop()
        {
            if(this.IsRunning)
            {
                this.elapsedSeconds = 0;
                this.IsRunning = false;
                this.state = State.NONE;
                this.displayTimer.Stop();
                this.testTimer.Stop();
                this.connector?.OnAstrandTestAbort();
            }
        }

        public void OnHeartrateReceived(int heartrate)
        {
            this.currentHeartrate = heartrate;

            // Change resistance in relation with the heartbeats per minute
            if (this.state == State.TESTFASE1)
            {
                if (this.currentHeartrate < 130 && this.currentResistance != 100)
                {
                    this.currentResistance += 1;
                    this.bike?.SetResistence((byte)this.currentResistance);
                    this.connector?.OnAstrandTestSetResistance(this.currentResistance);
                }
            }
        }

        public void OnDistanceRecieved(int distance)
        {
            this.workload = distance;
        }

        public void OnCycleRyhthmReceived(int cycleRyhthm)
        {
            if (this.state == State.TESTFASE1 || this.state == State.TESTFASE2)
            {
                if (cycleRyhthm >= 60)
                    this.connector?.OnAstrandTestToFast();
                else if (cycleRyhthm <= 50)
                    this.connector?.OnAstrandTestToSlow();
                else if (cycleRyhthm > 50 && cycleRyhthm < 60)
                    this.connector?.OnAstrandTestGoodSpeed();
            }

            // Change resistance in relation with the cyleryhthm
            //if(this.state == State.TESTFASE1 || this.state == State.TESTFASE2)
            //{
            //    if (cycleRyhthm >= 60)
            //    {
            //        if (this.currentResistance < 65)
            //            this.currentResistance += 5;
            //        this.bike?.SetResistence((byte)this.currentResistance);
            //        this.connector?.OnAstrandTestToFast();
            //        if(this.state == State.TESTFASE1)
            //            this.connector?.OnAstrandTestSetResistance(this.currentResistance);
            //    }
            //    else if(cycleRyhthm <= 50)
            //    {
            //        if (this.currentResistance > 10)
            //            this.currentResistance -= 5;
            //        this.bike?.SetResistence((byte)this.currentResistance);
            //        this.connector?.OnAstrandTestToSlow();
            //        if (this.state == State.TESTFASE1)
            //            this.connector?.OnAstrandTestSetResistance(this.currentResistance);
            //    }
            //    else if(cycleRyhthm > 50 && cycleRyhthm < 60)
            //    {
            //        if (this.currentResistance < 30)
            //            this.currentResistance += 5;
            //        else if (this.currentResistance > 30)
            //            this.currentResistance -= 5;
            //        this.bike?.SetResistence((byte)this.currentResistance);
            //        this.connector?.OnAstrandTestGoodSpeed();
            //        if (this.state == State.TESTFASE1)
            //            this.connector?.OnAstrandTestSetResistance(this.currentResistance);
            //    }
            //}
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
