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
      private static string NoError = string.Empty;
      private string message = Status.NoError;

      public void Set(string msg) => this.Message = msg;
      public void Clear() => this.Message = Status.NoError;
      public string GetMessage() => this.Message;
      public bool IsFatal => this.Message != Status.NoError;

      public string Message
      {
         get => this.message;
         private set
         {
            this.message = value;
            this.NotifyPropertyChanged();
         }
      }
   }
}
