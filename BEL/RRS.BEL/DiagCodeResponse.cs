using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class DiagCodeResponse : Response
    {
        [DataMember]
        public string DiagCode { get; set; }
    }
}
