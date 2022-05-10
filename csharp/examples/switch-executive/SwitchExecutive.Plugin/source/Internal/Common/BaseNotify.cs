using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SwitchExecutive.Plugin.Internal.Common
{
   public class BaseNotify : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      /// Raises the <see cref="PropertyChanged"/> event.
      /// <param name="propertyName">Name of the property that changed. The CallerMemberName attribute that is applied to this optional parameter causes the property name of the caller to be substituted as an argument.</param>
      public void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
      {
         this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
