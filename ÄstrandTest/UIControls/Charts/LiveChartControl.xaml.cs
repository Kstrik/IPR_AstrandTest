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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIControls.Charts
{
    /// <summary>
    /// Interaction logic for LiveChartControl.xaml
    /// </summary>
    public partial class LiveChartControl : UserControl
    {
        private LiveChart liveChart;

        public LiveChartControl(string chartHeader, string xHeader, string yHeader, double spacing, int width, int height, int stepsX, LiveChart.ColorTheme colorTheme,
                         bool resizeCanvasWithChart, bool showAxisX, bool showAxisY, bool showChartHeader, bool showAxisHeaderX, bool showAxisHeaderY, bool showGraphPoints)
        {
            InitializeComponent();
            this.liveChart = new LiveChart(chartHeader, xHeader, yHeader, spacing, width, height, stepsX, colorTheme, cnv_Canvas, resizeCanvasWithChart,
                                            showAxisX, showAxisY, showChartHeader, showAxisHeaderX, showAxisHeaderY, showGraphPoints);
        }

        public LiveChart GetLiveChart()
        {
            return this.liveChart;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            this.liveChart.OnMouseMove(e.GetPosition(cnv_Canvas));
        }
    }
}
