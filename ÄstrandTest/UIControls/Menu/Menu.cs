using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIControls.Menu
{
    public class Menu
    {
        private List<MenuItem> menuItems;
        private StackPanel stackPanel;

        private int width;
        private int height;

        private SolidColorBrush backgroundColor;
        private SolidColorBrush foregroundColor;
        private SolidColorBrush hoverBackgroundColor;
        private SolidColorBrush hoverForegroundColor;

        public Menu(int width, int height, SolidColorBrush backgroundColor, SolidColorBrush foregroundColor, SolidColorBrush hoverBackgroundColor, SolidColorBrush hoverForegroundColor)
        {
            this.menuItems = new List<MenuItem>();

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
            this.stackPanel = new StackPanel();
            this.stackPanel.Width = this.width;
            this.stackPanel.Height = this.height;
        }

        public void AddItem(string header, int width, int height, MenuItem.ItemClickEvent eventHandler)
        {
            MenuItem menuItem = new MenuItem(header, width, height, this.backgroundColor, this.foregroundColor, this.hoverBackgroundColor, this.hoverForegroundColor, this);
            this.menuItems.Add(menuItem);
            this.stackPanel.Children.Add(menuItem.GetUIComponents());

            if (eventHandler != null)
                menuItem.OnClick += eventHandler;
        }

        public void RemoveItem(string header)
        {
            MenuItem menuItem = this.menuItems.Where(m => m.GetHeader() == header).First();

            if (menuItem != null)
            {
                this.menuItems.Remove(menuItem);
                this.stackPanel.Children.Remove(menuItem.GetUIComponents());
            }
        }

        public StackPanel GetUIComponent()
        {
            return this.stackPanel;
        }
    }
}
