using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class BcbsmaSsoResponse : Response
    {
        [DataMember]
        public string ProviderId { get; set; }

        [DataMember]
        public int BcbsmaGroupIndicator { get; set; }

        [DataMember]
        public string BcbsmaProviderType { get; set; }

        [DataMember]
        public string IvrCode { get; set; }
    }
}
