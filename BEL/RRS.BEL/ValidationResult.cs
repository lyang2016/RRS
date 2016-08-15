using System;

namespace RRS.BEL
{
    [Serializable]
    public class ValidationResult
    {
        public ValidationResult(bool success, string reason)
        {
            _success = success;
            _reason = reason;
        }

        private readonly bool _success;
        public bool Success
        {
            get
            {
                return _success;
            }

        }

        private readonly string _reason;
        public string Reason
        {
            get
            {
                return _reason;
            }
        }
    }
}
