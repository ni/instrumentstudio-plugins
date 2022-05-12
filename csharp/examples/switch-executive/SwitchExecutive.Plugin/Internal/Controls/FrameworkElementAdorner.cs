using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

// This code based on code available here:
// https://www.codeproject.com/Articles/54472/Defining-WPF-Adorners-in-XAML
namespace SwitchExecutive.Plugin.Internal.Controls
{
    // This class is an adorner that allows a FrameworkElement derived class to adorn another FrameworkElement.
    internal class FrameworkElementAdorner : Adorner
    {
        // The framework element that is the adorner.
        private FrameworkElement _child;

        // Placement of the child.
        private AdornedControl.AdornerPlacement _horizontalAdornerPlacement = AdornedControl.AdornerPlacement.Inside;
        private AdornedControl.AdornerPlacement _verticalAdornerPlacement = AdornedControl.AdornerPlacement.Inside;

        // Offset of the child.
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
            : base(adornedElement)
        {
            _child = adornerChildElement;

            AddLogicalChild(adornerChildElement);
            AddVisualChild(adornerChildElement);

            // Binding necessary so that when adornedControl's (the "parent") IsVisible property changes, the
            // adorner also reflects that.
            var binding = new Binding
            {
                Path = new PropertyPath(nameof(adornedElement.IsVisible)),
                Source = adornedElement,
                Mode = BindingMode.OneWay,
                Converter = new BooleanToVisibilityConverter()
            };

            SetBinding(Adorner.VisibilityProperty, binding);
        }

        public FrameworkElementAdorner(
           FrameworkElement adornerChildElement,
           FrameworkElement adornedElement,
           AdornedControl.AdornerPlacement horizontalAdornerPlacement,
           AdornedControl.AdornerPlacement verticalAdornerPlacement,
           double offsetX,
           double offsetY)
            : this(adornerChildElement, adornedElement)
        {
            _horizontalAdornerPlacement = horizontalAdornerPlacement;
            _verticalAdornerPlacement = verticalAdornerPlacement;
            _offsetX = offsetX;
            _offsetY = offsetY;

            adornedElement.SizeChanged += new SizeChangedEventHandler(AdornedElement_SizeChanged);
        }

        #region Properties

        // Position of the child (when not set to NaN).
        public double PositionX { get; set; } = double.NaN;

        public double PositionY { get; set; } = double.NaN;

        /// <summary>
        /// Override AdornedElement from base class for less type-checking.
        /// </summary>
        public new FrameworkElement AdornedElement => (FrameworkElement)base.AdornedElement;

        protected override int VisualChildrenCount => 1;

        protected override IEnumerator LogicalChildren
        {
            get
            {
                var list = new List<FrameworkElement>() { _child };
                return (IEnumerator)list.GetEnumerator();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disconnect the child element from the visual tree so that it may be reused later.
        /// </summary>
        public void DisconnectChild()
        {
            RemoveLogicalChild(_child);
            RemoveVisualChild(_child);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Visual GetVisualChild(int index) => _child;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = PositionX;
            if (double.IsNaN(x))
            {
                x = DetermineX();
            }

            double y = PositionY;
            if (double.IsNaN(y))
            {
                y = DetermineY();
            }

            double adornerWidth = DetermineWidth();
            double adornerHeight = DetermineHeight();
            _child.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            return finalSize;
        }

        /// <summary>
        /// Determine the X coordinate of the child.
        /// </summary>
        private double DetermineX()
        {
            double adornedWidth;
            double adornerWidth;
            double widthDifference;

            switch (_child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    if (_horizontalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
                    {
                        return -_child.DesiredSize.Width + _offsetX;
                    }
                    else
                    {
                        return _offsetX;
                    }

                case HorizontalAlignment.Right:
                    if (_horizontalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
                    {
                        adornedWidth = AdornedElement.ActualWidth;
                        return adornedWidth + _offsetX;
                    }
                    else
                    {
                        adornerWidth = _child.DesiredSize.Width;
                        adornedWidth = AdornedElement.ActualWidth;
                        widthDifference = adornedWidth - adornerWidth;
                        return widthDifference + _offsetX;
                    }

                case HorizontalAlignment.Center:
                    adornerWidth = _child.DesiredSize.Width;
                    adornedWidth = AdornedElement.ActualWidth;
                    widthDifference = (adornedWidth / 2) - (adornerWidth / 2);
                    return widthDifference + _offsetX;

                case HorizontalAlignment.Stretch:
                    return 0.0;
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the Y coordinate of the child.
        /// </summary>
        private double DetermineY()
        {
            double adornedHeight;
            double adornerHeight;
            double heightDifference;

            switch (_child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    if (_verticalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
                    {
                        return -_child.DesiredSize.Height + _offsetY;
                    }
                    else
                    {
                        return _offsetY;
                    }

                case VerticalAlignment.Bottom:
                    if (_verticalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
                    {
                        adornedHeight = AdornedElement.ActualHeight;
                        return adornedHeight + _offsetY;
                    }
                    else
                    {
                        adornerHeight = _child.DesiredSize.Height;
                        adornedHeight = AdornedElement.ActualHeight;
                        heightDifference = adornedHeight - adornerHeight;
                        return heightDifference + _offsetY;
                    }

                case VerticalAlignment.Center:
                    adornerHeight = _child.DesiredSize.Height;
                    adornedHeight = AdornedElement.ActualHeight;
                    heightDifference = (adornedHeight / 2) - (adornerHeight / 2);
                    return heightDifference + _offsetY;

                case VerticalAlignment.Stretch:
                    return 0.0;
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the width of the child.
        /// </summary>
        private double DetermineWidth()
        {
            if (!double.IsNaN(PositionX))
            {
                return _child.DesiredSize.Width;
            }

            switch (_child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Right:
                case HorizontalAlignment.Center:
                    return _child.DesiredSize.Width;
                case HorizontalAlignment.Stretch:
                    return AdornedElement.ActualWidth;
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the height of the child.
        /// </summary>
        private double DetermineHeight()
        {
            if (!double.IsNaN(PositionY))
            {
                return _child.DesiredSize.Height;
            }

            switch (_child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                case VerticalAlignment.Bottom:
                case VerticalAlignment.Center:
                    return _child.DesiredSize.Height;
                case VerticalAlignment.Stretch:
                    return AdornedElement.ActualHeight;
            }

            return 0.0;
        }

        /// <summary>
        /// Event raised when the adorned control's size has changed.
        /// </summary>
        private void AdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        #endregion
    }
}
