using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class AuthRequest
    {
        [DataMember]
        public string IvrCode { get; set; }

        [DataMember]
        public string SubscriberId { get; set; }

        [DataMember]
        public string MemberSeq { get; set; }

        [DataMember]
        public string MemberId { get; set; }

        [DataMember]
        public bool HasReferral { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public string RequestType { get; set; }

        [DataMember]
        public string Condition { get; set; }

        [DataMember]
        public DateTime InitialVisitDate { get; set; }

        [DataMember]
        public int NumberOfVisits { get; set; }

        [DataMember]
        public int NumberOfWeeks { get; set; }

        [DataMember]
        public string PrimaryDiagnosisCode { get; set; }

        [DataMember]
        public string SecondaryDiagnosisCode { get; set; }

        [DataMember]
        public int PainRating { get; set; }

        [DataMember]
        public bool HasDiabetes { get; set; }

        [DataMember]
        public bool HasCancer { get; set; }

        [DataMember]
        public bool IsSmoker { get; set; }

        [DataMember]
        public bool IsOverweight { get; set; }

        [DataMember]
        public int MaxVisits { get; set; }

        [DataMember]
        public int ActualVisits { get; set; }

        [DataMember]
        public bool AcceptAuthReduction { get; set; }

        [DataMember]
        public bool SendToReview { get; set; }

        [DataMember]
        public DateTime ActualVisitStartDate { get; set; }

        [DataMember]
        public DateTime ActualVisitEndDate { get; set; }

        [DataMember]
        public string VisitYearType { get; set; }

        [DataMember]
        public bool HasExercise { get; set; }

        [DataMember]
        public bool HasDepression { get; set; }

        [DataMember]
        public int ApplicationId { get; set; }

        [DataMember]
        public string ClientAuthNumber { get; set; }

        [DataMember]
        public bool IsNaviNet { get; set; }
    }
}
