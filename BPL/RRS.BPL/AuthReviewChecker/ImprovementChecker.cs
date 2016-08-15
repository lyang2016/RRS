using System;
using System.Collections.Generic;
using RRS.BEL;

namespace RRS.BPL.AuthReviewChecker
{
    public class ImprovementChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            ValidationResult vr;

            try
            {
                PendCode pc;
                if (member.AuthTypeId.Equals("DC"))
                {
                    var chiroRequest = (ChiroAuthRequest) request;

                    /* Removed according to the new DC template (WI #12118 & 12402) 
                    if (member.LastFriScore >= 1 && member.LastFriScore <= 40)
                    {
                        if (member.LastFriScore - chiroRequest.FriScore <= 4)
                        {
                            pc = new PendCode { AuthCode = "EP", AuthStatus = "RCR" };
                            pendList.Add(pc);
                        }
                    }
                    else
                    {
                        if (member.CurrentScore >= member.LastScore)
                        {
                            pc = new PendCode { AuthCode = "EP", AuthStatus = "RCR" };
                            pendList.Add(pc);
                        }
                    }  
                     */

                    if (member.LastPsfsScore >= 0m && member.LastPsfsScore <= 10m)
                    {
                        if (chiroRequest.PsfsScore - member.LastPsfsScore < 2m)
                        {
                            pc = new PendCode { AuthCode = "EP", AuthStatus = "RCR" };
                            pendList.Add(pc);
                        }
                    }
                    else
                    {
                        if (member.CurrentScore >= member.LastScore)
                        {
                            pc = new PendCode { AuthCode = "EP", AuthStatus = "RCR" };
                            pendList.Add(pc);
                        }
                    }
                }
                else
                {
                    var ptotRequest = (PtotAuthRequest) request;

                    if (member.LastPsfsScore >= 0m && member.LastPsfsScore <= 10m)
                    {
                        if (ptotRequest.PsfsScore - member.LastPsfsScore < 2m)
                        {
                            pc = new PendCode { AuthCode = "EP", AuthStatus = "RCR" };
                            pendList.Add(pc);
                        }
                    }
                    else
                    {
                        if (member.CurrentScore >= member.LastScore)
                        {
                            pc = new PendCode { AuthCode = "EP", AuthStatus = "RCR" };
                            pendList.Add(pc);
                        }
                    }
                }

                vr = new ValidationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                var errMsg = "Error checking improvement for continuation due to: " + ex.Message;
                vr = new ValidationResult(false, errMsg);
            }

            return vr;
        }
    }
}
