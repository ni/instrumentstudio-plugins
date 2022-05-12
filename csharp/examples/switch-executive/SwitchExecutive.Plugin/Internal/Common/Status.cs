using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchExecutive.Plugin.Internal.Common
{
    internal interface IStatus : INotifyPropertyChanged
    {
        void Set(string msg);
        void Clear();
        string GetMessage();
        bool IsFatal { get; }

        string Message { get; }
    }

    public class Status : BaseNotify, IStatus
    {
        private static string _noError = string.Empty;
        private string _message = Status._noError;

        public void Set(string msg) => Message = msg;
        public void Clear() => Message = Status._noError;
        public string GetMessage() => Message;
        public bool IsFatal => Message != Status._noError;

        public string Message
        {
            get => _message;
            private set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }
    }
}
