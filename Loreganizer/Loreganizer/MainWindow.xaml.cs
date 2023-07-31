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

namespace Loreganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a new TextBox
            TextBox newTextBox = new TextBox();

            // Add any properties you want to set for the TextBox here
            newTextBox.Width = 150;
            newTextBox.Margin = new Thickness(5);

            // Set the alignment to center (optional)
            newTextBox.HorizontalAlignment = HorizontalAlignment.Center;
            newTextBox.VerticalAlignment = VerticalAlignment.Center;

            // Add the TextBox to the contentGrid
            contentGrid.Children.Add(newTextBox);

            // Optionally, you can set focus on the new TextBox
            newTextBox.Focus();
        }
    }
}
