using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Loreganizer
{
    internal class TBAdorner : Adorner
    {
        private Rectangle _child;
        private double _leftOffset;
        private double _topOffset;
        private bool _isClicked;
        private Thumb _thumb1, _thumb2, _thumb3, _thumb4;
        private VisualCollection _visuals;

        public TBAdorner(UIElement adornedElement) : base(adornedElement)
        {
            var brush = new VisualBrush(adornedElement);
            _visuals = new VisualCollection(this);
            /*_child = new Rectangle
            {
                Width = adornedElement.RenderSize.Width,
                Height = adornedElement.RenderSize.Height
            };
            _child.IsHitTestVisible = false;/*
            /*var animation = new DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };
            brush.BeginAnimation(Brush.OpacityProperty, animation);*/

            //_child.Fill = brush;
            //IsHitTestVisible = false;

            _thumb1 = new Thumb() { Background = Brushes.Gray, Height = 10, Width = 10 };
            _thumb2 = new Thumb() { Background = Brushes.Gray, Height = 10, Width = 10 };
            _thumb3 = new Thumb() { Background = Brushes.Gray, Height = 10, Width = 10 };
            _thumb4 = new Thumb() { Background = Brushes.Gray, Height = 10, Width = 10 };

            _thumb1.DragDelta += Thumb1_DragDelta;
            _thumb2.DragDelta += Thumb2_DragDelta;
            _thumb3.DragDelta += Thumb3_DragDelta;
            _thumb4.DragDelta += Thumb4_DragDelta;

            _visuals.Add(_thumb1);
            _visuals.Add(_thumb2);
            _visuals.Add(_thumb3);
            _visuals.Add(_thumb4);

            _thumb1.IsHitTestVisible = true;
            _thumb2.IsHitTestVisible = true;
            _thumb3.IsHitTestVisible = true;
            _thumb4.IsHitTestVisible = true;
        }

        protected override int VisualChildrenCount => _visuals.Count;

        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value;
                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value;
                UpdatePosition();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);

            var renderBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.2 };
            var renderPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            const double renderRadius = 5.0;

            //drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }

        /*protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }*/

        protected override Size ArrangeOverride(Size finalSize)
        {
            //_child.Arrange(new Rect(finalSize));
            _thumb1.Arrange(new Rect(-5, -5, 10, 10));
            _thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width-5, -5, 10, 10));
            _thumb3.Arrange(new Rect(-5, AdornedElement.DesiredSize.Height - 5, 10, 10));
            _thumb4.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        private void UpdatePosition()
        {
            var adornerLayer = Parent as AdornerLayer;
            adornerLayer?.Update(AdornedElement);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }

        private void Thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;
            if (element.Height - e.VerticalChange < 0)
            {
                element.Height = 0;
            }
            else 
            { 
                element.Height -= e.VerticalChange;
                Canvas.SetTop(AdornedElement, Canvas.GetTop(AdornedElement) + e.VerticalChange);
            }

            if (element.Width - e.HorizontalChange < 0)
            {
                element.Width = 0;
            }
            else 
            { 
                element.Width -= e.HorizontalChange;
                Canvas.SetLeft(AdornedElement, Canvas.GetLeft(AdornedElement) + e.HorizontalChange);
            }
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;
            if (element.Height - e.VerticalChange < 0)
            {
                element.Height = 0;
            }
            else
            {
                element.Height -= e.VerticalChange;
                Canvas.SetTop(AdornedElement, Canvas.GetTop(AdornedElement) + e.VerticalChange);
            }

            if (element.Width + e.HorizontalChange < 0)
            {
                element.Width = 0;
            }
            else { element.Width += e.HorizontalChange; }
        }

        private void Thumb3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;
            if (element.Height + e.VerticalChange < 0)
            {
                element.Height = 0;
            }
            else { element.Height += e.VerticalChange; }

            if (element.Width - e.HorizontalChange < 0)
            {
                element.Width = 0;
            }
            else
            {
                element.Width -= e.HorizontalChange;
                Canvas.SetLeft(AdornedElement, Canvas.GetLeft(AdornedElement) + e.HorizontalChange);
            }
        }

        private void Thumb4_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;
            if (element.Height + e.VerticalChange < 0)
            {
                element.Height = 0;
            }
            else { element.Height += e.VerticalChange; }

            if (element.Width + e.HorizontalChange < 0)
            {
                element.Width = 0;
            }
            else { element.Width += e.HorizontalChange; }
        }

    }
}
