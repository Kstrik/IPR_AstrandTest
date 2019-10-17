using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Interaction logic for VectorField.xaml
    /// </summary>
    public partial class Vector3Field : UserControl
    {
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(Vector3Field));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Vector3Field));
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(Vector3Field));
        public static readonly DependencyProperty ValueBackgroundProperty = DependencyProperty.Register("ValueBackground", typeof(Brush), typeof(Vector3Field));
        public static readonly DependencyProperty ValueBorderBrushProperty = DependencyProperty.Register("ValueBorderBrush", typeof(Brush), typeof(Vector3Field));
        public static readonly DependencyProperty ValueXProperty = DependencyProperty.Register("X", typeof(float), typeof(Vector3Field));
        public static readonly DependencyProperty ValueYProperty = DependencyProperty.Register("Y", typeof(float), typeof(Vector3Field));
        public static readonly DependencyProperty ValueZProperty = DependencyProperty.Register("Z", typeof(float), typeof(Vector3Field));

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

        public float X
        {
            get { return (float)this.GetValue(ValueXProperty); }
            set { this.SetValue(ValueXProperty, value); }
        }

        public float Y
        {
            get { return (float)this.GetValue(ValueYProperty); }
            set { this.SetValue(ValueYProperty, value); }
        }

        public float Z
        {
            get { return (float)this.GetValue(ValueZProperty); }
            set { this.SetValue(ValueZProperty, value); }
        }

        public Vector3Field()
        {
            InitializeComponent();
        }

        public Vector3 GetVector3()
        {
            return new Vector3(this.X, this.Y, this.Z);
        }

        public void SetVector3(Vector3 vector3)
        {
            if (vector3 != null)
            {
                this.X = vector3.X;
                this.Y = vector3.Y;
                this.Z = vector3.Z;
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
