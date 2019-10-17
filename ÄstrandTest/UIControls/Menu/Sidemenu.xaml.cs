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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIControls.Menu
{
    /// <summary>
    /// Interaction logic for Sidemenu.xaml
    /// </summary>
    public partial class Sidemenu : UserControl
    {
        public static readonly DependencyProperty ContentWidthProperty = DependencyProperty.Register("ContentWidth", typeof(int), typeof(Sidemenu));
        public static readonly DependencyProperty MenuBarWidthProperty = DependencyProperty.Register("MenuBarWidth", typeof(int), typeof(Sidemenu));
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register("AnimationDuration", typeof(int), typeof(Sidemenu));
        public static readonly DependencyProperty MenuBarBackgroundProperty = DependencyProperty.Register("MenuBarBackground", typeof(Brush), typeof(Sidemenu));
        public static readonly DependencyProperty HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(Sidemenu));
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(Sidemenu));

        public int ContentWidth
        {
            get { return (int)this.GetValue(ContentWidthProperty); }
            set { this.SetValue(ContentWidthProperty, value); }
        }

        public int MenuBarWidth
        {
            get { return (int)this.GetValue(MenuBarWidthProperty); }
            set { this.SetValue(MenuBarWidthProperty, value); }
        }

        public int AnimationDuration
        {
            get { return (int)this.GetValue(AnimationDurationProperty); }
            set { this.SetValue(AnimationDurationProperty, value); }
        }

        public Brush MenuBarBackground
        {
            get { return (Brush)this.GetValue(MenuBarBackgroundProperty); }
            set { this.SetValue(MenuBarBackgroundProperty, value); }
        }

        public Brush HoverForeground
        {
            get { return (Brush)this.GetValue(HoverForegroundProperty); }
            set { this.SetValue(HoverForegroundProperty, value); }
        }

        public Brush HoverBackground
        {
            get { return (Brush)this.GetValue(HoverBackgroundProperty); }
            set { this.SetValue(HoverBackgroundProperty, value); }
        }

        public Menu Menu;
        private Window parentWindow;

        private ThicknessAnimation slideAnimation;
        private Storyboard storyBoard;

        private bool isOpen;

        public Sidemenu()
        {
            InitializeComponent();
        }

        //public Sidemenu(int contentWidth, int menuBarWidth, int animationDuration, SolidColorBrush backgroundColor, SolidColorBrush foregroundColor, SolidColorBrush hoverBackgroundColor, SolidColorBrush hoverForegroundColor)
        //{
        //    InitializeComponent();

        //    this.Menu = new Menu(contentWidth, 1080, backgroundColor, foregroundColor, hoverBackgroundColor, hoverForegroundColor);
        //    this.ContentWidth = contentWidth;
        //    this.MenuBarWidth = menuBarWidth;
        //    this.AnimationDuration = animationDuration;

        //    this.slideAnimation = new ThicknessAnimation();
        //    this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
        //    Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Sidemenu.MarginProperty));
        //    this.storyBoard = new Storyboard();
        //    this.storyBoard.Children.Add(this.slideAnimation);

        //    this.isOpen = false;

        //    this.Loaded += new RoutedEventHandler(UserControl_Loaded);
        //}

        private void Sidemenu_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Menu = new Menu(this.ContentWidth, 1080, (SolidColorBrush)this.Foreground, (SolidColorBrush)this.Background, (SolidColorBrush)this.HoverBackground, (SolidColorBrush)this.HoverForeground);
            this.slideAnimation = new ThicknessAnimation();
            this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(this.AnimationDuration));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Sidemenu.MarginProperty));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Sidemenu.MarginProperty));
            this.storyBoard = new Storyboard();
            this.storyBoard.Children.Add(this.slideAnimation);
            this.isOpen = false;

            this.parentWindow = Window.GetWindow(this);
            this.parentWindow.SizeChanged += ParentWindow_SizeChanged;

            scv_Content.Height = this.parentWindow.Height;

            this.Margin = new Thickness(-this.ContentWidth, 0, 0, 0);
            //stk_Content.Children.Add(this.Menu.GetUIComponent());
        }

        public void SetupMenu()
        {
            this.Menu = new Menu(this.ContentWidth, 1080, (SolidColorBrush)this.Background, (SolidColorBrush)this.Foreground, (SolidColorBrush)this.HoverBackground, (SolidColorBrush)this.HoverForeground);
            stk_Content.Children.Add(this.Menu.GetUIComponent());
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = e.NewSize.Height;
            scv_Content.Height = this.Height;
            //stk_Content.Height = this.Height;
            stk_MenuBar.Height = this.Height;
            //grd_Grid.Width = this.ContentWidth + this.MenuBarWidth;
        }

        private void Sidemenu_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = e.NewSize.Height;
        }

        private void MenuToggle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(!this.isOpen)
            {
                this.slideAnimation.From = new Thickness(-this.ContentWidth, 0, 0, 0);
                this.slideAnimation.To = new Thickness(0, 0, 0, 0);
            }
            else
            {
                this.slideAnimation.From = new Thickness(0, 0, 0, 0);
                this.slideAnimation.To = new Thickness(-this.ContentWidth, 0, 0, 0);
            }
            storyBoard.Begin(this);
            this.isOpen = !this.isOpen;
        }

        public int GetMenuBarWidth()
        {
            return this.MenuBarWidth;
        }
    }
}
