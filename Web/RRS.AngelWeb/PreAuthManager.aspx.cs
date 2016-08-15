using System;
using System.Data;
using System.Text;
using System.Configuration;
using RRS.AngelWeb.RRSService;
using NLog;

namespace RRS.AngelWeb
{
    public partial class PreAuthManager : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetLogger("PreAuthManager");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var method = RequestHelper.NullString(Request["Method"]);
                var result = string.Empty;

                switch (method)
                {
                    case "CheckProvider":
                        result = CheckProvider();
                        break;
                    case "CheckInsuranceId":
                        result = CheckInsuranceId();
                        break;
                    case "GetClaimsAddress":
                        result = GetClaimsAddress();
                        break;
                    case "GetServiceTypes":
                        result = GetServiceTypes();
                        break;
                    case "GetAuthTypeFromSelectedServiceType":
                        result = GetAuthTypeFromSelectedServiceType();
                        break;
                    case "CheckMember":
                        result = CheckMember();
                        break;
                    case "GetChiroPreAuth":
                        result = GetChiroPreAuth();
                        break;
                    case "GetPTOTPreAuth":
                        result = GetPtotPreAuth();
                        break;
                    case "CheckDiagCode":
                        result = CheckDiagCode();
                        break;
                    case "UpdateProviderFax":
                        result = UpdateProviderFax();
                        break;
                }

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/xml";
                Response.Write(result);
                Response.Flush();
                Response.Close();
            }
        }

        private string CheckProvider()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;

            var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
            if (enableLogging)
            {
                var logTime = DateTime.Now;
                Logger.Error("*************************************   CheckProvider is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                Logger.Info("The original submitted data for the above CheckProvider are listed below:");
                Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
            }

            var accessCode = RequestHelper.NullString(Request["AccessCode"]);
            var result = client.CheckProviderFromAngel(accessCode);

            if (!result.IsValid)
            {
                returnCode = "199";
                errorMessage = result.ErrorMessage;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("ProviderId", RequestHelper.NullString(result.ProviderId), ref sb);
                AngelXmlHelper.GenerateVariable("LocationSeq", RequestHelper.NullString(result.LocationSeq), ref sb);
                AngelXmlHelper.GenerateVariable("ZipCode", RequestHelper.NullString(result.ZipCode), ref sb);
                AngelXmlHelper.GenerateVariable("TinLast4", RequestHelper.NullString(result.TinLast4), ref sb);
                AngelXmlHelper.GenerateVariable("Fax", RequestHelper.NullString(result.Fax), ref sb);
                AngelXmlHelper.GenerateVariable("IsBcbsma", RequestHelper.NullString(result.IsBcbsma), ref sb);
                AngelXmlHelper.GenerateVariable("PlayBcbsmaIntro", RequestHelper.NullString(result.PlayBcbsmaIntro), ref sb);
                AngelXmlHelper.GenerateVariable("PlayExcludeAlphaPrefix", RequestHelper.NullString(result.PlayExcludeAlphaPrefix), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight1", RequestHelper.NullString(result.IvrRight1), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight2", RequestHelper.NullString(result.IvrRight2), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight3", RequestHelper.NullString(result.IvrRight3), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight4", RequestHelper.NullString(result.IvrRight4), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight5", RequestHelper.NullString(result.IvrRight5), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight6", RequestHelper.NullString(result.IvrRight6), ref sb);
                AngelXmlHelper.GenerateVariable("IvrRight7", RequestHelper.NullString(result.IvrRight7), ref sb);
                AngelXmlHelper.GenerateVariable("NpiLast4", RequestHelper.NullString(result.NpiLast4), ref sb);
                AngelXmlHelper.GenerateVariable("IsHighmark", RequestHelper.NullString(result.IsHighmark), ref sb);
                AngelXmlHelper.GenerateVariable("IsDean", RequestHelper.NullString(result.IsDean), ref sb);                
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();
        }

        private string CheckInsuranceId()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;
            var ds = new DataSet();
            var seqList = string.Empty;
            var dobList = string.Empty;
            var idList = string.Empty;
            int i;

            try
            {
                var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
                if (enableLogging)
                {
                    var logTime = DateTime.Now;
                    Logger.Error("*************************************   CheckInsuranceId is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                    Logger.Info("The original submitted data for the above CheckInsuranceId are listed below:");
                    Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
                    Logger.Info("Subscriber Id={0}", RequestHelper.NullString(Request["MainSubscriberID"]));
                    Logger.Info("Start Date={0}", RequestHelper.NullString(Request["StartDate"]));                    
                }

                var accessCode = RequestHelper.NullString(Request["AccessCode"]);
                var subscriberId = RequestHelper.NullString(Request["MainSubscriberID"]);
                var startDate = RequestHelper.NullDateTime(Request["StartDate"]);
                ds = client.CheckMemberFromAngel(accessCode, subscriberId, startDate);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                {
                    returnCode = "199";
                    errorMessage = "No member found.";
                }
            }
            catch (Exception ex)
            {
                returnCode = "199";
                errorMessage = ex.Message;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);

            if (returnCode == "100")
            {
                for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    seqList += RequestHelper.NullString(ds.Tables[0].Rows[i]["Member_Seq"]) + ",";
                    dobList += RequestHelper.GetSqlDateFormat(RequestHelper.NullDateTime(ds.Tables[0].Rows[i]["Birth_Date"])) + ",";
                    idList += RequestHelper.NullString(ds.Tables[0].Rows[i]["Prefix_Subscriber_Id"]) + ",";
                }

                if (seqList.Length > 0)
                {
                    seqList = seqList.Substring(0, seqList.Length - 1);
                }

                if (dobList.Length > 0)
                {
                    dobList = dobList.Substring(0, dobList.Length - 1);
                }

                if (idList.Length > 0)
                {
                    idList = idList.Substring(0, idList.Length - 1);
                }

                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("MemberSequence", seqList, ref sb);
                AngelXmlHelper.GenerateVariable("MemberDOB", dobList, ref sb);
                AngelXmlHelper.GenerateVariable("PrefixSubscriberID", idList, ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }

            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();
        }

        private string GetClaimsAddress()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;
            var result = new MemberResponse();

            try
            {
                var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
                if (enableLogging)
                {
                    var logTime = DateTime.Now;
                    Logger.Error("*************************************   GetClaimsAddress is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                    Logger.Info("The original submitted data for the above GetClaimsAddress are listed below:");
                    Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
                    Logger.Info("Prefix Subscriber Id={0}", RequestHelper.NullString(Request["PrefixSubscriberId"]));
                    Logger.Info("Member Seq={0}", RequestHelper.NullString(Request["MemberSeq"]));
                }

                var accessCode = RequestHelper.NullString(Request["AccessCode"]);
                var prefixSubscriberId = RequestHelper.NullString(Request["PrefixSubscriberId"]);
                var memberSeq = RequestHelper.NullString(Request["MemberSeq"]);
                result = client.GetClaimsAddress(accessCode, prefixSubscriberId, memberSeq);
            }
            catch (Exception ex)
            {
                returnCode = "199";
                errorMessage = ex.Message;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("ClaimsAddressId", RequestHelper.NullString(result.ClaimsAddressId), ref sb);
                AngelXmlHelper.GenerateVariable("ClaimsAddress", RequestHelper.NullString(result.ClaimsAddress), ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();
        }

        private string GetServiceTypes()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;
            var ds = new DataSet();
            var idList = string.Empty;
            var descList = string.Empty;
            int i;

            try
            {
                var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
                if (enableLogging)
                {
                    var logTime = DateTime.Now;
                    Logger.Error("*************************************   GetServiceTypes is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                    Logger.Info("The original submitted data for the above GetServiceTypes are listed below:");
                    Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
                    Logger.Info("Prefix Subscriber Id={0}", RequestHelper.NullString(Request["PrefixSubscriberId"]));
                    Logger.Info("Member Seq={0}", RequestHelper.NullString(Request["MemberSeq"]));
                    Logger.Info("Start Date={0}", RequestHelper.NullString(Request["StartDate"]));
                }

                var accessCode = RequestHelper.NullString(Request["AccessCode"]);
                var prefixSubscriberId = RequestHelper.NullString(Request["PrefixSubscriberId"]);
                var memberSeq = RequestHelper.NullString(Request["MemberSeq"]);
                var startDate = RequestHelper.NullDateTime(Request["StartDate"]);
                ds = client.GetAllServiceType(accessCode, prefixSubscriberId, memberSeq, startDate);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                {
                    returnCode = "199";
                    errorMessage = "No service type found.";
                }
            }
            catch (Exception ex)
            {
                returnCode = "199";
                errorMessage = ex.Message;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);

            if (returnCode == "100")
            {
                for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    idList += RequestHelper.NullString(ds.Tables[0].Rows[i]["ServiceType_ID"]) + ",";
                    descList += RequestHelper.NullString(ds.Tables[0].Rows[i]["Description"]) + ",";
                }

                if (descList.Length > 0)
                {
                    descList = descList.Substring(0, descList.Length - 1);
                }

                if (idList.Length > 0)
                {
                    idList = idList.Substring(0, idList.Length - 1);
                }

                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("ServiceTypeId", idList, ref sb);
                AngelXmlHelper.GenerateVariable("ServiceDescription", descList, ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }

            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();   
        }

        private string GetAuthTypeFromSelectedServiceType()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;

            var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
            if (enableLogging)
            {
                var logTime = DateTime.Now;
                Logger.Error("*************************************   GetAuthTypeFromSelectedServiceType is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                Logger.Info("The original submitted data for the above GetAuthTypeFromSelectedServiceType are listed below:");
                Logger.Info("PrefixSubscriber Id={0}", RequestHelper.NullString(Request["PrefixSubscriberId"]));
                Logger.Info("Member Seq={0}", RequestHelper.NullString(Request["MemberSeq"]));
                Logger.Info("Start Date={0}", RequestHelper.NullString(Request["StartDate"]));
                Logger.Info("Service Type={0}", RequestHelper.NullString(Request["ServiceType"]));                    
            }

            var prefixSubscriberId = RequestHelper.NullString(Request["PrefixSubscriberId"]);
            var memberSeq = RequestHelper.NullString(Request["MemberSeq"]);
            var startDate = RequestHelper.NullDateTime(Request["StartDate"]);
            var serviceType = RequestHelper.NullString(Request["ServiceType"]);

            var result = client.GetAuthTypeFromSelectedServiceType(prefixSubscriberId, memberSeq, startDate, serviceType);

            if (!result.IsValid)
            {
                returnCode = "199";
                errorMessage = result.ErrorMessage;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("RealAuthTypeId", RequestHelper.NullString(result.RealAuthTypeId), ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();            
        }

        private string CheckMember()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;

            var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
            if (enableLogging)
            {
                var logTime = DateTime.Now;
                Logger.Error("*************************************   CheckMember is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                Logger.Info("The original submitted data for the above CheckMember are listed below:");
                Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
                Logger.Info("Subscriber Id={0}", RequestHelper.NullString(Request["MainSubscriberId"]));
                Logger.Info("Member Seq={0}", RequestHelper.NullString(Request["MemberSeq"]));
                Logger.Info("Auth Type={0}", RequestHelper.NullString(Request["AuthTypeId"]));
                Logger.Info("Date Of Birth={0}", RequestHelper.NullString(Request["DateOfBirth"]));
                Logger.Info("Start Date={0}", RequestHelper.NullString(Request["StartDate"]));
            }

            var accessCode = RequestHelper.NullString(Request["AccessCode"]);
            var subscriberId = RequestHelper.NullString(Request["MainSubscriberId"]);
            var memberSeq = RequestHelper.NullString(Request["MemberSeq"]);
            var authTypeId = RequestHelper.NullString(Request["AuthTypeId"]);
            var dateOfBirth = RequestHelper.NullDateTime(Request["DateOfBirth"]);
            var startDate = RequestHelper.NullDateTime(Request["StartDate"]);

            var result = client.CheckMemberFromWeb(accessCode, subscriberId, memberSeq, authTypeId, dateOfBirth, startDate);

            if (!result.IsValid)
            {
                returnCode = "199";
                errorMessage = result.ErrorMessage;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("MemberName", RequestHelper.NullString(result.MemberName), ref sb);
                AngelXmlHelper.GenerateVariable("MaxVisits", RequestHelper.NullString(result.MaxVisits), ref sb);
                AngelXmlHelper.GenerateVariable("ActualVisits", RequestHelper.NullString(result.ActualVisits), ref sb);
                AngelXmlHelper.GenerateVariable("PlanId", RequestHelper.NullString(result.PlanId), ref sb);
                AngelXmlHelper.GenerateVariable("ActualVisitStartDate", RequestHelper.GetSqlDateFormat(result.ActualVisitStartDate), ref sb);
                AngelXmlHelper.GenerateVariable("ActualVisitEndDate", RequestHelper.GetSqlDateFormat(result.ActualVisitEndDate), ref sb);
                AngelXmlHelper.GenerateVariable("VisitYearType", RequestHelper.NullString(result.VisitYearType), ref sb);
                AngelXmlHelper.GenerateVariable("UnmanagedVisits", RequestHelper.NullString(result.UnmanagedVisits), ref sb);
                AngelXmlHelper.GenerateVariable("ClaimsAddressError", RequestHelper.NullString(result.ClaimsAddressError), ref sb);
                AngelXmlHelper.GenerateVariable("ClaimsAddressId", RequestHelper.NullString(result.ClaimsAddressId), ref sb);
                AngelXmlHelper.GenerateVariable("ClaimAddress", RequestHelper.NullString(result.ClaimsAddress), ref sb);
                AngelXmlHelper.GenerateVariable("DateOfBirth", RequestHelper.GetSqlDateFormat(result.DateOfBirth), ref sb);
                AngelXmlHelper.GenerateVariable("IsMedicareAdvantage", RequestHelper.NullString(result.IsMedicareAdvantage), ref sb);
                AngelXmlHelper.GenerateVariable("PrefixSubscriberId", RequestHelper.NullString(result.PrefixSubscriberId), ref sb);
                AngelXmlHelper.GenerateVariable("UseCareRegistration", RequestHelper.NullString(result.UseCareRegistration), ref sb);
                AngelXmlHelper.GenerateVariable("RealAuthTypeId", RequestHelper.NullString(result.RealAuthTypeId), ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();                        
        }

        private string GetChiroPreAuth()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;
            var chiroRequest = new ChiroAuthRequest();

            var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
            if (enableLogging)
            {
                var logTime = DateTime.Now;
                Logger.Error("*************************************   GetChiroPreAuth is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                Logger.Info("The original submitted data for the above GetChiroPreAuth are listed below:");
                Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
                Logger.Info("Prefix Subscriber Id={0}", RequestHelper.NullString(Request["PrefixSubscriberId"]));
                Logger.Info("Member Seq={0}", RequestHelper.NullString(Request["MemberSeq"]));
                Logger.Info("Member ID={0}", RequestHelper.NullString(Request["MemberId"]));
                Logger.Info("Start Date={0}", RequestHelper.NullString(Request["StartDate"]));
                Logger.Info("Request Type={0}", RequestHelper.NullString(Request["RequestType"]));
                Logger.Info("Condition={0}", RequestHelper.NullString(Request["Condition"]));
                //Logger.Info("Injury Type={0}", RequestHelper.NullString(Request["InjuryType"]));
                Logger.Info("Work Related={0}", RequestHelper.NullString(Request["WorkRelated"]));
                Logger.Info("Auto Accident={0}", RequestHelper.NullString(Request["AutoAccident"]));
                Logger.Info("Other Injury={0}", RequestHelper.NullString(Request["OtherInjury"]));
                Logger.Info("Post Surgery={0}", RequestHelper.NullString(Request["PostSurgery"]));
                Logger.Info("No Injury={0}", RequestHelper.NullString(Request["NoInjury"]));
                //Logger.Info("Initial Injury Date={0}", RequestHelper.NullString(Request["InitialInjuryDate"]));
                Logger.Info("Duration={0}", RequestHelper.NullString(Request["Duration"]));
                Logger.Info("Initial Visit Date={0}", RequestHelper.NullString(Request["InitialVisitDate"]));
                Logger.Info("Visits To Date={0}", RequestHelper.NullString(Request["VisitsToDate"]));
                Logger.Info("Number Of Visits={0}", RequestHelper.NullString(Request["NumberOfVisits"]));
                Logger.Info("Number Of Weeks={0}", RequestHelper.NullString(Request["NumberOfWeeks"]));
                Logger.Info("Primary Diagnosis Code={0}", RequestHelper.NullString(Request["PrimaryDiagnosisCode"]));
                Logger.Info("Secondary Diagnosis Code={0}", RequestHelper.NullString(Request["SecondaryDiagnosisCode"]));
                Logger.Info("Third Diagnosis Code={0}", RequestHelper.NullString(Request["ThirdDiagnosisCode"]));
                //Logger.Info("Daily Living Rating={0}", RequestHelper.NullString(Request["DailyLivingRating"]));
                Logger.Info("Pain Rating={0}", RequestHelper.NullString(Request["PainRating"]));
                //Logger.Info("Range Of Motion Rating={0}", RequestHelper.NullString(Request["RangeOfMotionRating"]));
                Logger.Info("Limit Communication={0}", RequestHelper.NullString(Request["LimitCommunication"]));
                Logger.Info("Has Exercise={0}", RequestHelper.NullString(Request["HasExercise"]));
                Logger.Info("Is CoTreat={0}", RequestHelper.NullString(Request["IsCoTreat"]));
                //Logger.Info("Fri Score={0}", RequestHelper.NullString(Request["FriScore"]));
                Logger.Info("Psfs Score={0}", RequestHelper.NullString(Request["PsfsScore"]));
                Logger.Info("Has Diabetes={0}", RequestHelper.NullString(Request["HasDiabetes"]));
                Logger.Info("Has Stroke={0}", RequestHelper.NullString(Request["HasStroke"]));
                Logger.Info("Has Cancer={0}", RequestHelper.NullString(Request["HasCancer"]));
                Logger.Info("Is Smoker={0}", RequestHelper.NullString(Request["IsSmoker"]));
                Logger.Info("Is Overweight={0}", RequestHelper.NullString(Request["IsOverweight"]));
                //Logger.Info("Has ChronicPain={0}", RequestHelper.NullString(Request["HasChronicPain"]));
                Logger.Info("Has Pain History={0}", RequestHelper.NullString(Request["HasPainHistory"]));
                Logger.Info("Has Depression={0}", RequestHelper.NullString(Request["HasDepression"]));
                Logger.Info("Max Visits={0}", RequestHelper.NullString(Request["MaxVisits"]));
                Logger.Info("Actual Visits={0}", RequestHelper.NullString(Request["ActualVisits"]));
                Logger.Info("Actual Visit Start Date={0}", RequestHelper.NullString(Request["ActualVisitStartDate"]));
                Logger.Info("Actual Visit End Date={0}", RequestHelper.NullString(Request["ActualVisitEndDate"]));
                Logger.Info("Visit Year Type={0}", RequestHelper.NullString(Request["VisitYearType"]));
                Logger.Info("Service Type={0}", RequestHelper.NullString(Request["ServiceType"]));
                Logger.Info("Accept Auth Reduction={0}", RequestHelper.NullString(Request["AcceptAuthReduction"]));
                Logger.Info("Send To Review={0}", RequestHelper.NullString(Request["SendToReview"]));
                Logger.Info("Application Id={0}", RequestHelper.NullString(Request["ApplicationId"]));
                Logger.Info("Prior Auth Number={0}", RequestHelper.NullString(Request["PriorAuthNumber"]));
            }

            chiroRequest.IvrCode = RequestHelper.NullString(Request["AccessCode"]);
            chiroRequest.SubscriberId = RequestHelper.NullString(Request["PrefixSubscriberId"]);
            chiroRequest.MemberSeq = RequestHelper.NullString(Request["MemberSeq"]);
            chiroRequest.MemberId = chiroRequest.SubscriberId + chiroRequest.MemberSeq;
            chiroRequest.StartDate = RequestHelper.NullDateTime(Request["StartDate"]);
            chiroRequest.RequestType = RequestHelper.NullString(Request["RequestType"]);
            chiroRequest.Condition = RequestHelper.NullString(Request["Condition"]);
            //chiroRequest.InjuryType = RequestHelper.NullString(Request["InjuryType"]);
            chiroRequest.WorkRelated = RequestHelper.NullBool(Request["WorkRelated"]);
            chiroRequest.AutoAccident = RequestHelper.NullBool(Request["AutoAccident"]);
            chiroRequest.OtherInjury = RequestHelper.NullBool(Request["OtherInjury"]);
            chiroRequest.PostSurgery = RequestHelper.NullBool(Request["PostSurgery"]);
            chiroRequest.NoInjury = RequestHelper.NullBool(Request["NoInjury"]);
            //chiroRequest.InitialInjuryDate = RequestHelper.NullDateTime(Request["InitialInjuryDate"]);
            chiroRequest.Duration = RequestHelper.NullString(Request["Duration"]);
            chiroRequest.InitialVisitDate = RequestHelper.NullDateTime(Request["InitialVisitDate"]);
            chiroRequest.VisitsToDate = RequestHelper.NullInt(Request["VisitsToDate"]);
            chiroRequest.NumberOfVisits = RequestHelper.NullInt(Request["NumberOfVisits"]);
            chiroRequest.NumberOfWeeks = RequestHelper.NullInt(Request["NumberOfWeeks"]);
            chiroRequest.PrimaryDiagnosisCode = RequestHelper.NullString(Request["PrimaryDiagnosisCode"]);
            chiroRequest.SecondaryDiagnosisCode = RequestHelper.NullString(Request["SecondaryDiagnosisCode"]);
            chiroRequest.ThirdDiagnosisCode = RequestHelper.NullString(Request["ThirdDiagnosisCode"]);
            //chiroRequest.DailyLivingRating = RequestHelper.NullInt(Request["DailyLivingRating"]);
            chiroRequest.PainRating = RequestHelper.NullInt(Request["PainRating"]);
            //chiroRequest.RangeOfMotionRating = RequestHelper.NullInt(Request["RangeOfMotionRating"]);
            chiroRequest.LimitCommunication = RequestHelper.NullBool(Request["LimitCommunication"]);
            chiroRequest.HasExercise = RequestHelper.NullBool(Request["HasExercise"]);
            chiroRequest.IsCoTreat = RequestHelper.NullBool(Request["IsCoTreat"]);
            //chiroRequest.FriScore = RequestHelper.NullInt(Request["FriScore"]);
            chiroRequest.PsfsScore = RequestHelper.NullDecimal(Request["PsfsScore"]);
            chiroRequest.HasDiabetes = RequestHelper.NullBool(Request["HasDiabetes"]);
            chiroRequest.HasStroke = RequestHelper.NullBool(Request["HasStroke"]);
            chiroRequest.HasCancer = RequestHelper.NullBool(Request["HasCancer"]);
            chiroRequest.IsSmoker = RequestHelper.NullBool(Request["IsSmoker"]);
            chiroRequest.IsOverweight = RequestHelper.NullBool(Request["IsOverweight"]);
            //chiroRequest.HasChronicPain = RequestHelper.NullBool(Request["HasChronicPain"]);
            chiroRequest.HasPainHistory = RequestHelper.NullBool(Request["HasPainHistory"]);
            chiroRequest.HasDepression = RequestHelper.NullBool(Request["HasDepression"]);
            chiroRequest.MaxVisits = RequestHelper.NullInt(Request["MaxVisits"]);
            chiroRequest.ActualVisits = RequestHelper.NullInt(Request["ActualVisits"]);
            chiroRequest.ActualVisitStartDate = RequestHelper.NullDateTime(Request["ActualVisitStartDate"]);
            chiroRequest.ActualVisitEndDate = RequestHelper.NullDateTime(Request["ActualVisitEndDate"]);
            chiroRequest.VisitYearType = RequestHelper.NullString(Request["VisitYearType"]);
            chiroRequest.ServiceType = RequestHelper.NullString(Request["ServiceType"]);
            chiroRequest.AcceptAuthReduction = RequestHelper.NullBool(Request["AcceptAuthReduction"]);
            chiroRequest.SendToReview = RequestHelper.NullBool(Request["SendToReview"]);
            chiroRequest.ApplicationId = RequestHelper.NullInt(Request["ApplicationId"]);
            chiroRequest.PriorAuthNumber = RequestHelper.NullString(Request["PriorAuthNumber"]);


            // Call GetChiroPreAuth() method
            var result = client.GetChiroPreAuth(chiroRequest);

            if (!result.IsValid)
            {
                returnCode = "199";
                errorMessage = result.ErrorMessage;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("AuthNumber", RequestHelper.NullString(result.AuthNumber), ref sb);
                AngelXmlHelper.GenerateVariable("VisitsApproved", RequestHelper.NullString(result.VisitsApproved), ref sb);
                AngelXmlHelper.GenerateVariable("DaysApproved", RequestHelper.NullString(result.DaysApproved), ref sb);
                AngelXmlHelper.GenerateVariable("DateApprovedFrom", RequestHelper.GetSqlDateFormat(result.DateApprovedFrom), ref sb);
                AngelXmlHelper.GenerateVariable("DateApprovedThru", RequestHelper.GetSqlDateFormat(result.DateApprovedThru), ref sb);
                AngelXmlHelper.GenerateVariable("DateApproved", RequestHelper.GetSqlDateFormat(result.DateApproved), ref sb);
                AngelXmlHelper.GenerateVariable("IsApproveReduction", RequestHelper.NullString(result.IsApproveReduction), ref sb);
                AngelXmlHelper.GenerateVariable("DecisionText", RequestHelper.NullString(result.DecisionText), ref sb);
                AngelXmlHelper.GenerateVariable("AuthCode", RequestHelper.NullString(result.AuthCode), ref sb);
                AngelXmlHelper.GenerateVariable("AuthStatus", RequestHelper.NullString(result.AuthStatus), ref sb);
                AngelXmlHelper.GenerateVariable("ClientAuthNumber", RequestHelper.NullString(result.ClientAuthNumber), ref sb);
                AngelXmlHelper.GenerateVariable("BcbsmaNumber", RequestHelper.NullString(result.BcbsmaNumber), ref sb);
                AngelXmlHelper.GenerateVariable("FailToPreAuthDays", RequestHelper.NullString(result.FailToPreAuthDays), ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();                        
        }

        private string GetPtotPreAuth()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;
            var ptotRequest = new PtotAuthRequest();

            var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
            if (enableLogging)
            {
                var logTime = DateTime.Now;
                Logger.Error("*************************************   GetPtotPreAuth is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                Logger.Info("The original submitted data for the above GetPtotPreAuth are listed below:");
                Logger.Info("Access Code={0}", RequestHelper.NullString(Request["AccessCode"]));
                Logger.Info("Prefix Subscriber Id={0}", RequestHelper.NullString(Request["PrefixSubscriberId"]));
                Logger.Info("Member Seq={0}", RequestHelper.NullString(Request["MemberSeq"]));
                Logger.Info("Member ID={0}", RequestHelper.NullString(Request["MemberId"]));
                Logger.Info("Has Referral={0}", RequestHelper.NullString(Request["HasReferral"]));
                Logger.Info("Start Date={0}", RequestHelper.NullString(Request["StartDate"]));
                Logger.Info("Request Type={0}", RequestHelper.NullString(Request["RequestType"]));
                Logger.Info("Condition={0}", RequestHelper.NullString(Request["Condition"]));
                Logger.Info("Work Related={0}", RequestHelper.NullString(Request["WorkRelated"]));
                Logger.Info("Auto Accident={0}", RequestHelper.NullString(Request["AutoAccident"]));
                Logger.Info("Other Injury={0}", RequestHelper.NullString(Request["OtherInjury"]));
                Logger.Info("Post Surgery={0}", RequestHelper.NullString(Request["PostSurgery"]));
                Logger.Info("No Injury={0}", RequestHelper.NullString(Request["NoInjury"]));
                Logger.Info("Duration={0}", RequestHelper.NullString(Request["Duration"]));
                Logger.Info("Initial Visit Date={0}", RequestHelper.NullString(Request["InitialVisitDate"]));
                Logger.Info("Prior Treatment={0}", RequestHelper.NullString(Request["PriorTreatment"]));
                Logger.Info("Number Of Visits={0}", RequestHelper.NullString(Request["NumberOfVisits"]));
                Logger.Info("Number Of Weeks={0}", RequestHelper.NullString(Request["NumberOfWeeks"]));
                Logger.Info("Primary Diagnosis Code={0}", RequestHelper.NullString(Request["PrimaryDiagnosisCode"]));
                Logger.Info("Secondary Diagnosis Code={0}", RequestHelper.NullString(Request["SecondaryDiagnosisCode"]));
                Logger.Info("UpperExtremity={0}", RequestHelper.NullString(Request["UpperExtremity"]));
                Logger.Info("LowerExtremity={0}", RequestHelper.NullString(Request["LowerExtremity"]));
                Logger.Info("LsSpine={0}", RequestHelper.NullString(Request["LsSpine"]));
                Logger.Info("CtSpine={0}", RequestHelper.NullString(Request["CtSpine"]));
                Logger.Info("HandWrist={0}", RequestHelper.NullString(Request["HandWrist"]));
                Logger.Info("OtherRegion={0}", RequestHelper.NullString(Request["OtherRegion"]));
                Logger.Info("Psfs Score={0}", RequestHelper.NullString(Request["PsfsScore"]));
                Logger.Info("Pain Rating={0}", RequestHelper.NullString(Request["PainRating"]));
                Logger.Info("Has Pain History={0}", RequestHelper.NullString(Request["HasPainHistory"]));
                Logger.Info("Is Smoker={0}", RequestHelper.NullString(Request["IsSmoker"]));
                Logger.Info("Is Drinker={0}", RequestHelper.NullString(Request["IsDrinker"]));
                Logger.Info("Take Opioids={0}", RequestHelper.NullString(Request["TakeOpioids"]));
                Logger.Info("Is Overweight={0}", RequestHelper.NullString(Request["IsOverweight"]));
                Logger.Info("Has Exercise={0}", RequestHelper.NullString(Request["HasExercise"]));
                Logger.Info("Has Confidence={0}", RequestHelper.NullString(Request["HasConfidence"]));
                Logger.Info("Has Depression={0}", RequestHelper.NullString(Request["HasDepression"]));
                Logger.Info("Limit Communication={0}", RequestHelper.NullString(Request["LimitCommunication"]));
                Logger.Info("Has Diabetes={0}", RequestHelper.NullString(Request["HasDiabetes"]));
                Logger.Info("Has Cns={0}", RequestHelper.NullString(Request["HasCns"]));
                Logger.Info("Has Cardiovascular={0}", RequestHelper.NullString(Request["HasCardiovascular"]));
                Logger.Info("Has Cancer={0}", RequestHelper.NullString(Request["HasCancer"]));
                Logger.Info("Has LungDisease={0}", RequestHelper.NullString(Request["HasLungDisease"]));
                Logger.Info("Max Visits={0}", RequestHelper.NullString(Request["MaxVisits"]));
                Logger.Info("Actual Visits={0}", RequestHelper.NullString(Request["ActualVisits"]));
                Logger.Info("Actual Visit Start Date={0}", RequestHelper.NullString(Request["ActualVisitStartDate"]));
                Logger.Info("Actual Visit End Date={0}", RequestHelper.NullString(Request["ActualVisitEndDate"]));
                Logger.Info("Visit Year Type={0}", RequestHelper.NullString(Request["VisitYearType"]));
                Logger.Info("Real Auth Type={0}", RequestHelper.NullString(Request["RealAuthTypeId"]));
                Logger.Info("Service Type={0}", RequestHelper.NullString(Request["ServiceType"]));
                Logger.Info("Accept Auth Reduction={0}", RequestHelper.NullString(Request["AcceptAuthReduction"]));
                Logger.Info("Send To Review={0}", RequestHelper.NullString(Request["SendToReview"]));
                Logger.Info("Application Id={0}", RequestHelper.NullString(Request["ApplicationId"]));
            }

            ptotRequest.IvrCode = RequestHelper.NullString(Request["AccessCode"]);
            ptotRequest.SubscriberId = RequestHelper.NullString(Request["PrefixSubscriberId"]);
            ptotRequest.MemberSeq = RequestHelper.NullString(Request["MemberSeq"]);
            ptotRequest.MemberId = ptotRequest.SubscriberId + ptotRequest.MemberSeq;
            ptotRequest.HasReferral = RequestHelper.NullBool(Request["HasReferral"]);
            ptotRequest.StartDate = RequestHelper.NullDateTime(Request["StartDate"]);
            ptotRequest.RequestType = RequestHelper.NullString(Request["RequestType"]);
            ptotRequest.Condition = RequestHelper.NullString(Request["Condition"]);
            ptotRequest.WorkRelated = RequestHelper.NullBool(Request["WorkRelated"]);
            ptotRequest.AutoAccident = RequestHelper.NullBool(Request["AutoAccident"]);
            ptotRequest.OtherInjury = RequestHelper.NullBool(Request["OtherInjury"]);
            ptotRequest.PostSurgery = RequestHelper.NullBool(Request["PostSurgery"]);
            ptotRequest.NoInjury = RequestHelper.NullBool(Request["NoInjury"]);
            ptotRequest.Duration = RequestHelper.NullString(Request["Duration"]);
            ptotRequest.InitialVisitDate = RequestHelper.NullDateTime(Request["InitialVisitDate"]);
            ptotRequest.PriorTreatment = RequestHelper.NullString(Request["PriorTreatment"]);
            ptotRequest.NumberOfVisits = RequestHelper.NullInt(Request["NumberOfVisits"]);
            ptotRequest.NumberOfWeeks = RequestHelper.NullInt(Request["NumberOfWeeks"]);
            ptotRequest.PrimaryDiagnosisCode = RequestHelper.NullString(Request["PrimaryDiagnosisCode"]);
            ptotRequest.SecondaryDiagnosisCode = RequestHelper.NullString(Request["SecondaryDiagnosisCode"]);
            ptotRequest.UpperExtremity = RequestHelper.NullBool(Request["UpperExtremity"]);
            ptotRequest.LowerExtremity = RequestHelper.NullBool(Request["LowerExtremity"]);
            ptotRequest.LsSpine = RequestHelper.NullBool(Request["LsSpine"]);
            ptotRequest.CtSpine = RequestHelper.NullBool(Request["CtSpine"]);
            ptotRequest.HandWrist = RequestHelper.NullBool(Request["HandWrist"]);
            ptotRequest.OtherRegion = RequestHelper.NullBool(Request["OtherRegion"]);
            ptotRequest.PsfsScore = RequestHelper.NullDecimal(Request["PsfsScore"]);
            ptotRequest.PainRating = RequestHelper.NullInt(Request["PainRating"]);
            ptotRequest.HasPainHistory = RequestHelper.NullBool(Request["HasPainHistory"]);
            ptotRequest.IsSmoker = RequestHelper.NullBool(Request["IsSmoker"]);
            ptotRequest.IsDrinker = RequestHelper.NullBool(Request["IsDrinker"]);
            ptotRequest.TakeOpioids = RequestHelper.NullBool(Request["TakeOpioids"]);
            ptotRequest.IsOverweight = RequestHelper.NullBool(Request["IsOverweight"]);
            ptotRequest.HasExercise = RequestHelper.NullBool(Request["HasExercise"]);
            ptotRequest.HasConfidence = RequestHelper.NullBool(Request["HasConfidence"]);
            ptotRequest.HasDepression = RequestHelper.NullBool(Request["HasDepression"]);
            ptotRequest.LimitCommunication = RequestHelper.NullBool(Request["LimitCommunication"]);
            ptotRequest.HasDiabetes = RequestHelper.NullBool(Request["HasDiabetes"]);
            ptotRequest.HasCns = RequestHelper.NullBool(Request["HasCns"]);
            ptotRequest.HasCardiovascular = RequestHelper.NullBool(Request["HasCardiovascular"]);
            ptotRequest.HasCancer = RequestHelper.NullBool(Request["HasCancer"]);
            ptotRequest.HasLungDisease = RequestHelper.NullBool(Request["HasLungDisease"]);
            ptotRequest.MaxVisits = RequestHelper.NullInt(Request["MaxVisits"]);
            ptotRequest.ActualVisits = RequestHelper.NullInt(Request["ActualVisits"]);
            ptotRequest.ActualVisitStartDate = RequestHelper.NullDateTime(Request["ActualVisitStartDate"]);
            ptotRequest.ActualVisitEndDate = RequestHelper.NullDateTime(Request["ActualVisitEndDate"]);
            ptotRequest.VisitYearType = RequestHelper.NullString(Request["VisitYearType"]);
            ptotRequest.RealAuthTypeId = RequestHelper.NullString(Request["RealAuthTypeId"]);
            ptotRequest.ServiceType = RequestHelper.NullString(Request["ServiceType"]);
            ptotRequest.AcceptAuthReduction = RequestHelper.NullBool(Request["AcceptAuthReduction"]);
            ptotRequest.SendToReview = RequestHelper.NullBool(Request["SendToReview"]);
            ptotRequest.ApplicationId = RequestHelper.NullInt(Request["ApplicationId"]);
            ptotRequest.ReferralDate = RequestHelper.NullDateTime(Request["ReferralDate"]);


            // Call GetPtotPreAuth() method
            var result = client.GetPtotPreAuth(ptotRequest);

            if (!result.IsValid)
            {
                returnCode = "199";
                errorMessage = result.ErrorMessage;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("AuthNumber", RequestHelper.NullString(result.AuthNumber), ref sb);
                AngelXmlHelper.GenerateVariable("VisitsApproved", RequestHelper.NullString(result.VisitsApproved), ref sb);
                AngelXmlHelper.GenerateVariable("DaysApproved", RequestHelper.NullString(result.DaysApproved), ref sb);
                AngelXmlHelper.GenerateVariable("DateApprovedFrom", RequestHelper.GetSqlDateFormat(result.DateApprovedFrom), ref sb);
                AngelXmlHelper.GenerateVariable("DateApprovedThru", RequestHelper.GetSqlDateFormat(result.DateApprovedThru), ref sb);
                AngelXmlHelper.GenerateVariable("DateApproved", RequestHelper.GetSqlDateFormat(result.DateApproved), ref sb);
                AngelXmlHelper.GenerateVariable("IsApproveReduction", RequestHelper.NullString(result.IsApproveReduction), ref sb);
                AngelXmlHelper.GenerateVariable("DecisionText", RequestHelper.NullString(result.DecisionText), ref sb);
                AngelXmlHelper.GenerateVariable("AuthCode", RequestHelper.NullString(result.AuthCode), ref sb);
                AngelXmlHelper.GenerateVariable("AuthStatus", RequestHelper.NullString(result.AuthStatus), ref sb);
                AngelXmlHelper.GenerateVariable("ClientAuthNumber", RequestHelper.NullString(result.ClientAuthNumber), ref sb);
                AngelXmlHelper.GenerateVariable("FailToPreAuthDays", RequestHelper.NullString(result.FailToPreAuthDays), ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();                        
        }

        private string CheckDiagCode()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;

            var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
            if (enableLogging)
            {
                var logTime = DateTime.Now;
                Logger.Error("*************************************   CheckDiagCode is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                Logger.Info("The original submitted data for the above CheckDiagCode are listed below:");
                Logger.Info("Diag Code={0}", RequestHelper.NullString(Request["DiagCode"]));
            }

            var diagCode = RequestHelper.NullString(Request["DiagCode"]);

            var result = client.CheckDiagCode(diagCode, false);

            if (!result.IsValid)
            {
                returnCode = "199";
                errorMessage = result.ErrorMessage;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
                AngelXmlHelper.GenerateVariable("FullDiagCode", RequestHelper.NullString(result.DiagCode), ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();                        
        }

        private string UpdateProviderFax()
        {
            var sb = new StringBuilder();
            var client = new PreAuthManagerClient();
            var returnCode = "100";
            var errorMessage = string.Empty;

            try
            {
                var enableLogging = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
                if (enableLogging)
                {
                    var logTime = DateTime.Now;
                    Logger.Error("*************************************   UpdateProviderFax is called at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
                    Logger.Info("The original submitted data for the above UpdateProviderFax are listed below:");
                    Logger.Info("Provider ID={0}", RequestHelper.NullString(Request["ProviderId"]));
                    Logger.Info("Location Seq={0}", RequestHelper.NullString(Request["LocationSeq"]));
                    Logger.Info("Fax={0}", RequestHelper.NullString(Request["Fax"]));
                }

                var providerId = RequestHelper.NullString(Request["ProviderId"]);
                var locationSeq = RequestHelper.NullString(Request["LocationSeq"]);
                var fax = RequestHelper.NullString(Request["Fax"]);
                client.UpdateProviderFax(providerId, locationSeq, fax);
            }
            catch (Exception ex)
            {
                returnCode = "199";
                errorMessage = ex.Message;
            }

            AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
            if (returnCode == "100")
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            else
            {
                AngelXmlHelper.GenerateVariable("ReturnCode", returnCode, ref sb);
                AngelXmlHelper.GenerateVariable("ErrorMessage", errorMessage, ref sb);
            }
            AngelXmlHelper.GenerateFooter(ref sb);

            return sb.ToString();            
        }
    }
}