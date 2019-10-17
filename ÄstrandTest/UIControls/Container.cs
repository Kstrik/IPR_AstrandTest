using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace UIControls
{
    [ContentProperty(nameof(Children))]
    public class Container : ContentControl
    {
        public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(nameof(Children), typeof(UIElementCollection), typeof(Container), new PropertyMetadata());
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Container));
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(Container));
        public static readonly DependencyProperty ScrollbarVisibilityProperty = DependencyProperty.Register("ScrollbarVisibility", typeof(ScrollBarVisibility), typeof(Container));

        public UIElementCollection Children
        {
            get { return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        public Brush HeaderForeground
        {
            get { return (Brush)this.GetValue(HeaderForegroundProperty); }
            set { this.SetValue(HeaderForegroundProperty, value); }
        }

        public ScrollBarVisibility ScrollbarVisibility
        {
            get { return (ScrollBarVisibility)this.GetValue(ScrollbarVisibilityProperty); }
            set { this.SetValue(ScrollbarVisibilityProperty, value); }
        }

        private Grid grid;
        private Label label;
        private StackPanel stackPanel;
        private ScrollViewer scrollViewer;

        public Container()
        {
            this.DataContext = this;

            this.grid = new Grid();
            BindingOperations.SetBinding(this.grid, Grid.BackgroundProperty, new Binding("Foreground"));

            this.label = new Label();
            this.label.FontSize = 12;
            this.label.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.label, Label.ForegroundProperty, new Binding("HeaderForeground"));
            BindingOperations.SetBinding(this.label, Label.FontSizeProperty, new Binding("FontSize"));
            BindingOperations.SetBinding(this.label, Label.ContentProperty, new Binding("Header"));

            this.stackPanel = new StackPanel();
            //this.stackPanel.Margin = new Thickness(5, 35, 5, 5);
            //BindingOperations.SetBinding(this.stackPanel, ScrollViewer.BackgroundProperty, new Binding("Background"));

            this.scrollViewer = new ScrollViewer();
            this.scrollViewer.CanContentScroll = false;
            this.scrollViewer.Content = this.stackPanel;
            this.scrollViewer.Margin = new Thickness(5, 35, 5, 5);
            BindingOperations.SetBinding(this.scrollViewer, ScrollViewer.BackgroundProperty, new Binding("Background"));
            BindingOperations.SetBinding(this.scrollViewer, ScrollViewer.VerticalScrollBarVisibilityProperty, new Binding("ScrollbarVisibility"));

            this.grid.Children.Add(this.label);
            this.grid.Children.Add(this.scrollViewer);
            //this.grid.Children.Add(this.stackPanel);
            this.Content = this.grid;

            Children = this.stackPanel.Children;
        }

        public void ScrollToTop()
        {
            this.scrollViewer.ScrollToTop();
        }

        public void ScrollToEnd()
        {
            this.scrollViewer.ScrollToEnd();
        }
    }
}
