using System;
using System.Runtime.Serialization;

namespace RRS.BEL
{
    [DataContract]
    [Serializable]
    public class ProviderResponse : Response
    {
        [DataMember]
        public string ProviderId { get; set; }

        [DataMember]
        public string LocationSeq { get; set; }

        [DataMember]
        public string ProviderName { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string Fax { get; set; }

        [DataMember]
        public string TinLast4 { get; set; }

        [DataMember]
        public bool IvrRight1 { get; set; }

        [DataMember]
        public bool IvrRight2 { get; set; }

        [DataMember]
        public bool IvrRight3 { get; set; }

        [DataMember]
        public bool IvrRight4 { get; set; }

        [DataMember]
        public bool IvrRight5 { get; set; }

        [DataMember]
        public bool IvrRight6 { get; set; }

        [DataMember]
        public bool IvrRight7 { get; set; }

        [DataMember]
        public bool IsBcbsma { get; set; }

        [DataMember]
        public bool PlayBcbsmaIntro { get; set; }

        [DataMember]
        public bool PlayExcludeAlphaPrefix { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string IvrCode { get; set; }

        [DataMember]
        public string Suffix { get; set; }

        [DataMember]
        public int ErrorNumber { get; set; }

        [DataMember]
        public string AuthTypeId { get; set; }
    }
}
