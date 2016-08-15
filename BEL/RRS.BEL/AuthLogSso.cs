using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class AuthSsoData
    {
        [DataMember]
        public string OrderingPhysicianId { get; set; }

        [DataMember]
        public string OrderingPhysicianNpi { get; set; }

        [DataMember]
        public string OrderingVendorId { get; set; }

        [DataMember]
        public string OrderingVendorNpi { get; set; }

        [DataMember]
        public string PerformingPhysicianId { get; set; }

        [DataMember]
        public string PerformingPhysicianNpi { get; set; }

        [DataMember]
        public string PerformingVendorId { get; set; }

        [DataMember]
        public string PerformingVendorNpi { get; set; }

        [DataMember]
        public string FacilityId { get; set; }

        [DataMember]
        public string FacilityNpi { get; set; }

        [DataMember(Order = 11)]
        public string Service { get; set; }
    }
}
