﻿using System;
using System.Collections.Generic;
using RRS.BEL;
using RRS.DAL;

namespace RRS.BPL.AuthReviewChecker
{
    public class GroupPendChecker : IAuthReviewChecker
    {
        public ValidationResult CheckAuthReview(AuthRequest request, Member member, Provider provider, ref List<PendCode> pendList)
        {
            ValidationResult vr;

            try
            {
                var pc = PendCodeAdapter.CheckGroupPend(member);
                vr = new ValidationResult(true, string.Empty);

                if (!pc.AuthCode.Equals(string.Empty))
                {
                    pendList.Add(pc);
                }
            }
            catch (Exception ex)
            {
                var errMsg = "Error checking group pended due to: " + ex.Message;
                vr = new ValidationResult(false, errMsg);
            }

            return vr;
        }
    }
}
