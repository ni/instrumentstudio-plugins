using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SwitchExecutive.Plugin.Internal.Controls
{
    public class CollapsibleRowDefinition : RowDefinition
    {
        #region Fields

        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
           "IsCollapsed",
           typeof(bool),
           typeof(CollapsibleRowDefinition),
           new PropertyMetadata(false, OnIsCollapsedChanged));

        private bool _isCollapsed = false;
        private double _minimumExpandedHeight = 0.0;

        #endregion

        #region Constructors

        public CollapsibleRowDefinition()
        {
            // Add a value changed callback for RowDefinition.Height to update ExpandedHeight.
            var heightPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(
               RowDefinition.HeightProperty,
               typeof(RowDefinition));

            Loaded += (sender, e) =>
            {
                heightPropertyDescriptor.AddValueChanged(this, OnHeightChanged);
            };

            Unloaded += (sender, e) =>
            {
                heightPropertyDescriptor.RemoveValueChanged(this, OnHeightChanged);
            };
        }

        #endregion

        #region Properties

        public GridLength CollapsedHeight { get; set; }

        public GridLength ExpandedHeight { get; set; }

        public double MinimumExpandedHeight
        {
            get
            {
                return _minimumExpandedHeight;
            }

            set
            {
                _minimumExpandedHeight = value;

                if (!IsCollapsed)
                {
                    MinHeight = value;
                }
            }
        }

        public bool IsCollapsed
        {
            get
            {
                return _isCollapsed;
            }

            set
            {
                if (_isCollapsed != value)
                {
                    _isCollapsed = value;
                    Height = value ? CollapsedHeight : ExpandedHeight;
                    MinHeight = value ? CollapsedHeight.Value : MinimumExpandedHeight;
                }
            }
        }

        #endregion

        #region Methods

        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var row = (CollapsibleRowDefinition)d;
            row.IsCollapsed = (bool)e.NewValue;
        }

        private static void OnHeightChanged(object sender, EventArgs e)
        {
            var row = (CollapsibleRowDefinition)sender;

            // When the row height changes, update ExpandedHeight to match.
            if (!row.IsCollapsed)
            {
                row.ExpandedHeight = row.Height;
            }
        }

        #endregion
    }
}
