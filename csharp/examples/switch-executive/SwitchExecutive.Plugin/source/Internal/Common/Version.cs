using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace SwitchExecutive.Plugin.Internal.Common
{
   [JsonObject(MemberSerialization.OptIn)]
   class Version
   {
      [JsonProperty]
      public int CurrentVersion => 1;
      [JsonProperty]
      public int OldestCompatibleVersion => 1; //todo: check this and return an error
   }
}
