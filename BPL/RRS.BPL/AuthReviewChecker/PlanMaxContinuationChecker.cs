using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    class PlanMaxContinuationChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            if (request.ActualVisits >= member.PlanVisitMaxContinuation)
            {
                var pc = new PendCode { AuthCode = "TC", AuthStatus = "RCR" };
                pendList.Add(pc);
            }

            var vr = new ValidationResult(true, string.Empty);
            return vr;
        }
    }
}
