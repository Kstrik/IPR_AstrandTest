using ÄstrandTestServer.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UIControls.Charts;

namespace ÄstrandTestSpecialistClient
{
    /// <summary>
    /// Interaction logic for TestDataWindow.xaml
    /// </summary>
    public partial class TestDataWindow : Window
    {
        private LiveChartControl heartrateChart;
        private LiveChartControl distanceChart;
        private LiveChartControl speedChart;
        private LiveChartControl cycleRhythmChart;

        private ÄstrandTest testData;

        private int maxIntervals;

        private string username;
        private int birthYear;
        private int weight;
        private bool isMan;

        private bool hasSteadyState;
        private double vo2;

        public TestDataWindow(string username, int birthYear, int weight, bool isMan, int maxIntervals, bool hasSteadyState, double vo2)
        {
            InitializeComponent();

            this.username = username;
            this.birthYear = birthYear;
            this.weight = weight;
            this.isMan = isMan;

            this.hasSteadyState = hasSteadyState;
            this.vo2 = vo2;

            this.testData = new ÄstrandTest();
            this.maxIntervals = maxIntervals;
            con_TestData.Header = $"Cliënt data [{username}]";

            this.heartrateChart = new LiveChartControl("Hartslag", "", "sl/pm", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, true, true);
            this.distanceChart = new LiveChartControl("Afstand", "", "meters", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, true, true);
            this.speedChart = new LiveChartControl("Snelheid", "", "m/s", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, true, true);
            this.cycleRhythmChart = new LiveChartControl("Rotaties per minuut", "", "rpm", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, true, true);

            grd_Grid.Children.Add(this.heartrateChart);
            grd_Grid.Children.Add(this.distanceChart);
            grd_Grid.Children.Add(this.speedChart);
            grd_Grid.Children.Add(this.cycleRhythmChart);

            Grid.SetColumn(this.heartrateChart, 0);
            Grid.SetColumn(this.distanceChart, 1);
            Grid.SetColumn(this.speedChart, 0);
            Grid.SetColumn(this.cycleRhythmChart, 1);
            Grid.SetRow(this.heartrateChart, 0);
            Grid.SetRow(this.distanceChart, 0);
            Grid.SetRow(this.speedChart, 1);
            Grid.SetRow(this.cycleRhythmChart, 1);

            lbl_Username.Content = username;
            lbl_BirthYear.Content = birthYear;
            lbl_Weight.Content = weight;
            lbl_Gender.Content = (isMan) ? "Man" : "Vrouw";
            lbl_SteadyState.Content = (hasSteadyState) ? "JA" : "NEE";
            lbl_VO2.Content = vo2;

            if (!hasSteadyState)
                lbl_SteadyStateNotification.Content = "Er was geen steady state behaald, VO2max waarde is niet betrouwbaar!";
        }

        public void ProcessHistoryData()
        {
            List<(LiveChartControl, List<(int value, DateTime time)>)> dataHisory = new List<(LiveChartControl, List<(int value, DateTime time)>)>()
            {
                (this.heartrateChart, this.testData.HeartrateValues),
                (this.distanceChart, this.testData.DistanceValues),
                (this.speedChart, this.testData.SpeedValues),
                (this.cycleRhythmChart, this.testData.CycleRhythmValues)
            };

            foreach ((LiveChartControl livechart, List<(int value, DateTime time)> history) data in dataHisory)
            {
                int stepSize = (data.history.Count() >= this.maxIntervals) ? data.history.Count() / this.maxIntervals : 1;
                int stepAmount = data.history.Count() / stepSize;

                for (int i = 0; i < data.history.Count; i += stepSize)
                {
                    if (data.history.Count > (i + stepAmount))
                        data.livechart.GetLiveChart().Update(GetAverage(data.history.GetRange(i, stepAmount)));
                    else
                        data.livechart.GetLiveChart().Update(GetAverage(data.history.GetRange(i, data.history.Count - i)));
                }
            }
        }

        private double GetAverage(List<(int value, DateTime time)> values)
        {
            double total = 0;
            foreach ((int value, DateTime time) item in values)
                total += item.value;

            return total / values.Count;
        }

        public void AddHeartRate((int value, DateTime time) heartrate)
        {
            this.testData.HeartrateValues.Add(heartrate);
        }

        public void AddDistance((int value, DateTime time) distance)
        {
            this.testData.DistanceValues.Add(distance);
        }

        public void AddSpeed((int value, DateTime time) speed)
        {
            this.testData.SpeedValues.Add(speed);
        }

        public void AddCycleRyhthm((int value, DateTime time) cycleRhythm)
        {
            this.testData.CycleRhythmValues.Add(cycleRhythm);
        }
    }
}
