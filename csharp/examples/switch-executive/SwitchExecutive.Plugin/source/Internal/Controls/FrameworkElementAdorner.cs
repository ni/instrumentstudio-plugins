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
      private FrameworkElement child;

      // Placement of the child.
      private AdornedControl.AdornerPlacement horizontalAdornerPlacement = AdornedControl.AdornerPlacement.Inside;
      private AdornedControl.AdornerPlacement verticalAdornerPlacement = AdornedControl.AdornerPlacement.Inside;

      // Offset of the child.
      private double offsetX = 0.0;
      private double offsetY = 0.0;

      public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
          : base(adornedElement)
      {
         this.child = adornerChildElement;

         this.AddLogicalChild(adornerChildElement);
         this.AddVisualChild(adornerChildElement);

         // Binding necessary so that when adornedControl's (the "parent") IsVisible property changes, the
         // adorner also reflects that.
         var binding = new Binding
         {
            Path = new PropertyPath(nameof(adornedElement.IsVisible)),
            Source = adornedElement,
            Mode = BindingMode.OneWay,
            Converter = new BooleanToVisibilityConverter()
         };

         this.SetBinding(Adorner.VisibilityProperty, binding);
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
         this.horizontalAdornerPlacement = horizontalAdornerPlacement;
         this.verticalAdornerPlacement = verticalAdornerPlacement;
         this.offsetX = offsetX;
         this.offsetY = offsetY;

         adornedElement.SizeChanged += new SizeChangedEventHandler(this.AdornedElement_SizeChanged);
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
            var list = new List<FrameworkElement>() { this.child };
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
         this.RemoveLogicalChild(this.child);
         this.RemoveVisualChild(this.child);
      }

      protected override Size MeasureOverride(Size constraint)
      {
         this.child.Measure(constraint);
         return this.child.DesiredSize;
      }

      protected override Visual GetVisualChild(int index) => this.child;

      protected override Size ArrangeOverride(Size finalSize)
      {
         double x = this.PositionX;
         if (double.IsNaN(x))
         {
            x = this.DetermineX();
         }

         double y = this.PositionY;
         if (double.IsNaN(y))
         {
            y = this.DetermineY();
         }

         double adornerWidth = this.DetermineWidth();
         double adornerHeight = this.DetermineHeight();
         this.child.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
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

         switch (this.child.HorizontalAlignment)
         {
            case HorizontalAlignment.Left:
               if (this.horizontalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
               {
                  return -this.child.DesiredSize.Width + this.offsetX;
               }
               else
               {
                  return this.offsetX;
               }

            case HorizontalAlignment.Right:
               if (this.horizontalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
               {
                  adornedWidth = this.AdornedElement.ActualWidth;
                  return adornedWidth + this.offsetX;
               }
               else
               {
                  adornerWidth = this.child.DesiredSize.Width;
                  adornedWidth = this.AdornedElement.ActualWidth;
                  widthDifference = adornedWidth - adornerWidth;
                  return widthDifference + this.offsetX;
               }

            case HorizontalAlignment.Center:
               adornerWidth = this.child.DesiredSize.Width;
               adornedWidth = this.AdornedElement.ActualWidth;
               widthDifference = (adornedWidth / 2) - (adornerWidth / 2);
               return widthDifference + this.offsetX;

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

         switch (this.child.VerticalAlignment)
         {
            case VerticalAlignment.Top:
               if (this.verticalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
               {
                  return -this.child.DesiredSize.Height + this.offsetY;
               }
               else
               {
                  return this.offsetY;
               }

            case VerticalAlignment.Bottom:
               if (this.verticalAdornerPlacement == AdornedControl.AdornerPlacement.Outside)
               {
                  adornedHeight = this.AdornedElement.ActualHeight;
                  return adornedHeight + this.offsetY;
               }
               else
               {
                  adornerHeight = this.child.DesiredSize.Height;
                  adornedHeight = this.AdornedElement.ActualHeight;
                  heightDifference = adornedHeight - adornerHeight;
                  return heightDifference + this.offsetY;
               }

            case VerticalAlignment.Center:
               adornerHeight = this.child.DesiredSize.Height;
               adornedHeight = this.AdornedElement.ActualHeight;
               heightDifference = (adornedHeight / 2) - (adornerHeight / 2);
               return heightDifference + this.offsetY;

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
         if (!double.IsNaN(this.PositionX))
         {
            return this.child.DesiredSize.Width;
         }

         switch (this.child.HorizontalAlignment)
         {
            case HorizontalAlignment.Left:
            case HorizontalAlignment.Right:
            case HorizontalAlignment.Center:
               return this.child.DesiredSize.Width;
            case HorizontalAlignment.Stretch:
               return this.AdornedElement.ActualWidth;
         }

         return 0.0;
      }

      /// <summary>
      /// Determine the height of the child.
      /// </summary>
      private double DetermineHeight()
      {
         if (!double.IsNaN(this.PositionY))
         {
            return this.child.DesiredSize.Height;
         }

         switch (this.child.VerticalAlignment)
         {
            case VerticalAlignment.Top:
            case VerticalAlignment.Bottom:
            case VerticalAlignment.Center:
               return this.child.DesiredSize.Height;
            case VerticalAlignment.Stretch:
               return this.AdornedElement.ActualHeight;
         }

         return 0.0;
      }

      /// <summary>
      /// Event raised when the adorned control's size has changed.
      /// </summary>
      private void AdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
      {
         this.InvalidateMeasure();
      }

      #endregion
   }
}
