using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UIControls.Charts
{
    public class LiveChart
    {
        public class ColorTheme
        {
            public SolidColorBrush PrimaryColor;
            public SolidColorBrush SecondaryColor;
            public SolidColorBrush LineColor;
            public SolidColorBrush BackgroundColor;
            public SolidColorBrush StrokeColor;

            public ColorTheme(SolidColorBrush primaryColor, SolidColorBrush secondaryColor, SolidColorBrush lineColor, SolidColorBrush backgroundColor, SolidColorBrush strokeColor)
            {
                this.PrimaryColor = primaryColor;
                this.SecondaryColor = secondaryColor;
                this.LineColor = lineColor;
                this.BackgroundColor = backgroundColor;
                this.StrokeColor = strokeColor;
            }
        }

        public double Spacing { get { return this.spacing; } set { this.spacing = value; } }
        public int Width { get { return this.width; } set { this.width = value; } }
        public int Height { get { return this.height; } set { this.height = value; } }

        private string chartHeader;
        private string xHeader;
        private string yHeader;

        private double spacing;
        private int width;
        private int height;

        private bool showAxisX;
        private bool showAxisY;
        private bool showChartHeader;
        private bool showAxisHeaderX;
        private bool showAxisHeaderY;

        private ColorTheme colorTheme;
        private Canvas canvas;

        private Polygon polygon;
        private Line indicatorLine;
        private Label indicatorLabel;
        private LinearGradientBrush gradientBrush;

        private List<double> data;
        private int stepsX;
        private double greatestValue;

        private List<GraphPoint> graphPoints = new List<GraphPoint>();
        private bool showGraphPoints;

        public LiveChart(string chartHeader, string xHeader, string yHeader, double spacing, int width, int height, int stepsX, ColorTheme colorTheme, Canvas canvas,
                         bool resizeCanvasWithChart, bool showAxisX, bool showAxisY, bool showChartHeader, bool showAxisHeaderX, bool showAxisHeaderY, bool showGraphPoints)
        {
            this.chartHeader = chartHeader;
            this.xHeader = xHeader;
            this.yHeader = yHeader;
            this.spacing = spacing;
            this.width = width;
            this.height = height;

            this.colorTheme = colorTheme;
            this.canvas = canvas;

            this.showAxisX = showAxisX;
            this.showAxisY = showAxisY;
            this.showChartHeader = showChartHeader;
            this.showAxisHeaderX = showAxisHeaderX;
            this.showAxisHeaderY = showAxisHeaderY;

            this.canvas.Background = colorTheme.BackgroundColor;

            this.polygon = new Polygon();
            SetupGradient();
            this.polygon.Fill = this.gradientBrush;
            this.polygon.Stroke = this.colorTheme.StrokeColor;
            this.canvas.Children.Add(this.polygon);
            SetupIndicators();

            this.data = new List<double>();
            this.stepsX = stepsX;
            this.greatestValue = 0;

            if (resizeCanvasWithChart)
                ResizeCanvas();

            SetupChart();

            this.showGraphPoints = showGraphPoints;
        }

        public void OnMouseMove(Point mousePosition)
        {
            if(this.showGraphPoints)
            {
                foreach (GraphPoint graphPoint in this.graphPoints)
                {
                    graphPoint.OnMouseMoved(mousePosition);
                }
            }
        }

        private void SetupIndicators()
        {
            this.indicatorLine = new Line();
            this.indicatorLine.Stroke = this.colorTheme.LineColor;
            this.indicatorLine.StrokeThickness = 1;
            this.canvas.Children.Add(this.indicatorLine);

            this.indicatorLabel = new Label();
            this.indicatorLabel.Foreground = this.colorTheme.LineColor;
            this.indicatorLabel.FontSize = 12;
            this.indicatorLabel.HorizontalAlignment = HorizontalAlignment.Left;
            this.indicatorLabel.VerticalAlignment = VerticalAlignment.Top;
            this.canvas.Children.Add(this.indicatorLabel);
        }

        private void SetupGradient()
        {
            GradientStopCollection gradientStopCollection = new GradientStopCollection();
            gradientStopCollection.Add(new GradientStop(this.colorTheme.PrimaryColor.Color, 0));
            gradientStopCollection.Add(new GradientStop(this.colorTheme.SecondaryColor.Color, 1));

            this.gradientBrush = new LinearGradientBrush(gradientStopCollection) { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
        }

        public void Update(double data)
        {
            data = Math.Round(data, 2);

            this.data.Add(data);
            if (this.data.Count > this.stepsX)
            {
                this.data.RemoveAt(0);

                if (this.showGraphPoints)
                {
                    this.graphPoints[0].Remove();
                    this.graphPoints.RemoveAt(0);
                }
            }

            this.greatestValue = this.data.Max();

            if (this.showGraphPoints)
            {
                double lengthOfStepX = this.width / this.stepsX;
                lengthOfStepX += lengthOfStepX / this.stepsX;
                Point point = new Point();
                point.X = lengthOfStepX * (this.data.Count - 1) + this.spacing;
                point.Y = this.height + this.spacing - (this.height / this.greatestValue * data);
                GraphPoint graphPoint = new GraphPoint(point, data, 3, ref this.canvas);
                this.graphPoints.Add(graphPoint);
            }


            this.polygon.Points = GetPointsFromData();
            UpdateIndicators(data);
        }

        private PointCollection GetPointsFromData()
        {
            PointCollection points = new PointCollection();
            double lengthOfStepX = this.width / this.stepsX;
            lengthOfStepX += lengthOfStepX / this.stepsX;

            for (int i = 0; i < this.data.Count; i++)
            {
                Point point = new Point();
                point.X = lengthOfStepX * i + this.spacing;
                point.Y = this.height + this.spacing - (this.height / this.greatestValue * this.data[i]);
                points.Add(point);

                if (this.showGraphPoints)
                    this.graphPoints[i].Position = point;
            }

            points.Add(new Point(((this.width / this.stepsX + lengthOfStepX / this.stepsX) * (this.data.Count - 1)) + this.spacing, this.height + this.spacing));
            points.Add(new Point(this.spacing, this.height + this.spacing));

            return points;
        }

        private void UpdateIndicators(double data)
        {
            double newY = this.height + this.spacing - (this.height / this.greatestValue * data);
            this.indicatorLine.X1 = this.spacing;
            this.indicatorLine.Y1 = newY;
            this.indicatorLine.X2 = this.width + this.spacing;
            this.indicatorLine.Y2 = newY;

            this.indicatorLabel.Content = data;
            this.indicatorLabel.Margin = new Thickness(this.indicatorLine.X2, this.indicatorLine.Y1 - 12, 0, 0);
        }

        private void SetupChart()
        {
            SetupAxis();
            SetupHeaders();
        }

        private void SetupAxis()
        {
            if (this.showAxisX)
            {
                Line lineX = new Line();
                lineX.X1 = this.spacing;
                lineX.Y1 = this.height + this.spacing;
                lineX.X2 = this.width + this.spacing;
                lineX.Y2 = this.height + this.spacing;
                lineX.Stroke = this.colorTheme.LineColor;

                this.canvas.Children.Add(lineX);
            }

            if (this.showAxisY)
            {
                Line lineY = new Line();
                lineY.X1 = this.spacing;
                lineY.Y1 = this.spacing;
                lineY.X2 = this.spacing;
                lineY.Y2 = this.height + this.spacing;
                lineY.Stroke = this.colorTheme.LineColor;

                this.canvas.Children.Add(lineY);
            }
        }

        private void SetupHeaders()
        {
            if (this.showChartHeader)
            {
                TextBlock headerChart = new TextBlock();
                headerChart.Text = this.chartHeader;
                headerChart.FontSize = 12;
                headerChart.Margin = new System.Windows.Thickness(((this.width / 2) + (this.spacing / 2)) - (headerChart.ActualWidth / 2), (this.spacing / 2) - (headerChart.FontSize / 2), 0, 0);
                headerChart.Background = Brushes.Transparent;
                headerChart.Foreground = this.colorTheme.LineColor;

                this.canvas.Children.Add(headerChart);
            }

            if (this.showAxisHeaderX)
            {
                TextBlock headerX = new TextBlock();
                headerX.Text = this.xHeader;
                headerX.FontSize = 12;
                headerX.Margin = new System.Windows.Thickness(((this.width / 2) + (this.spacing / 2)) - (headerX.ActualWidth / 2), (this.height + (this.spacing * 2)) - ((this.spacing / 2) + (headerX.FontSize / 2)), 0, 0);
                headerX.Background = Brushes.Transparent;
                headerX.Foreground = this.colorTheme.LineColor;

                this.canvas.Children.Add(headerX);
            }

            if (this.showAxisHeaderY)
            {
                TextBlock headerY = new TextBlock();
                headerY.Text = this.yHeader;
                headerY.FontSize = 12;
                headerY.LayoutTransform = new RotateTransform(-90);
                headerY.Margin = new System.Windows.Thickness((this.spacing / 2) - (headerY.FontSize / 2), (this.height / 2) + (this.spacing / 2), 0, 0);
                headerY.Background = Brushes.Transparent;
                headerY.Foreground = this.colorTheme.LineColor;

                this.canvas.Children.Add(headerY);
            }
        }

        private void ResizeCanvas()
        {
            this.canvas.Width = this.width + (this.spacing * 2);
            this.canvas.Height = this.height + (this.spacing * 2);
        }

        public void Clear()
        {
            this.canvas.Children.Clear();
        }

        public static ColorTheme BlueTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                Brushes.Transparent);

        public static ColorTheme GreenTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF047C50")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF047C50")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        Brushes.Transparent);

        public static ColorTheme RedTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB93838")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB93838")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        Brushes.Transparent);

        public static ColorTheme OrangeTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF9700")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF9700")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        Brushes.Transparent);

        public static ColorTheme YellowTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFE800")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFE800")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        Brushes.Transparent);

        public static ColorTheme BlueGreenTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF047C50")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                        Brushes.Transparent);

        public static ColorTheme BlueRedTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB93838")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
                                                                Brushes.Transparent);

        public static ColorTheme BlueGreenDarkTheme = new ColorTheme(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF047C50")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")),
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E")),
                                                                Brushes.Transparent);
    }
}
