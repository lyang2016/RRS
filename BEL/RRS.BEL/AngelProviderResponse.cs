using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class AngelProviderResponse : ProviderResponse
    {
        [DataMember]
        public string NpiLast4 { get; set; }

        [DataMember]
        public bool IsHighmark { get; set; }

        [DataMember]
        public bool IsDean { get; set; }
    }
}
