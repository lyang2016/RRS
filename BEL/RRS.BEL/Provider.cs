using System;

namespace RRS.BEL
{
    [Serializable]
    public class Provider
    {
        public string Id { get; set; }

        public string LocationSeq { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return FirstName + ' ' + LastName; }
        }

        public bool AuthWaiver { get; set; }

        public string Suffix { get; set; }

        public string NetworkId { get; set; }
    }
}
