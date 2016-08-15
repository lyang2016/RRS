using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class MemberResponse : Response
    {
        [DataMember]
        public string MemberName { get; set; }

        [DataMember]
        public int MaxVisits { get; set; }

        [DataMember]
        public int ActualVisits { get; set; }

        [DataMember]
        public string SubscriberId { get; set; }

        [DataMember]
        public string MemberSeq { get; set; }

        [DataMember]
        public string PlanId { get; set; }

        [DataMember]
        public DateTime ActualVisitStartDate { get; set; }

        [DataMember]
        public DateTime ActualVisitEndDate { get; set; }

        [DataMember]
        public string VisitYearType { get; set; }

        [DataMember]
        public int UnmanagedVisits { get; set; }

        [DataMember]
        public string ClaimsAddressError { get; set; }

        [DataMember]
        public int ClaimsAddressId { get; set; }

        [DataMember]
        public string ClaimsAddress { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public int ErrorNumber { get; set; }

        [DataMember]
        public bool IsMedicareAdvantage { get; set; }

        [DataMember]
        public string PrefixSubscriberId { get; set; }

        [DataMember]
        public bool UseCareRegistration { get; set; }

        [DataMember]
        public string RealAuthTypeId { get; set; }

        [DataMember(Order = 20)]
        public string IvrCode { get; set; }
    }
}
