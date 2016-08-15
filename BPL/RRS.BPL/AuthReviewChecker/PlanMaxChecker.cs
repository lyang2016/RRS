using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    public class PlanMaxChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            if (request.ActualVisits >= request.MaxVisits)
            {
                var pc = new PendCode {AuthCode = "AU", AuthStatus = "RCR"};
                pendList.Add(pc);
            }

            var vr = new ValidationResult(true, string.Empty);
            return vr;            
        }
    }
}
