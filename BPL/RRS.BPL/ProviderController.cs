using System;
using System.Collections.Generic;
using RRS.BEL;
using RRS.DAL;

namespace RRS.BPL
{
    public class ProviderController
    {
        public static ValidationResult ValidateProvider(string code, bool isIvr, string userId, string password, 
                                            ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = ProviderAdapter.CheckProvider(code, isIvr, userId, password);
                if (isIvr)
                {
                    if (int.Parse(result["err_num"])==0)
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
                    if (result["err_num"].Trim().ToUpper()=="Y")
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

        public static ValidationResult ValidateProviderFromSso(string clientPrefix, string id, DateTime requestedStartDate,  
                                            ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = ProviderAdapter.CheckProviderFromSso(clientPrefix, id, requestedStartDate);
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

        public static ValidationResult AngelValidateProvider(string accessCode, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = ProviderAdapter.AngelCheckProvider(accessCode);
                if (int.Parse(result["err_num"]) != 0)
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

        public static ValidationResult ValidateBcbsmaSso(string bcbsmaProviderId, ref Dictionary<string, string> result)
        {
            ValidationResult vr;

            try
            {
                result = ProviderAdapter.ValidateBcbsmaSso(bcbsmaProviderId);
                if (!result["errormessage"].Equals("Available"))
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
