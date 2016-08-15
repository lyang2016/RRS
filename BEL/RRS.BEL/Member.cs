using System;

namespace RRS.BEL
{
    [Serializable]
    public class Member
    {
        public string SubscriberId { get; set; }

        public string MemberSeq { get; set; }

        public string GroupId { get; set; }

        public string DivisionId { get; set; }

        public string PlanId { get; set; }

        public string NetworkId { get; set; }

        public DateTime GroupCoverageStart { get; set; }

        public int UnmanagedVisits { get; set; }

        public int NumberOfPreviousAuths { get; set; }

        public int AllowedVisits { get; set; }

        public int AllowedDays { get; set; }

        public string SevereCode { get; set; }

        public int LastFriScore { get; set; }

        public int LastScore { get; set; }

        public int CurrentScore { get; set; }

        public bool SkipImprovementCheck { get; set; }

        public string DrgCode { get; set; }

        public string SecondDrgCode { get; set; }

        public string ThirdDrgCode { get; set; }

        public int AutoApprovedVisits { get; set; }

        public int ApprovedVisits { get; set; }

        public int ApprovedDays { get; set; }

        public bool BenefitMaxApplied { get; set; }

        public bool LimitToYear
        {
            get { return GroupId.Contains("BCBSMA_UM"); }
        }

        public string AuthNotes { get; set; }

        public int OnSetDatePoints { get; set; }

        public int AdditionalVisits { get; set; }

        public string ApporvedBy { get; set; }

        public string DenialLetterPrinted { get; set; }

        public DateTime DenialLetterDate { get; set; }

        public string DenialLetterUser { get; set; }

        public string CaseNumber { get; set; }

        public int NewOrExtended { get; set; }

        public string AuditFlag { get; set; }

        public string AuthTypeId { get; set; }

        public decimal LastPsfsScore { get; set; }

        public int PlanVisitMaxInitial { get; set; }

        public int PlanVisitMaxContinuation { get; set; }

        public bool InternalPlanMaxApplied { get; set; }

        public int CareRegistrationVisits { get; set; }

        public bool IsMedicareAdvantageMember { get; set; }

        public string SelectedServiceType { get; set; }

        public int FailToPreAuthDays { get; set; }

        public int VisitsRequested { get; set; }
    }
}
