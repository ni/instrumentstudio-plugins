using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwitchExecutive.Plugin.Internal.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Version
    {
        [JsonProperty]
        public int CurrentVersion => 1;
        [JsonProperty]
        public int OldestCompatibleVersion => 1; // todo: check this and return an error
    }
}
