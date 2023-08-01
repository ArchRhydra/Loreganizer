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
        private bool _isDown;
        private bool _isDragging;
        private double _originalLeft;
        private double _originalTop;
        private Point _startPoint;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a new TextBox
            TextBox textBox = new TextBox();

            // Set some properties for the TextBox
            textBox.Width = 150;
            textBox.Height = 30;
            textBox.TextAlignment = TextAlignment.Center;

            // Calculate the center of the Canvas
            double centerX = contentCanvas.ActualWidth / 2;
            double centerY = contentCanvas.ActualHeight / 2;

            // Set the position of the TextBox to center it on the Canvas
            Canvas.SetLeft(textBox, centerX - textBox.Width / 2);
            Canvas.SetTop(textBox, centerY - textBox.Height / 2);

            // Add the TextBox to the Canvas
            contentCanvas.Children.Add(textBox);
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object MovingObject = null;
        }


    }
}
