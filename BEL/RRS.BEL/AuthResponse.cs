using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class AuthResponse : Response
    {
        [DataMember]
        public int AuthNumber { get; set; }

        [DataMember]
        public int VisitsApproved { get; set; }

        [DataMember]
        public int DaysApproved { get; set; }

        [DataMember]
        public DateTime DateApprovedFrom { get; set; }

        [DataMember]
        public DateTime DateApprovedThru { get; set; }

        [DataMember]
        public DateTime DateApproved { get; set; }

        [DataMember]
        public bool IsApproveReduction { get; set; }

        [DataMember]
        public string DecisionText { get; set; }

        [DataMember]
        public string AuthCode { get; set; }

        [DataMember]
        public string AuthStatus { get; set; }

        [DataMember]
        public string ClientAuthNumber { get; set; }

        [DataMember]
        public int FailToPreAuthDays { get; set; }
    }
}
