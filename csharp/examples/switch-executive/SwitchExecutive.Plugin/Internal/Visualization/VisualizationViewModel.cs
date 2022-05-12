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

        private RelayCommand _refreshCommand;
        private readonly ISwitchExecutiveDriverOperations _driverOperations;

        #endregion

        #region Constructors

        public VisualizationViewModel(
           ISwitchExecutiveDriverOperations driverOperations)
        {
            _driverOperations = driverOperations;

            DeviceTableViewModel = new DeviceTableViewModel(driverOperations);
            ChannelTableViewModel = new ChannelTableViewModel(driverOperations);
            RouteTableViewModel = new RouteTableViewModel(driverOperations);
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
                if (_refreshCommand == null)
                    _refreshCommand = new RelayCommand(param => _driverOperations.Refresh());

                return _refreshCommand;
            }
        }

        #endregion

        #region Methods

        #endregion
    }
}
