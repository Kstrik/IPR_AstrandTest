using System;
using System.Collections.Generic;
using System.IO;
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

namespace UIControls.Fields
{
    /// <summary>
    /// Interaction logic for ImageSelectionField.xaml
    /// </summary>
    public partial class ImageSelectionField : UserControl
    {
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(ImageSelectionField));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ImageSelectionField));
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(ImageSelectionField));
        public static readonly DependencyProperty ValueBackgroundProperty = DependencyProperty.Register("ValueBackground", typeof(Brush), typeof(ImageSelectionField));
        public static readonly DependencyProperty ValueBorderBrushProperty = DependencyProperty.Register("ValueBorderBrush", typeof(Brush), typeof(ImageSelectionField));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(IEnumerable<object>), typeof(ImageSelectionField), null);
        public static readonly DependencyProperty MaxImageSizeProperty = DependencyProperty.Register("MaxImageSize", typeof(double), typeof(ImageSelectionField), null);

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

        public IEnumerable<object> Value
        {
            get { return (IEnumerable<object>)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public double MaxImageSize
        {
            get { return (double)this.GetValue(MaxImageSizeProperty); }
            set { this.SetValue(MaxImageSizeProperty, value); }
        }

        public object SelectedValue
        {
            get { return cmbValue.SelectedItem; }
            set { cmbValue.SelectedItem = value; }
        }

        public Dictionary<string, string> Dictionary;

        public ImageSelectionField()
        {
            InitializeComponent();
        }

        private void CmbValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filepath = "";
            if (this.Dictionary != null)
                this.Dictionary.TryGetValue(cmbValue.SelectedItem.ToString(), out filepath);
            else
                filepath = cmbValue.SelectedItem.ToString();

            if (!String.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                imgImage.Source = new BitmapImage(new Uri(filepath, UriKind.Absolute));
            }
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
