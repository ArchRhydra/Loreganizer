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
        private bool _isCanvas; //true when most recent mouse down event was the blank canvas space
        private bool _panning;
        private UIElement? _originalElement;
        private double _originalLeft;
        private double _originalTop;
        private double _newLeft;
        private double _newTop;
        private Point _startPoint;
        private Canvas? _contentCanvas;
        private TBAdorner _overlayElement;


        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnPageLoad;
        }

        /*
         * OnPageLoad
         * Activates when this window opens
         * Creates the canvas and assigns listeners for controls
         */
        public void OnPageLoad(object sender, RoutedEventArgs e)
        {
            _contentCanvas = new Canvas();
            _contentCanvas.Height = 415;
            _contentCanvas.Background = new SolidColorBrush(Colors.White);

            /* Block of code for having a text box placed upon opening
            var tb = new TextBox { Text = "Dewit" };
            Canvas.SetTop(tb, 0);
            Canvas.SetLeft(tb, 0);
            //_contentCanvas.Children.Add(tb);
            */

            _contentCanvas.PreviewMouseLeftButtonDown += contentCanvas_PreviewMouseLeftButtonDown;
            _contentCanvas.PreviewMouseMove += contentCanvas_PreviewMouseMove;
            _contentCanvas.PreviewMouseLeftButtonUp += contentCanvas_PreviewMouseLeftButtonUp;
            PreviewKeyDown += window1_PreviewKeyDown;

            mainPanel.Children.Add(_contentCanvas);
        }

        /*
         * TextBoxTool_Button_Click
         * Places a text box in the center of the canvas (default location and size subject to future change)
         */
        private void TextBoxTool_Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a new TextBox
            TextBox textBox = new TextBox();

            // Set dimensions and text alignment
            textBox.Width = 150;
            textBox.Height = 30;
            textBox.TextAlignment = TextAlignment.Center;

            // Calculate the center of the Canvas
            double centerX = _contentCanvas.ActualWidth / 2;
            double centerY = _contentCanvas.ActualHeight / 2;

            // Set location of TextBox to center of canvas
            Canvas.SetLeft(textBox, centerX - textBox.Width / 2);
            Canvas.SetTop(textBox, centerY - textBox.Height / 2);

            // Add TextBox to Canvas
            _contentCanvas.Children.Add(textBox);
        }

        private void Pan_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_panning)
            {
                _panning = false;
                Cursor = Cursors.Arrow;
            }
            else
            {
                _panning = true;
                Cursor = Cursors.Hand;
            }
            Debug.WriteLine("Pan " + _panning);
        }

        /*
         * window1_PreviewKeyDown
         * Handles keyboard presses
         * Only function currently is to immediately end drag if esc is hit
         */
        private void window1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                DragFinished();
            }
        }

        /*
         * contentCanvas_PreviewMouseLeftButtonUp
         * Triggered when the left mouse button is released
         * Handles "clicks" and figures out if the mouse button release is coming from a click or a drag
         * Clicking on an object in the canvas selects it (gives it the adorner)
         */
        private void contentCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Left mb up for" + e.Source);
            if (_originalElement == null)
            {

            }
            //if mouse release is not coming off of a drag, it is a click
            else if (!_isDragging && !_isCanvas)
            {
                //if there is an adorner already, remove it first
                if (_overlayElement != null)
                {
                    AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);
                }

                // Adding adorner to the selected element
                Debug.WriteLine("Left click");
                _overlayElement = new TBAdorner(_originalElement);
                var layer = AdornerLayer.GetAdornerLayer(_originalElement);
                layer.Add(_overlayElement);

            }

            // Handle release of a drag
            if (_isDown)
            {
                DragFinished();
                e.Handled = true;
            }
        }

        /*
         * DragFinished
         * This is called when a drag is released
         * Takes off adorner that was present during drag
         * Marks that dragging is over with bools
         */
        private void DragFinished()
        {
            Mouse.Capture(null);
            if (_isDragging)
            {
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);

                Canvas.SetTop(_originalElement, _originalTop + _newTop);
                Canvas.SetLeft(_originalElement, _originalLeft + _newLeft);


            }
            _isDragging = false;
            _isDown = false;
            Debug.WriteLine("Done dragging");
        }

        /*
         * contentCanvas_PreviewMouseMove
         * Triggered when mouse moves while motion is being captured (ex. during drag)
         * Either marks the beginning of a drag or continues it and updates position
         */
        private void contentCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown && _panning == false)
            {
                //if drag has not yet been started, but mouse is being held and starting to move, start drag
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(_contentCanvas).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) || (Math.Abs(e.GetPosition(_contentCanvas).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                    Debug.WriteLine("Drag Started");
                }
                //if already dragging, update position and continue
                if (_isDragging)
                {
                    DragMoved();
                    Debug.WriteLine("Moving");
                }
            } else if(_isDown && _panning)
            {
                PanMoved();
            }
        }

        /*
         * DragStarted
         * Marks the start of a drag by setting _isDragging to true and saving starting location
         * Adds an adorner to the dragging object, and removes a previous adorner if it existed (to ensure there is only one "selected" object at a time)
         */
        private void DragStarted()
        {
            //mark that dragging started and save initial location
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

            //remove previous adorner if it exists
            if (_overlayElement != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);
            }

            //add adorner to object that is to be dragged
            _overlayElement = new TBAdorner(_originalElement);
            var layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_overlayElement);

        }

        /*
         * DragMoved
         * Called by the mouse move listener during a drag
         * Updates object position live during a drag
         */
        private void DragMoved()
        {
            var currentPosition = Mouse.GetPosition(_contentCanvas);

            _newLeft = currentPosition.X - _startPoint.X;
            _newTop = currentPosition.Y - _startPoint.Y;

            Canvas.SetLeft(_originalElement, _originalLeft + _newLeft);
            Canvas.SetTop(_originalElement, _originalTop + _newTop);
        }

        private void PanMoved()
        {
            var currentPosition = Mouse.GetPosition(_contentCanvas);

            _newLeft = currentPosition.X - _startPoint.X;
            _newTop = currentPosition.Y - _startPoint.Y;

            UIElementCollection children = _contentCanvas.Children;
            foreach (UIElement tb in children)
            {
                Canvas.SetLeft(tb, Canvas.GetLeft(tb) + _newLeft);
                Canvas.SetTop(tb, Canvas.GetTop(tb) + _newTop);
            }
            _startPoint = currentPosition;
        }

        /*
         * contentCanvas_PreviewMouseLeftButtonDown
         * Triggers when mouse left button gets pushed down
         * Detects when a press happens over the canvas, which "deselects" the current selected object
         * Detects when a press happens on an element and sets it as the current _originalElement
         */
        private void contentCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Left mb down for " + e.Source);

            //if pressed on the blank space, deselect whatever is selected
            if (e.Source == _contentCanvas)
            {
                if (_overlayElement != null)
                {
                    AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);
                    _isCanvas = true;
                }
                if (_panning)
                {
                    _isDown = true;
                    _startPoint = e.GetPosition(_contentCanvas);
                    _contentCanvas.CaptureMouse();
                    e.Handled = true;
                }
            }
            //if pressed on something, make it the current _originalElement and mark appropriate flags
            else
            {
                _isDown = true;
                _isCanvas = false;
                _startPoint = e.GetPosition(_contentCanvas);
                _originalElement = e.Source as UIElement;
                _contentCanvas.CaptureMouse();
                e.Handled = true;
            }
        }


    }
}