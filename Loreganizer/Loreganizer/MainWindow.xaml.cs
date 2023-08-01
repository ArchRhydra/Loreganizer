using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private UIElement _originalElement;
        private double _originalLeft;
        private double _originalTop;
        private double _newLeft;
        private double _newTop;
        private Point _startPoint;
        private Canvas _contentCanvas;


        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnPageLoad;
        }

        public void OnPageLoad(object sender, RoutedEventArgs e)
        {
            _contentCanvas = new Canvas();
            _contentCanvas.Height = 415;
            var tb = new TextBox { Text = "Dewit" };
            Canvas.SetTop(tb, 0);
            Canvas.SetLeft(tb, 0);
            _contentCanvas.Children.Add(tb);
            _contentCanvas.PreviewMouseLeftButtonDown += contentCanvas_PreviewMouseLeftButtonDown;
            _contentCanvas.PreviewMouseMove += contentCanvas_PreviewMouseMove;
            _contentCanvas.PreviewMouseLeftButtonUp += contentCanvas_PreviewMouseLeftButtonUp;
            PreviewKeyDown += window1_PreviewKeyDown;
            
            mainPanel.Children.Add(_contentCanvas);
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
            double centerX = _contentCanvas.ActualWidth / 2;
            double centerY = _contentCanvas.ActualHeight / 2;

            // Set the position of the TextBox to center it on the Canvas
            Canvas.SetLeft(textBox, centerX - textBox.Width / 2);
            Canvas.SetTop(textBox, centerY - textBox.Height / 2);

            // Add the TextBox to the Canvas
            _contentCanvas.Children.Add(textBox);
        }

        private void window1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }

        private void contentCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Up");
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragFinished(bool done)
        {
            Mouse.Capture(null);
            if (_isDragging)
            {

                if(done == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _newTop);
                    Canvas.SetLeft(_originalElement, _originalLeft + _newLeft);
                }
                
            }
            _isDragging = false;
            _isDown = false; Debug.WriteLine("Done dragging");
        }

        private void contentCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Moving");
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(_contentCanvas).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) || (Math.Abs(e.GetPosition(_contentCanvas).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragStarted()
        {
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

        }

        private void DragMoved()
        {
            var currentPosition = Mouse.GetPosition(_contentCanvas);

            _newLeft = currentPosition.X - _startPoint.X;
            _newTop = currentPosition.Y - _startPoint.Y;

        }

        private void contentCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Down");
            if(e.Source == _contentCanvas)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(_contentCanvas);
                _originalElement = e.Source as UIElement;
                _contentCanvas.CaptureMouse();
                e.Handled = true;
            }
        }

    }
}
