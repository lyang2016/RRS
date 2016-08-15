using System;
using System.Collections.Generic;
using RRS.BEL;
using RRS.DAL;

namespace RRS.BPL
{
    public class MemberController
    {
        public static ValidationResult ValidateMember(string code, bool isIvr, string memberId, DateTime dateOfBirth, 
                                            DateTime startDate, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.CheckMember(code, isIvr, memberId, dateOfBirth, startDate);
                if (isIvr)
                {
                    if (int.Parse(result["err_num"]) == 0)
                    {
                        vr = new ValidationResult(true, string.Empty);
                    }
                    else
                    {
                        vr = new ValidationResult(false, result["errormessage"]);
                    }
                }
                else
                {
                    if (result["err_num"].Trim().ToUpper() == "Y")
                    {
                        vr = new ValidationResult(false, result["errormessage"]);
                    }
                    else
                    {
                        vr = new ValidationResult(true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }

        public static ValidationResult ValidateMemberFromWeb(string code, string subscriberId, string memberSeq, string authTypeId, 
                        DateTime dateOfBirth, DateTime startDate, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.CheckMemberFromWeb(code, subscriberId, memberSeq, authTypeId, dateOfBirth, startDate);

                    if (result["err_num"].Trim().ToUpper() == "Y")
                    {
                        vr = new ValidationResult(false, result["errormessage"]);
                    }
                    else
                    {
                        vr = new ValidationResult(true, string.Empty);
                    }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }

        public static ValidationResult PreCheckMemberInfo(string ivrCode, string subscriberId, string memberSeq, 
                        DateTime startDate, DateTime dateOfBirth, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.PreCheckMemeberInfo(ivrCode, subscriberId, memberSeq, startDate, dateOfBirth);

                if (result["err_num"].Trim().ToUpper() == "Y")
                {
                    vr = new ValidationResult(false, result["errormessage"]);
                }
                else
                {
                    vr = new ValidationResult(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }

        public static ValidationResult GetAuthTypeFromSelectedServiceType(string prefixSubscriberId, string memberSeq,
                        DateTime startDate, string serviceType, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.GetAuthTypeFromSelectedServiceType(prefixSubscriberId, memberSeq, startDate, serviceType);

                if (result["err_num"].Trim().ToUpper() == "Y")
                {
                    vr = new ValidationResult(false, result["errormessage"]);
                }
                else
                {
                    vr = new ValidationResult(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }

        public static ValidationResult ValidateMemberFromSso(string clientPrefix, string subscriberId, string blindKey,  
                                                                ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.CheckMemberFromSso(clientPrefix, subscriberId, blindKey);

                if (result["err_num"].Trim().ToUpper() == "Y")
                {
                    vr = new ValidationResult(false, result["errormessage"]);
                }
                else
                {
                    vr = new ValidationResult(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }

        public static ValidationResult ValidateMemberPtotAuthType(string code, string subscriberId, string memberSeq, 
                        DateTime startDate, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.CheckMemberPtotAuthType(code, subscriberId, memberSeq, startDate);

                if (result["err_num"].Trim().ToUpper() == "Y")
                {
                    vr = new ValidationResult(false, result["errormessage"]);
                }
                else
                {
                    vr = new ValidationResult(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }

        public static ValidationResult ValidateBcbsmaMember(string providerId, string subscriberId, string memberSeq, 
                        DateTime dateOfBirth, DateTime startDate, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = MemberAdapter.CheckBcbsmaMember(providerId, subscriberId, memberSeq, dateOfBirth, startDate);

                if (DbHelper.NullString(result["errormessage"]) != string.Empty)
                {
                    vr = new ValidationResult(false, result["errormessage"]);
                }
                else
                {
                    vr = new ValidationResult(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                vr = new ValidationResult(false, ex.Message);
            }

            return vr;
        }
    }
}
