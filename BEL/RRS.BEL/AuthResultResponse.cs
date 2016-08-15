using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class AuthResultResponse : Response
    {
        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string AuthTypeId { get; set; }

        [DataMember]
        public string SubscriberId { get; set; }

        [DataMember]
        public string MemberSeq { get; set; }

        [DataMember]
        public string GroupId { get; set; }

        [DataMember]
        public string DivisionId { get; set; }

        [DataMember]
        public DateTime GroupCoverageStart { get; set; }

        [DataMember]
        public string NetworkId { get; set; }

        [DataMember]
        public string ProviderId { get; set; }

        [DataMember]
        public string ProviderLocationSeq { get; set; }

        [DataMember]
        public string ReferredById { get; set; }

        [DataMember]
        public string ReferredByName { get; set; }

        [DataMember]
        public string CaseIndexRequested { get; set; }

        [DataMember]
        public int VisitsRequested { get; set; }

        [DataMember]
        public bool PreviousTreatment { get; set; }

        [DataMember]
        public string OtherInsurance { get; set; }

        [DataMember]
        public string TreatmentType { get; set; }

        [DataMember]
        public string DiagCode1 { get; set; }

        [DataMember]
        public string DiagCode2 { get; set; }

        [DataMember]
        public bool NewOrExtended { get; set; }

        [DataMember]
        public string CaseIndexApproved { get; set; }

        [DataMember]
        public int VisitsApproved { get; set; }

        [DataMember]
        public DateTime ApprovedFrom { get; set; }

        [DataMember]
        public DateTime ApprovedThru { get; set; }

        [DataMember]
        public decimal AmountApproved { get; set; }

        [DataMember]
        public string AuthCode { get; set; }

        [DataMember]
        public string ApprovedBy { get; set; }

        [DataMember]
        public int VisitsActual { get; set; }

        [DataMember]
        public decimal AmountActual { get; set; }

        [DataMember]
        public int FriScore { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public DateTime EntryDate { get; set; }

        [DataMember]
        public string EntryUser { get; set; }

        [DataMember]
        public DateTime UpdateDate { get; set; }

        [DataMember]
        public string UpdateUser { get; set; }

        [DataMember]
        public string CaseNumber { get; set; }

        [DataMember]
        public string DiagCode3 { get; set; }

        [DataMember]
        public string DiagCode4 { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public bool Closed { get; set; }

        [DataMember]
        public string CaseIndexActual { get; set; }

        [DataMember]
        public decimal AmountRequested { get; set; }

        [DataMember]
        public string DrgCategory { get; set; }

        [DataMember]
        public string DrgCode { get; set; }

        [DataMember]
        public DateTime RequestedFrom { get; set; }

        [DataMember]
        public DateTime RequestedThru { get; set; }

        [DataMember]
        public DateTime ApprovedDate { get; set; }

        [DataMember]
        public string Narrative { get; set; }

        [DataMember]
        public string WorkRelated { get; set; }

        [DataMember]
        public string AutoAccident { get; set; }

        [DataMember]
        public string AuditFlag { get; set; }

        [DataMember]
        public DateTime InjuryDate { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string TplanPrinted { get; set; }

        [DataMember]
        public DateTime TplanPrintedDate { get; set; }

        [DataMember]
        public string TplanPrintedUser { get; set; }

        [DataMember]
        public int VisitsToDate { get; set; }

        [DataMember]
        public int AdditionalVisits { get; set; }

        [DataMember]
        public string PlanId { get; set; }

        [DataMember]
        public string IvrFlag { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public DateTime FaxDate { get; set; }

        [DataMember]
        public string FaxUser { get; set; }

        [DataMember]
        public string FaxId { get; set; }

        [DataMember]
        public DateTime RecordsReceivedDate { get; set; }

        [DataMember]
        public string DenialLetterPrinted { get; set; }

        [DataMember]
        public DateTime DenialLetterDate { get; set; }

        [DataMember]
        public string DenialLetterUser { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public bool ExtensionGranted { get; set; }

        [DataMember]
        public bool HasDiabetes { get; set; }

        [DataMember]
        public bool HasStroke { get; set; }

        [DataMember]
        public bool HasCancer { get; set; }

        [DataMember]
        public bool IsOverweight { get; set; }

        [DataMember]
        public bool IsSmoker { get; set; }

        [DataMember]
        public bool HasChronicPain { get; set; }

        [DataMember]
        public string EdiAuthNumber { get; set; }

        [DataMember]
        public string BcbsmaNumber { get; set; }

        [DataMember]
        public string EdiCtrlNumber { get; set; }

        [DataMember]
        public string InUseBy { get; set; }

        [DataMember]
        public string OrderingProviderName { get; set; }

        [DataMember]
        public string OrderingProviderAddress { get; set; }

        [DataMember]
        public string OrderingProviderCity { get; set; }

        [DataMember]
        public string OrderingProviderState { get; set; }

        [DataMember]
        public string OrderingProviderZipCode { get; set; }

        [DataMember]
        public string OrderingProviderPhone { get; set; }

        [DataMember]
        public string OrderingProviderFax { get; set; }

        [DataMember]
        public string OrderingProviderDiagnosis { get; set; }

        [DataMember]
        public DateTime ReferralDate { get; set; }

        [DataMember]
        public bool PostSurgery { get; set; }

        [DataMember]
        public bool OtherInjury { get; set; }

        [DataMember]
        public bool NoInjury { get; set; }

        [DataMember]
        public string Duration { get; set; }

        [DataMember]
        public string PriorTreatment { get; set; }

        [DataMember]
        public bool UpperExtremity { get; set; }

        [DataMember]
        public bool LowerExtremity { get; set; }

        [DataMember]
        public bool LsSpine { get; set; }

        [DataMember]
        public bool CtSpine { get; set; }

        [DataMember]
        public bool HandWrist { get; set; }

        [DataMember]
        public bool OtherRegion { get; set; }

        [DataMember]
        public decimal PsfsScore { get; set; }

        [DataMember]
        public bool HasPainHistory { get; set; }

        [DataMember]
        public bool IsDrinker { get; set; }

        [DataMember]
        public bool TakeOpioids { get; set; }

        [DataMember]
        public bool HasExercise { get; set; }

        [DataMember]
        public bool HasConfidence { get; set; }

        [DataMember]
        public bool HasDepression { get; set; }

        [DataMember]
        public bool LimitCommunication { get; set; }

        [DataMember]
        public bool HasCns { get; set; }

        [DataMember]
        public bool HasCardiovascular { get; set; }

        [DataMember]
        public bool HasLungDisease { get; set; }
    }
}
