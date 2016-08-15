using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    public interface IAuthReviewChecker
    {
        ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList);
    }
}
