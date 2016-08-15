using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    public class DrgChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            if (member.DrgCode.Equals("999"))
            {
                var pc = new PendCode { AuthCode = "DG", AuthStatus = "RCR" };
                pendList.Add(pc);
            }

            var vr = new ValidationResult(true, string.Empty);
            return vr;
        }
    }
}
