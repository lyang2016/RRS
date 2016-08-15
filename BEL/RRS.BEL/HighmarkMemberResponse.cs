using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class HighmarkMemberResponse : Response
    {
        [DataMember]
        public string SubscriberId { get; set; }

        [DataMember]
        public string MemberSequence { get; set; }

        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string CopcId { get; set; }

        [DataMember]
        public string CoptC { get; set; }

        [DataMember]
        public string Eal { get; set; }

        [DataMember]
        public string IsErisa { get; set; }

        [DataMember]
        public string GroupNumber { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string GroupClientNumber { get; set; }

        [DataMember]
        public string PartnerId { get; set; }

        [DataMember]
        public string Lob { get; set; }

        [DataMember]
        public string PlanType { get; set; }
    }
}
