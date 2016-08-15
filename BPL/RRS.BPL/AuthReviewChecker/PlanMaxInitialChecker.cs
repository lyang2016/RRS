using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    class PlanMaxInitialChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            if (request.ActualVisits >= member.PlanVisitMaxInitial)
            {
                var pc = new PendCode { AuthCode = "TI", AuthStatus = "RCR" };
                pendList.Add(pc);
            }

            var vr = new ValidationResult(true, string.Empty);
            return vr;
        }
    }
}
