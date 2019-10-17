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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIControls.Fields
{
    /// <summary>
    /// Interaction logic for TextField.xaml
    /// </summary>
    public partial class TextField : UserControl
    {
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(TextField));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TextField));
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(TextField));
        public static readonly DependencyProperty ValueBackgroundProperty = DependencyProperty.Register("ValueBackground", typeof(Brush), typeof(TextField));
        public static readonly DependencyProperty ValueBorderBrushProperty = DependencyProperty.Register("ValueBorderBrush", typeof(Brush), typeof(TextField));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TextField));

        public Brush HeaderForeground
        {
            get { return (Brush)this.GetValue(HeaderForegroundProperty); }
            set { this.SetValue(HeaderForegroundProperty, value); }
        }

        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        public Brush ValueForeground
        {
            get { return (Brush)this.GetValue(ValueForegroundProperty); }
            set { this.SetValue(ValueForegroundProperty, value); }
        }

        public Brush ValueBackground
        {
            get { return (Brush)this.GetValue(ValueBackgroundProperty); }
            set { this.SetValue(ValueBackgroundProperty, value); }
        }

        public Brush ValueBorderBrush
        {
            get { return (Brush)this.GetValue(ValueBorderBrushProperty); }
            set { this.SetValue(ValueBorderBrushProperty, value); }
        }

        public string Value
        {
            get { return (string)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public TextField()
        {
            InitializeComponent();
        }

        public void ApplyDarkTheme()
        {
            this.HeaderForeground = Brushes.White;
            this.ValueForeground = Brushes.White;
            this.ValueBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            this.ValueBorderBrush = Brushes.Transparent;
            this.FontSize = 12;
        }
    }
}
