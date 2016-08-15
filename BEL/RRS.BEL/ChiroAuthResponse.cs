using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class ChiroAuthResponse : AuthResponse
    {
        [DataMember]
        public string BcbsmaNumber { get; set; }
    }
}
