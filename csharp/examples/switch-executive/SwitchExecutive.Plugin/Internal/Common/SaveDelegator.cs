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
        private PluginSession pluginSession;
        private Func<string> serialize;
        private Action<string> deserialize;
        private bool loading = false;

        public SaveDelegator(PluginSession pluginSession)
        {
            this.pluginSession = pluginSession;
        }

        public void Save()
        {
            if (!this.Attrached())
                return;

            if (!this.loading)
            {
                string serializedState = this.serialize();
                this.pluginSession.EditTimeConfiguration = serializedState;
                this.pluginSession.RunTimeConfiguration = serializedState;
            }
        }

        public void Deserialize(string json)
        {
            if (!this.Attrached())
                return;

            this.loading = true;
            this.deserialize(json);
            this.loading = false;
        }

        public void Attach(
           Func<string> serialize,
           Action<string> deserialize)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }

        private bool Attrached() => this.serialize != null && this.deserialize != null;
    }
}
