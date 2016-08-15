using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class PtotAuthRequest : AuthRequest
    {
        [DataMember]
        public bool WorkRelated { get; set; }

        [DataMember]
        public bool AutoAccident { get; set; }

        [DataMember]
        public bool OtherInjury { get; set; }

        [DataMember]
        public bool PostSurgery { get; set; }

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
        public bool HasConfidence { get; set; }

        [DataMember]
        public bool LimitCommunication { get; set; }

        [DataMember]
        public bool HasCns { get; set; }

        [DataMember]
        public bool HasCardiovascular { get; set; }

        [DataMember]
        public bool HasLungDisease { get; set; }

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
        public string RealAuthTypeId { get; set; }

        [DataMember]
        public string ServiceType { get; set; }
    }
}
