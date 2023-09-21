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
using System.IO;
using System.Xml;
using System.Configuration;
using Microsoft.Win32;

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
        private double _currentScale;
        private Point _fromCenter; //coords are how far away from center the pan has gone
        private Point _startPoint;
        private Canvas? _contentCanvas;
        private TBAdorner _overlayElement;
        private string _savePath; //null if file is not something opened from a save. Contains path otherwise.


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
            this.Title = "Loreganizer";

            _contentCanvas = canvas;
            _contentCanvas.Height = 100000;
            _contentCanvas.Width = 100000;
            _contentCanvas.Background = new SolidColorBrush(Colors.White);
            _currentScale = 1.0;

            _contentCanvas.PreviewMouseLeftButtonDown += contentCanvas_PreviewMouseLeftButtonDown;
            _contentCanvas.PreviewMouseMove += contentCanvas_PreviewMouseMove;
            _contentCanvas.PreviewMouseLeftButtonUp += contentCanvas_PreviewMouseLeftButtonUp;
            PreviewKeyDown += window1_PreviewKeyDown;

            //mainPanel.Children.Add(_contentCanvas);
            sideGrid.Height = 200;
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

            // Set location of TextBox to near top left of current window
            Canvas.SetLeft(textBox, 100);
            Canvas.SetTop(textBox, 100);

            // Add TextBox to Canvas
            _contentCanvas.Children.Add(textBox);
        }

        private void Tools_Button_Click(object sender, RoutedEventArgs e)
        {
            if(sideGrid.Visibility == Visibility.Visible)
            {
                sideGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                sideGrid.Visibility = Visibility.Visible;
            }
        }

        /*
         * TextBoxFromData
         * Takes an array with height,width,x,y,content in that order to make a text box
         */
        public void DrawTextBoxFromData(String[] tbData)
        {
            // Create a new TextBox
            TextBox textBox = new TextBox();

            // Set dimensions and text alignment
            textBox.Width = double.Parse(tbData[1]);
            textBox.Height = double.Parse(tbData[0]);
            textBox.TextAlignment = TextAlignment.Center;

            // Set x and y
            Canvas.SetLeft(textBox, double.Parse(tbData[2]));
            Canvas.SetTop(textBox, double.Parse(tbData[3]));

            // Set content
            textBox.Text = tbData[4];

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

        private void Recenter_Button_Click(object sender, RoutedEventArgs e)
        {
            UIElementCollection children = _contentCanvas.Children;
            foreach (UIElement tb in children)
            {
                Canvas.SetLeft(tb, Canvas.GetLeft(tb) - _fromCenter.X);
                Canvas.SetTop(tb, Canvas.GetTop(tb) - _fromCenter.Y);
            }
            _fromCenter.X = 0;
            _fromCenter.Y = 0;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_savePath == null) //if current file is not a save, use the save as feature
            {
                Save_As_Button_Click(sender, e);
            }
            else                   //otherwise save already exists, so just overwrite it
            {
                SaveToXml(_savePath);
            }
        }

        private void Save_As_Button_Click(object sender, RoutedEventArgs e)
        {
            string fileString;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            if ((bool)saveFileDialog.ShowDialog())
            {
                fileString = saveFileDialog.FileName;
                SaveToXml(fileString);
            }
        }

        private void SaveToXml(string path)
        {
            UIElementCollection children = _contentCanvas.Children;
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("<lrg>");
                foreach (UIElement child in children)
                {
                    if (child.GetType().ToString().Equals("System.Windows.Controls.TextBox"))
                    {
                        TextBox tbTemp = (TextBox)child;
                        sw.WriteLine("  <tb>");
                        sw.WriteLine("    <height>" + tbTemp.Height + "</height>");
                        sw.WriteLine("    <width>" + tbTemp.Width + "</width>");
                        sw.WriteLine("    <x>" + (Canvas.GetLeft(tbTemp) - _fromCenter.X) + "</x>");
                        sw.WriteLine("    <y>" + (Canvas.GetTop(tbTemp) - _fromCenter.Y) + "</y>");
                        sw.WriteLine("    <content>" + tbTemp.Text + "</content>");
                        sw.WriteLine("  </tb>");
                    }
                }
                sw.WriteLine("</lrg>");
            }

        }

        private void Open_Button_Click(object sender, RoutedEventArgs e)
        {
            string fileString;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                if (path.Substring(path.Length - 4).Equals(".xml"))
                {
                    fileString = path;
                    if (_overlayElement != null)
                    {
                        AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);
                    }
                    _contentCanvas.Children.Clear();
                    _overlayElement = null;
                    LrgXmlReader lrgReader = new LrgXmlReader(fileString);
                    lrgReader.Read();
                    _savePath = fileString;
                    this.Title = "Loreganizer: " + _savePath;
                }
            }

        }

        private void ScaleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedItem = (comboBox.SelectedItem as ComboBoxItem)?.Content as string;

            switch (selectedItem)
            {
                case "Standard":
                    _currentScale = 1.0; // Reset to standard scale
                    ApplyScale();
                    break;
                case "Zoom In":
                    _currentScale = 1.5; // Increase scale by 50%
                    ApplyScale();
                    break;
                case "Zoom Out":
                    _currentScale = 0.5; // Reduce scale by 50%
                    ApplyScale();
                    break;
            }
        }

        private void ApplyScale()
        {
            if (_contentCanvas != null)
            {
                double originalWidth = _contentCanvas.ActualWidth;
                double originalHeight = _contentCanvas.ActualHeight;

                _contentCanvas.LayoutTransform = new ScaleTransform(_currentScale, _currentScale);

                double newWidth = _contentCanvas.ActualWidth;
                double newHeight = _contentCanvas.ActualHeight;
                double widthChange = newWidth - originalWidth;
                double heightChange = newHeight - originalHeight;

                _fromCenter.X -= widthChange / 2;
                _fromCenter.Y -= heightChange / 2;

                UIElementCollection children = _contentCanvas.Children;
                foreach (UIElement tb in children)
                {
                    Canvas.SetLeft(tb, Canvas.GetLeft(tb) - widthChange / 2);
                    Canvas.SetTop(tb, Canvas.GetTop(tb) - heightChange / 2);
                }
            }
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
            else if (!_isDragging && !_isCanvas && !_panning && AdornerLayer.GetAdornerLayer(_originalElement) != null)
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

            _fromCenter.X += _newLeft;
            _fromCenter.Y += _newTop;

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