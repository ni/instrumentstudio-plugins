using NationalInstruments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace SwitchExecutive.Plugin.Internal.Controls
{
    [ContentProperty(nameof(DisplayContent))]
    [DebuggerDisplay("DisplayContainer for {Title}")]
    public sealed class DisplayContainer : ContentControl
    {
        #region Fields

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
           "Title",
           typeof(string),
           typeof(DisplayContainer),
           new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsCollapsibleProperty = DependencyProperty.Register(
           "IsCollapsible",
           typeof(bool),
           typeof(DisplayContainer),
           new UIPropertyMetadata(true, OnIsCollapsibleChanged));

        public static readonly DependencyProperty IsContentCollapsedProperty = DependencyProperty.Register(
           "IsContentCollapsed",
            typeof(bool),
            typeof(DisplayContainer),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PreferredProportionProperty = DependencyProperty.Register(
           "PreferredProportion",
           typeof(double),
           typeof(DisplayContainer),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DisplayContentProperty = DependencyProperty.Register(
           "DisplayContent",
           typeof(object),
           typeof(DisplayContainer),
           new UIPropertyMetadata(null, OnContentPropertyChanged));

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
           "HeaderContent",
           typeof(object),
           typeof(DisplayContainer),
           new UIPropertyMetadata(null, OnContentPropertyChanged));

        private ICommand expandCollapseContentCommand;

        #endregion

        #region Constructors

        public DisplayContainer()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Default proportionate row height when the DisplayContainer is hosted in a <see cref="DisplayContainerCollection"/>
        /// </summary>
        public double DefaultProportion { get; set; }

        /// <summary>
        /// Preferred proportionate row height when the DisplayContainer is hosted in a <see cref="DisplayContainerCollection"/>
        /// PreferredProportion is the proportionate height for the container's row when <see cref="IsContentCollapsed"/> is false.
        /// </summary>
        public double PreferredProportion
        {
            get { return (double)this.GetValue(DisplayContainer.PreferredProportionProperty); }
            set { this.SetValue(DisplayContainer.PreferredProportionProperty, value); }
        }

        public double MinimumExpandedHeight { get; set; }

        public bool IsCollapsible
        {
            get { return (bool)this.GetValue(DisplayContainer.IsCollapsibleProperty); }
            set { this.SetValue(DisplayContainer.IsCollapsibleProperty, value); }
        }

        public string Title
        {
            get { return (string)this.GetValue(DisplayContainer.TitleProperty); }
            set { this.SetValue(DisplayContainer.TitleProperty, value); }
        }

        public bool IsContentCollapsed
        {
            get { return (bool)this.GetValue(DisplayContainer.IsContentCollapsedProperty); }
            set { this.SetValue(DisplayContainer.IsContentCollapsedProperty, value); }
        }

        public object DisplayContent
        {
            get { return (object)this.GetValue(DisplayContainer.DisplayContentProperty); }
            set { this.SetValue(DisplayContainer.DisplayContentProperty, value); }
        }

        public object HeaderContent
        {
            get { return (object)this.GetValue(DisplayContainer.HeaderContentProperty); }
            set { this.SetValue(DisplayContainer.HeaderContentProperty, value); }
        }

        public ICommand ExpandCollapseContentCommand
        {
            get
            {
                return this.expandCollapseContentCommand ?? (this.expandCollapseContentCommand = new RelayCommand(param => { this.IsContentCollapsed = !this.IsContentCollapsed; }));
            }
        }

        /// <summary>
        /// Overriding LogicalChildren and adding the HeaderContent/DisplayContent as logical children
        /// of the control allows property inheritance and ElementName binding within the content.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var children = new List<object>()
            {
               this.DisplayContent,
               this.HeaderContent
            };

                return children.GetEnumerator();
            }
        }

        #endregion

        #region Methods

        private static void OnIsCollapsibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = (DisplayContainer)d;
            if (container.IsCollapsible == false && container.IsContentCollapsed == true)
            {
                container.IsContentCollapsed = false;
            }
        }

        /// <summary>
        /// Overriding LogicalChildren and adding the HeaderContent/DisplayContent as logical children
        /// of the control allows property inheritance and ElementName binding within the content.
        /// </summary>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = (DisplayContainer)d;
            if (e.OldValue != null)
            {
                container.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                container.AddLogicalChild(e.NewValue);
            }
        }

        #endregion
    }
}
