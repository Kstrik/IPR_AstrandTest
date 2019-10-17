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

namespace UIControls.Menu
{
    /// <summary>
    /// Interaction logic for SidemenuWindow.xaml
    /// </summary>
    public partial class SidemenuWindow : UserControl
    {
        public Sidemenu Sidemenu;
        private Window parentWindow;

        public SidemenuWindow(Sidemenu sidemenu)
        {
            InitializeComponent();

            this.Sidemenu = sidemenu;

            this.Loaded += new RoutedEventHandler(UserControl_Loaded);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.parentWindow = Window.GetWindow(this);
            this.parentWindow.SizeChanged += ParentWindow_SizeChanged;
            grd_Grid.Children.Add(this.Sidemenu);
            stk_Content.Width = parentWindow.Width - this.Sidemenu.GetMenuBarWidth() - 16;
            stk_Content.Margin = new Thickness(this.Sidemenu.GetMenuBarWidth(), 0, 0, 0);
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            stk_Content.Width = e.NewSize.Width - this.Sidemenu.GetMenuBarWidth() - 16;
            stk_Content.Height = e.NewSize.Height;
        }

        public StackPanel GetContentPanel()
        {
            return stk_Content;
        }
    }
}
