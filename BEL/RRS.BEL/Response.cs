using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class Response
    {
        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
