using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UIControls.Charts;

namespace ÄstrandTestSpecialistClient.UI
{
    public class TestDataControl : ContentControl
    {
        public static readonly DependencyProperty HeartrateProperty = DependencyProperty.Register("Heartrate", typeof(int), typeof(TestDataControl));
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(int), typeof(TestDataControl));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(TestDataControl));
        public static readonly DependencyProperty CycleRhythmProperty = DependencyProperty.Register("CycleRhythm", typeof(int), typeof(TestDataControl));

        public int Heartrate
        {
            get { return (int)this.GetValue(HeartrateProperty); }
            set { this.SetValue(HeartrateProperty, value); }
        }

        public int Distance
        {
            get { return (int)this.GetValue(DistanceProperty); }
            set { this.SetValue(DistanceProperty, value); }
        }

        public int Speed
        {
            get { return (int)this.GetValue(SpeedProperty); }
            set { this.SetValue(SpeedProperty, value); }
        }

        public int CycleRhythm
        {
            get { return (int)this.GetValue(CycleRhythmProperty); }
            set { this.SetValue(CycleRhythmProperty, value); }
        }

        private Grid grid;

        private StackPanel detailsPanel;
        private Label heartrateLabel;
        private Label heartrateDisplay;
        private Label distanceLabel;
        private Label distanceDisplay;
        private Label speedLabel;
        private Label speedDisplay;
        private Label cycleRhythmLabel;
        private Label cycleRhythmDisplay;

        private LiveChartControl liveChartControl;

        public string Username;

        public TestDataControl()
        {
            this.DataContext = this;

            this.grid = new Grid();
            ColumnDefinition detailsColumn = new ColumnDefinition();
            detailsColumn.Width = new GridLength(200);
            this.grid.ColumnDefinitions.Add(detailsColumn);
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            this.grid.RowDefinitions.Add(new RowDefinition());

            SetupLabels();

            this.detailsPanel = new StackPanel();
            this.detailsPanel.Children.Add(this.heartrateLabel);
            this.detailsPanel.Children.Add(this.heartrateDisplay);
            this.detailsPanel.Children.Add(this.distanceLabel);
            this.detailsPanel.Children.Add(this.distanceDisplay);
            this.detailsPanel.Children.Add(this.speedLabel);
            this.detailsPanel.Children.Add(this.speedDisplay);
            this.detailsPanel.Children.Add(this.cycleRhythmLabel);
            this.detailsPanel.Children.Add(this.cycleRhythmDisplay);

            this.liveChartControl = new LiveChartControl("Hartslag", "", "", 40, 400, 200, 20, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);

            this.grid.Children.Add(this.detailsPanel);
            this.grid.Children.Add(this.liveChartControl);

            BindingOperations.SetBinding(this.heartrateDisplay, Label.ContentProperty, new Binding("Heartrate"));
            BindingOperations.SetBinding(this.distanceDisplay, Label.ContentProperty, new Binding("Distance"));
            BindingOperations.SetBinding(this.speedDisplay, Label.ContentProperty, new Binding("Speed"));
            BindingOperations.SetBinding(this.cycleRhythmDisplay, Label.ContentProperty, new Binding("CycleRhythm"));

            Grid.SetColumn(this.detailsPanel, 0);
            Grid.SetColumn(this.liveChartControl, 1);
            Grid.SetRow(this.detailsPanel, 0);
            Grid.SetRow(this.liveChartControl, 0);

            this.Content = this.grid;
        }

        public TestDataControl(string username)
            : this()
        {
            this.Username = username;
        }

        private void SetupLabels()
        {
            this.heartrateLabel = new Label();
            this.heartrateLabel.FontSize = 12;
            this.heartrateLabel.Margin = new Thickness(5, 5, 0, 0);
            this.heartrateLabel.Content = "Hartslag:";
            BindingOperations.SetBinding(this.heartrateLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.heartrateDisplay = new Label();
            this.heartrateDisplay.FontSize = 12;
            this.heartrateDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.heartrateDisplay, Label.ForegroundProperty, new Binding("Foreground"));

            this.distanceLabel = new Label();
            this.distanceLabel.FontSize = 12;
            this.distanceLabel.Margin = new Thickness(5, 5, 0, 0);
            this.distanceLabel.Content = "Afstand:";
            BindingOperations.SetBinding(this.distanceLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.distanceDisplay = new Label();
            this.distanceDisplay.FontSize = 12;
            this.distanceDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.distanceDisplay, Label.ForegroundProperty, new Binding("Foreground"));

            this.speedLabel = new Label();
            this.speedLabel.FontSize = 12;
            this.speedLabel.Margin = new Thickness(5, 5, 0, 0);
            this.speedLabel.Content = "Snelheid:";
            BindingOperations.SetBinding(this.speedLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.speedDisplay = new Label();
            this.speedDisplay.FontSize = 12;
            this.speedDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.speedDisplay, Label.ForegroundProperty, new Binding("Foreground"));

            this.cycleRhythmLabel = new Label();
            this.cycleRhythmLabel.FontSize = 12;
            this.cycleRhythmLabel.Margin = new Thickness(5, 5, 0, 0);
            this.cycleRhythmLabel.Content = "Rotaties per minuut:";
            BindingOperations.SetBinding(this.cycleRhythmLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.cycleRhythmDisplay = new Label();
            this.cycleRhythmDisplay.FontSize = 12;
            this.cycleRhythmDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.cycleRhythmDisplay, Label.ForegroundProperty, new Binding("Foreground"));
        }

        public void UpdateChart(double value)
        {
            this.liveChartControl.GetLiveChart().Update(value);
        }
    }
}
