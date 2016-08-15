using System;
using System.Collections.Generic;
using RRS.BEL;
using RRS.DAL;

namespace RRS.BPL
{
    public class DiagCodeController
    {
       public static ValidationResult ValidateDiagCode(string code, bool isIvr, ref Dictionary<string, string> result)
       {
           ValidationResult vr;

           try
           {
               result = DiagCodeAdapter.CheckDiagCode(code, isIvr);
               vr = result["returnval"].Trim().ToUpper() == "TRUE" ? new ValidationResult(true, string.Empty) : new ValidationResult(false, "Invalid Diag Code");
           }
           catch(Exception ex)
           {
               vr = new ValidationResult(false, ex.Message);
           }

           return vr;
       }
    }
}
