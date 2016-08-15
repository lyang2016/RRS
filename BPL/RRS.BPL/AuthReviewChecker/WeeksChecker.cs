using System;
using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    public class WeeksChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            if (request.NumberOfVisits > 4 && request.NumberOfVisits <= request.NumberOfWeeks/2)
            {
                var pc = new PendCode { AuthCode = "WR", AuthStatus = "RCR" };
                pendList.Add(pc);
            }

            var vr = new ValidationResult(true, string.Empty);
            return vr;
        }
    }
}
