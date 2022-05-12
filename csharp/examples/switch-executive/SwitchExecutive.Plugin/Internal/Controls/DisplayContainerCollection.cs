using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NationalInstruments.Controls;

namespace SwitchExecutive.Plugin.Internal.Controls
{
    [TemplatePart(Name = "PART_Grid", Type = typeof(Grid))]
    public sealed class DisplayContainerCollection : ContentControl
    {
        #region Fields

        private Grid _grid;
        private bool _loaded;

        #endregion

        #region Constructors

        public DisplayContainerCollection()
        {
            Loaded += DisplayContainerCollectionLoaded;

            // TODO:  for some reason this works in the IF stack, but in the plugin it causes a crash
            // on tab switching.  Race condition?  This is really only necessary for layouts that add
            // display containers dynamically (like FFT views).
            //Unloaded += DisplayContainerCollectionUnloaded;
        }

        #endregion

        #region Properties

        public DisplayContainerList DisplayContainers { get; } = new DisplayContainerList();

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _grid = GetTemplateChild("PART_Grid") as Grid;

            if (_grid == null)
            {
                return;
            }

            foreach (var container in DisplayContainers)
            {
                _grid.Children.Add(container);
            }
        }

        public void ResetToDefaults()
        {
            foreach (var container in DisplayContainers)
            {
                container.PreferredProportion = container.DefaultProportion;
                container.IsContentCollapsed = false;

                var containerRow = GetRowDefinitionForContainer(container) as CollapsibleRowDefinition;
                if (containerRow != null)
                {
                    containerRow.Height = new GridLength(container.DefaultProportion, GridUnitType.Star);
                }
            }
        }

        private static void AddGridSplitterBindings(GridSplitter gridSplitter, DisplayContainer previousContainer, DisplayContainer nextContainer)
        {
            MultiBinding multiBinding = new MultiBinding() { Converter = new BooleanAndConverter() };

            // A splitter is disabled if either the container above or below it has its content collapsed
            var booleanToNotBooleanConverter = new BooleanToNotBooleanConverter();
            multiBinding.Bindings.Add(new Binding(nameof(previousContainer.IsContentCollapsed)) { Source = previousContainer, Converter = booleanToNotBooleanConverter });
            multiBinding.Bindings.Add(new Binding(nameof(nextContainer.IsContentCollapsed)) { Source = nextContainer, Converter = booleanToNotBooleanConverter });

            gridSplitter.SetBinding(GridSplitter.IsEnabledProperty, multiBinding);
        }

        /// <summary>
        /// Add any bindings between the row and the DisplayContainer hosted in that row.
        /// </summary>
        private static void AddContainerRowDefinitionBindings(CollapsibleRowDefinition rowDefinitionForContainer, DisplayContainer displayContainer)
        {
            var isCollapsedBinding = new Binding()
            {
                Source = displayContainer,
                Path = new PropertyPath(nameof(displayContainer.IsContentCollapsed))
            };

            BindingOperations.SetBinding(rowDefinitionForContainer, CollapsibleRowDefinition.IsCollapsedProperty, isCollapsedBinding);
        }

        private void DisplayContainerCollectionLoaded(object sender, RoutedEventArgs e)
        {
            if (_grid == null)
                return;

            if (_loaded)
                return;

            AddContainerPropertyChangedListeners();
            PopulateGrid();

            _loaded = true;
        }

        private void DisplayContainerCollectionUnloaded(object sender, RoutedEventArgs e)
        {
            if (!_loaded)
                return;

            RemoveRowDefinitionPropertyChangedListeners();
            RemoveContainerPropertyChangedListeners();

            _loaded = false;
        }

        private void PopulateGrid()
        {
            ResetGrid();

            var activeContainers = DisplayContainers.Where(c => c.Visibility == Visibility.Visible).ToList();
            for (int index = 0; index < activeContainers.Count; index++)
            {
                var displayContainer = activeContainers[index];

                // Add a splitter between containers only after the first row.
                if (index != 0)
                {
                    var rowDefinitionForSplitter = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
                    _grid.RowDefinitions.Add(rowDefinitionForSplitter);

                    var gridSplitter = new GridSplitter() { Style = (Style)InstrumentPanelResources.Instance["GridSplitterStyle"], Height = (double)InstrumentPanelResources.Instance["DisplayContainerSplitterHeight"] };
                    Grid.SetRow(gridSplitter, _grid.RowDefinitions.Count - 1);
                    _grid.Children.Add(gridSplitter);

                    var previousContainer = activeContainers[index - 1];
                    AddGridSplitterBindings(gridSplitter, previousContainer, displayContainer);
                }

                var rowDefinitionForContainer = new CollapsibleRowDefinition()
                {
                    Height = new GridLength(displayContainer.PreferredProportion, GridUnitType.Star),
                    ExpandedHeight = new GridLength(displayContainer.PreferredProportion, GridUnitType.Star),
                    CollapsedHeight = GridLength.Auto,
                    MinimumExpandedHeight = displayContainer.MinimumExpandedHeight,
                };

                AddContainerRowDefinitionBindings(rowDefinitionForContainer, displayContainer);

                _grid.RowDefinitions.Add(rowDefinitionForContainer);
                Grid.SetRow(displayContainer, _grid.RowDefinitions.Count - 1);
            }

            AddRowDefinitionPropertyChangedListeners();

            // Add a row at the end for hidden containers. We keep the hidden containers in the Grid to preserve the Bindings.
            var rowDefinitionForHiddenContainers = new RowDefinition() { Height = GridLength.Auto };
            _grid.RowDefinitions.Add(rowDefinitionForHiddenContainers);

            var hiddenRowIndex = _grid.RowDefinitions.IndexOf(rowDefinitionForHiddenContainers);
            var hiddenContainers = DisplayContainers.Where(c => c.Visibility != Visibility.Visible).ToList();
            foreach (var container in hiddenContainers)
            {
                Grid.SetRow(container, hiddenRowIndex);
            }
        }

        /// <summary>
        /// Reset the grid by clearing out the rows and removing the GridSplitters.
        /// This prepares the grid to be populated in <see cref="PopulateGrid"/>.
        /// </summary>
        private void ResetGrid()
        {
            RemoveRowDefinitionPropertyChangedListeners();
            _grid.RowDefinitions.Clear();

            // Remove the existing splitters, but leave the DisplayContainers as children of the Grid to preserve Bindings.
            // New splitters are added between visible containers when the grid is populated.
            var splitters = _grid.Children.OfType<GridSplitter>().ToList();
            foreach (var splitter in splitters)
            {
                _grid.Children.Remove(splitter);
            }
        }

        /// <summary>
        /// Registers property changed listeners for each DisplayContainer in the collection.
        /// If the Visibility of any DisplayContainer changes, we need to update the grid that hosts the containers.
        /// If the PreferredProportion of any DisplayContainer changes, for example on reload or for DefaultSetup,
        /// then we need to update the properties of the CollapsibleRowDefinition holding that container.
        /// </summary>
        private void AddContainerPropertyChangedListeners()
        {
            var visibilityPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DisplayContainer.VisibilityProperty, typeof(DisplayContainer));
            var proportionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DisplayContainer.PreferredProportionProperty, typeof(DisplayContainer));
            foreach (DisplayContainer displayContainer in DisplayContainers)
            {
                visibilityPropertyDescriptor.AddValueChanged(displayContainer, DisplayContainerVisibilityChanged);
                proportionPropertyDescriptor.AddValueChanged(displayContainer, DisplayContainerPreferredProportionChanged);
            }
        }

        private void RemoveContainerPropertyChangedListeners()
        {
            var visibilityPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DisplayContainer.VisibilityProperty, typeof(DisplayContainer));
            var proportionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DisplayContainer.PreferredProportionProperty, typeof(DisplayContainer));
            foreach (DisplayContainer displayContainer in DisplayContainers)
            {
                visibilityPropertyDescriptor.RemoveValueChanged(displayContainer, DisplayContainerVisibilityChanged);
                proportionPropertyDescriptor.RemoveValueChanged(displayContainer, DisplayContainerPreferredProportionChanged);
            }
        }

        /// <summary>
        /// Registers PropertyChanged listeners for the RowDefintions that are hosting the Containers
        /// If the Height of a RowDefinition changes, from the window size changing or the user moving a GridSplitter,
        /// then we need to push that value back to the DisplayContainer, so it can be persisted in the Model.
        /// </summary>
        private void AddRowDefinitionPropertyChangedListeners()
        {
            var heightPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(RowDefinition));
            foreach (var rowDefinition in _grid.RowDefinitions.OfType<CollapsibleRowDefinition>())
            {
                heightPropertyDescriptor.AddValueChanged(rowDefinition, RowDefinitionHeightChanged);
            }
        }

        private void RemoveRowDefinitionPropertyChangedListeners()
        {
            var heightPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(RowDefinition));
            foreach (var rowDefinition in _grid.RowDefinitions.OfType<CollapsibleRowDefinition>())
            {
                heightPropertyDescriptor.RemoveValueChanged(rowDefinition, RowDefinitionHeightChanged);
            }
        }

        private void DisplayContainerVisibilityChanged(object sender, EventArgs e)
        {
            var container = sender as DisplayContainer;
            if (container.Visibility != Visibility.Visible)
            {
                // Containers that are hidden have preferred proportion reset to the default proportion.
                container.PreferredProportion = container.DefaultProportion;
                container.IsContentCollapsed = false;
            }

            // Recreate the grid to show only visible containers
            PopulateGrid();
        }

        private void RowDefinitionHeightChanged(object sender, EventArgs e)
        {
            var collapsibleRow = (CollapsibleRowDefinition)sender;
            var rowIndex = _grid.RowDefinitions.IndexOf(collapsibleRow);
            var container = DisplayContainers.Where(c => Grid.GetRow(c) == rowIndex).FirstOrDefault();
            if (container != null &&
               !container.IsContentCollapsed &&
               container.PreferredProportion != collapsibleRow.Height.Value)
            {
                container.PreferredProportion = collapsibleRow.Height.Value;
            }
        }

        private void DisplayContainerPreferredProportionChanged(object sender, EventArgs e)
        {
            var container = sender as DisplayContainer;
            var containerRow = GetRowDefinitionForContainer(container) as CollapsibleRowDefinition;

            if (containerRow == null)
            {
                return;
            }

            if (containerRow.IsCollapsed)
            {
                containerRow.ExpandedHeight = new GridLength(container.PreferredProportion, GridUnitType.Star);
            }
            else
            {
                containerRow.Height = new GridLength(container.PreferredProportion, GridUnitType.Star);
            }
        }

        private RowDefinition GetRowDefinitionForContainer(DisplayContainer container)
        {
            var row = Grid.GetRow(container);
            return _grid.RowDefinitions.ElementAt(row);
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// The DisplayContainer property has to be an IList to allow adding items to the collection directly in XAML.
        /// Subclassing List allows us to limit the type of object that can be added.
        /// https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/xaml-syntax-in-detail#collection-syntax
        /// </summary>
        public sealed class DisplayContainerList : List<DisplayContainer>
        {
        }

        #endregion
    }
}
