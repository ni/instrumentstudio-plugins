using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

// This code based on code available here:
// https://www.codeproject.com/Articles/54472/Defining-WPF-Adorners-in-XAML
namespace SwitchExecutive.Plugin.Internal.Controls
{
    /// <summary>
    /// A content control that allows an adorner for the content to
    /// be defined in XAML.
    /// </summary>
    public class AdornedControl : ContentControl
    {
        #region Fields

        /// <summary>
        /// Dependency properties.
        /// </summary>
        public static readonly DependencyProperty IsAdornerVisibleProperty =
            DependencyProperty.Register("IsAdornerVisible", typeof(bool), typeof(AdornedControl), new FrameworkPropertyMetadata(IsAdornerVisible_PropertyChanged));

        public static readonly DependencyProperty AdornerContentProperty =
            DependencyProperty.Register("AdornerContent", typeof(FrameworkElement), typeof(AdornedControl), new FrameworkPropertyMetadata(AdornerContent_PropertyChanged));

        public static readonly DependencyProperty HorizontalAdornerPlacementProperty =
            DependencyProperty.Register("HorizontalAdornerPlacement", typeof(AdornerPlacement), typeof(AdornedControl), new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        public static readonly DependencyProperty VerticalAdornerPlacementProperty =
            DependencyProperty.Register("VerticalAdornerPlacement", typeof(AdornerPlacement), typeof(AdornedControl), new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        public static readonly DependencyProperty AdornerOffsetXProperty =
            DependencyProperty.Register("AdornerOffsetX", typeof(double), typeof(AdornedControl));

        public static readonly DependencyProperty AdornerOffsetYProperty =
            DependencyProperty.Register("AdornerOffsetY", typeof(double), typeof(AdornedControl));

        /// <summary>
        /// Caches the adorner layer.
        /// </summary>
        private AdornerLayer _adornerLayer = null;

        /// <summary>
        /// The actual adorner create to contain our 'adorner UI content'.
        /// </summary>
        private FrameworkElementAdorner _adorner = null;

        #endregion

        #region Constructors

        public AdornedControl()
        {
            Focusable = false; // By default don't want 'AdornedControl' to be focusable.

            DataContextChanged += new DependencyPropertyChangedEventHandler(AdornedControl_DataContextChanged);
        }

        #endregion

        #region Enums

        public enum AdornerPlacement
        {
            Inside,
            Outside
        }

        #endregion

        /// <summary>
        /// Shows or hides the adorner.
        /// Set to 'true' to show the adorner or 'false' to hide the adorner.
        /// </summary>
        public bool IsAdornerVisible
        {
            get
            {
                return (bool)GetValue(IsAdornerVisibleProperty);
            }

            set
            {
                SetValue(IsAdornerVisibleProperty, value);
            }
        }

        /// <summary>
        /// Used in XAML to define the UI content of the adorner.
        /// </summary>
        public FrameworkElement AdornerContent
        {
            get
            {
                return (FrameworkElement)GetValue(AdornerContentProperty);
            }

            set
            {
                SetValue(AdornerContentProperty, value);
            }
        }

        /// <summary>
        /// Specifies the horizontal placement of the adorner relative to the adorned control.
        /// </summary>
        public AdornerPlacement HorizontalAdornerPlacement
        {
            get
            {
                return (AdornerPlacement)GetValue(HorizontalAdornerPlacementProperty);
            }

            set
            {
                SetValue(HorizontalAdornerPlacementProperty, value);
            }
        }

        /// <summary>
        /// Specifies the vertical placement of the adorner relative to the adorned control.
        /// </summary>
        public AdornerPlacement VerticalAdornerPlacement
        {
            get
            {
                return (AdornerPlacement)GetValue(VerticalAdornerPlacementProperty);
            }

            set
            {
                SetValue(VerticalAdornerPlacementProperty, value);
            }
        }

        /// <summary>
        /// X offset of the adorner.
        /// </summary>
        public double AdornerOffsetX
        {
            get
            {
                return (double)GetValue(AdornerOffsetXProperty);
            }

            set
            {
                SetValue(AdornerOffsetXProperty, value);
            }
        }

        /// <summary>
        /// Y offset of the adorner.
        /// </summary>
        public double AdornerOffsetY
        {
            get
            {
                return (double)GetValue(AdornerOffsetYProperty);
            }

            set
            {
                SetValue(AdornerOffsetYProperty, value);
            }
        }

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ShowOrHideAdornerInternal();
        }

        /// <summary>
        /// Event raised when the value of IsAdornerVisible has changed.
        /// </summary>
        private static void IsAdornerVisible_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdornedControl c = (AdornedControl)o;
            c.ShowOrHideAdornerInternal();
        }

        /// <summary>
        /// Event raised when the value of AdornerContent has changed.
        /// </summary>
        private static void AdornerContent_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdornedControl c = (AdornedControl)o;
            c.ShowOrHideAdornerInternal();
        }

        /// <summary>
        /// Event raised when the DataContext of the adorned control changes.
        /// </summary>
        private void AdornedControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAdornerDataContext();
        }

        /// <summary>
        /// Update the DataContext of the adorner from the adorned control.
        /// </summary>
        private void UpdateAdornerDataContext()
        {
            if (AdornerContent != null)
            {
                AdornerContent.DataContext = DataContext;
            }
        }

        /// <summary>
        /// Internal method to show or hide the adorner based on the value of IsAdornerVisible.
        /// </summary>
        private void ShowOrHideAdornerInternal()
        {
            if (IsAdornerVisible)
            {
                ShowAdornerInternal();
            }
            else
            {
                HideAdornerInternal();
            }
        }

        /// <summary>
        /// Internal method to show the adorner.
        /// </summary>
        private void ShowAdornerInternal()
        {
            if (_adorner != null)
            {
                // Already adorned.
                return;
            }

            if (AdornerContent != null)
            {
                if (_adornerLayer == null)
                {
                    _adornerLayer = AdornerLayer.GetAdornerLayer(this);
                }

                if (_adornerLayer != null)
                {
                    _adorner = new FrameworkElementAdorner(AdornerContent, this, HorizontalAdornerPlacement, VerticalAdornerPlacement, AdornerOffsetX, AdornerOffsetY);
                    _adornerLayer.Add(_adorner);

                    UpdateAdornerDataContext();
                }
            }
        }

        /// <summary>
        /// Internal method to hide the adorner.
        /// </summary>
        private void HideAdornerInternal()
        {
            if (_adornerLayer == null || _adorner == null)
            {
                // Not already adorned.
                return;
            }

            _adornerLayer.Remove(_adorner);
            _adorner.DisconnectChild();

            _adorner = null;
            _adornerLayer = null;
        }

        #endregion
    }
}
