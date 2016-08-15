using System;
using System.Collections.Generic;
using RRS.BEL;
using RRS.DAL;

namespace RRS.BPL.AuthReviewChecker
{
    public class WorkInjuryChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            ValidationResult vr;

            try
            {
                PendCode pc;
                if (member.AuthTypeId.Equals("DC"))
                {
                    var chiroRequest = (ChiroAuthRequest)request;
                    pc = PendCodeAdapter.CheckChiroWorkInjury(chiroRequest, member);
                }
                else
                {
                    var ptotRequest = (PtotAuthRequest) request;
                    pc = PendCodeAdapter.CheckPtotWorkInjury(ptotRequest, member);
                }

                vr = new ValidationResult(true, string.Empty);                    

                if (!pc.AuthCode.Equals(string.Empty))
                {
                    pendList.Add(pc);
                }
            }
            catch (Exception ex)
            {
                var errMsg = "Error checking member injury type due to: " + ex.Message;
                vr = new ValidationResult(false, errMsg);
            }

            return vr;
        }
    }
}
