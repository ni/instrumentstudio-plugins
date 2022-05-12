using System;
using System.Collections.Generic;
using System.Linq;
using SwitchExecutive.Plugin.Internal.DriverOperations;
using SwitchExecutive.Plugin.Internal.Common;
using NationalInstruments;

namespace SwitchExecutive.Plugin.Internal
{
    internal sealed class VisualizationViewModel : BaseNotify
    {
        #region Fields

        private RelayCommand refreshCommand;
        private readonly ISwitchExecutiveDriverOperations driverOperations;

        #endregion

        #region Constructors

        public VisualizationViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            this.driverOperations = driverOperations;

            this.DeviceTableViewModel = new DeviceTableViewModel(driverOperations);
            this.ChannelTableViewModel = new ChannelTableViewModel(driverOperations);
            this.RouteTableViewModel = new RouteTableViewModel(driverOperations);
        }

        #endregion

        #region Properties

        public DeviceTableViewModel DeviceTableViewModel { get; }
        public ChannelTableViewModel ChannelTableViewModel { get; }
        public RouteTableViewModel RouteTableViewModel { get; }

        public RelayCommand RefreshCommand
        {
            get
            {
                if (this.refreshCommand == null)
                    this.refreshCommand = new RelayCommand(param => this.driverOperations.Refresh());

                return this.refreshCommand;
            }
        }

        #endregion

        #region Methods

        #endregion
    }
}
