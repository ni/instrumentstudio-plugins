using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.InstrumentFramework.Plugins;

namespace SwitchExecutive.Plugin.Internal.Common
{
    public interface ISave
    {
        void Save();
    }

    class SaveDelegator : ISave
    {
        private PluginSession _pluginSession;
        private Func<string> _serialize;
        private Action<string> _deserialize;
        private bool _loading = false;

        public SaveDelegator(PluginSession pluginSession)
        {
            _pluginSession = pluginSession;
        }

        public void Save()
        {
            if (!Attrached())
                return;

            if (!_loading)
            {
                string serializedState = _serialize();
                _pluginSession.EditTimeConfiguration = serializedState;
                _pluginSession.RunTimeConfiguration = serializedState;
            }
        }

        public void Deserialize(string json)
        {
            if (!Attrached())
                return;

            _loading = true;
            _deserialize(json);
            _loading = false;
        }

        public void Attach(
           Func<string> serialize,
           Action<string> deserialize)
        {
            _serialize = serialize;
            _deserialize = deserialize;
        }

        private bool Attrached() => _serialize != null && _deserialize != null;
    }
}
