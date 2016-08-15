using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class ChiroAuthRequest : AuthRequest
    {
        /* Removed according to the new DC template (WI #12118 & 12402)
        [DataMember]
        public DateTime InitialInjuryDate { get; set; }
         */

        [DataMember]
        public int VisitsToDate { get; set; }

        [DataMember]
        public string ThirdDiagnosisCode { get; set; }

        /* Removed according to the new DC template (WI #12118 & 12402)
        [DataMember]
        public int DailyLivingRating { get; set; }

        [DataMember]
        public int RangeOfMotionRating { get; set; }
         */

        [DataMember]
        public bool IsCoTreat { get; set; }

        /* Removed according to the new DC template (WI #12118 & 12402)
        [DataMember]
        public int FriScore { get; set; }
         */

        [DataMember]
        public bool HasStroke { get; set; }

        /* Removed according to the new DC template (WI #12118 & 12402)
        [DataMember]
        public bool HasChronicPain { get; set; }
         */

        [DataMember]
        public string PriorAuthNumber { get; set; }


        // moved from AuthRequest

        /* Removed according to the new DC template (WI #12118 & 12402)
        [DataMember]
        public string InjuryType { get; set; }
         */

        [DataMember(Order = 42)]
        public string ServiceType { get; set; }

        [DataMember(Order = 43)]
        public bool WorkRelated { get; set; }

        [DataMember(Order = 44)]
        public bool AutoAccident { get; set; }

        [DataMember(Order = 45)]
        public bool OtherInjury { get; set; }

        [DataMember(Order = 46)]
        public bool PostSurgery { get; set; }

        [DataMember(Order = 47)]
        public bool NoInjury { get; set; }

        [DataMember(Order = 48)]
        public string Duration { get; set; }

        [DataMember(Order = 49)]
        public bool LimitCommunication { get; set; }

        [DataMember(Order = 50)]
        public bool HasPainHistory { get; set; }

        [DataMember(Order = 51)]
        public decimal PsfsScore { get; set; }

    }
}
