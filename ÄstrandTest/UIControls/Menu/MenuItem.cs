using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls.Menu
{
    public class MenuItem
    {
        public delegate void ItemClickEvent(object sender);

        private Menu menu;
        public ItemClickEvent OnClick;

        private string header;
        private int width;
        private int height;

        private Canvas canvas;
        private Label label;

        private SolidColorBrush backgroundColor;
        private SolidColorBrush foregroundColor;
        private SolidColorBrush hoverBackgroundColor;
        private SolidColorBrush hoverForegroundColor;

        public MenuItem(string header, int width, int height, SolidColorBrush backgroundColor, SolidColorBrush foregroundColor, SolidColorBrush hoverBackgroundColor, SolidColorBrush hoverForegroundColor, Menu menu)
        {
            this.menu = menu;

            this.header = header;
            this.width = width;
            this.height = height;

            this.backgroundColor = backgroundColor;
            this.foregroundColor = foregroundColor;
            this.hoverBackgroundColor = hoverBackgroundColor;
            this.hoverForegroundColor = hoverForegroundColor;

            InitializUIElements();
        }

        private void InitializUIElements()
        {
            this.canvas = new Canvas();
            this.canvas.Background = this.backgroundColor;
            this.canvas.Width = this.width;
            this.canvas.Height = this.height;
            this.canvas.HorizontalAlignment = HorizontalAlignment.Left;

            this.label = new Label();
            this.label.Foreground = this.foregroundColor;
            this.label.Content = this.header;
            //this.label.FontSize = this.height / 4;
            //this.label.Margin = new Thickness((this.height / 4.0D), (this.height / 4.0D), (this.height / 4.0D), (this.height / 4.0D));
            this.label.FontSize = this.height / 3;
            this.label.Margin = new Thickness((this.height / 6.0D), (this.height / 6.0D), (this.height / 6.0D), (this.height / 6.0D));

            this.canvas.Children.Add(this.label);

            this.canvas.MouseDown += Canvas_MouseDown;
            this.canvas.MouseEnter += Canvas_MouseEnter;
            this.canvas.MouseLeave += Canvas_MouseLeave;
        }

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.OnClick != null)
                this.OnClick(this);
        }

        private void Canvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.canvas.Background = this.hoverBackgroundColor;
            this.label.Foreground = this.hoverForegroundColor;
        }

        private void Canvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.canvas.Background = this.backgroundColor;
            this.label.Foreground = this.foregroundColor;
        }

        public Canvas GetUIComponents()
        {
            return this.canvas;
        }

        public string GetHeader()
        {
            return this.header;
        }
    }
}
