using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;
using RRS.BEL;

namespace RRS.Services
{
    [ServiceContract(Namespace = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/")]
    [XmlSerializerFormat]
    public interface IPreAuthManager
    {
        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/HelloWorld")]
        string HelloWorld();

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetChiroPreAuth")]
        ChiroAuthResponse GetChiroPreAuth(ChiroAuthRequest request);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetPtotPreAuth")]
        PtotAuthResponse GetPtotPreAuth(PtotAuthRequest request);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetRrsQuestions")]
        DataSet GetRrsQuestions(string questionSet);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/InsertRrsEntryLog")]
        bool InsertRrsEntryLog(string callStarted, string sessionId, string phoneNumber, string variableName,
                              string entryString, string validationResult, string isBcbsma);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/InsertAuthSsoLog")]
        bool InsertAuthSsoLog(int authNumber, AuthSsoData ssoData);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/UpdateProviderFax")]
        bool UpdateProviderFax(string providerId, string locationSeq, string fax);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckDiagCode")]
        DiagCodeResponse CheckDiagCode(string code, bool isIvr);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/InsertInvalidDiagCode")]
        bool InsertInvalidDiagCode(string code, string providerId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckMedicareDiagCode")]
        Response CheckMedicareDiagCode(string code, int codeType);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckProvider")]
        ProviderResponse CheckProvider(string ivrCode, bool isIvr, string userId, string password);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckProviderFromSso")]
        ProviderResponse CheckProviderFromSso(string clientPrefix, string id, DateTime requestedStartDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckProviderFromAngel")]
        AngelProviderResponse CheckProviderFromAngel(string ivrCode);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckMember")]
        MemberResponse CheckMember(string ivrCode, bool isIvr, string memberId, DateTime dateOfBirth, DateTime startDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckMemberFromWeb")]
        MemberResponse CheckMemberFromWeb(string ivrCode, string subscriberId, string memberSeq, string authTypeId, DateTime dateOfBirth, DateTime startDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckMemberFromSso")]
        MemberResponse CheckMemberFromSso(string clientPrefix, string subscriberId, string blindKey);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckMemberFromAngel")]
        DataSet CheckMemberFromAngel(string ivrCode, string subscriberId, DateTime startDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/PreCheckMemberInfo")]
        MemberResponse PreCheckMemberInfo(string ivrCode, string subscriberId, string memberSeq, DateTime startDate, DateTime dateOfBirth);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetAuthTypeFromSelectedServiceType")]
        MemberResponse GetAuthTypeFromSelectedServiceType(string prefixSubscriberId, string memberSeq, DateTime startDate, string serviceType);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckFriScore")]
        Response CheckFriScore(string friScore);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckPsfsScore")]
        Response CheckPsfsScore(string psfsScore);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetAuthResult")]
        AuthResultResponse GetAuthResult(string authNumber);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetProviderInfo")]
        ProviderResponse GetProviderInfo(string ivrCode);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetProviderFax")]
        ProviderResponse GetProviderFax(string providerId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/ValidateBcbsmaNumber")]
        Response ValidateBcbsmaNumber(string bcbsmaNumber, string providerId, string subscriberId, string memberSeq);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetDiagnosticCodes")]
        List<string> GetDiagnosticCodes(string keyWord);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetProviderAddressList")]
        DataSet GetProviderAddressList(string providerId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetProviderAddressListFromSso")]
        DataSet GetProviderAddressListFromSso(string providerId, DateTime requestedStartDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetAllMatchedPtotAuthType")]
        DataSet GetAllMatchedPtotAuthType(string ivrCode, string subscriberId, string memberSeq, DateTime requestedStartDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetAllMatchedPtotAuthTypeFromSso")]
        DataSet GetAllMatchedPtotAuthTypeFromSso(string ivrCode, string clientPrefix, string subscriberId, string blindKey, DateTime requestedStartDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetAllServiceType")]
        DataSet GetAllServiceType(string ivrCode, string prefixSubscriberId, string memberSeq, DateTime requestedStartDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetClaimsAddress")]
        MemberResponse GetClaimsAddress(string ivrCode, string prefixSubscriberId, string memberSeq);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/HasDuplicateClientAuthNumber")]
        bool HasDuplicateClientAuthNumber(string clientAuthNumber, string subscriberId, string providerId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetHighmarkMemberData")]
        HighmarkMemberResponse GetHighmarkMemberData(string subscriberId, DateTime inquiryDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetHighmarkMemberDataByAuth")]
        HighmarkMemberResponse GetHighmarkMemberDataByAuth(string authNumber, DateTime inquiryDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetBcbsmaProviderList")]
        DataSet GetBcbsmaProviderList();

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/ValidateBcbsmaSso")]
        BcbsmaSsoResponse ValidateBcbsmaSso(string bcbsmaProviderId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetBcbsmaProviderGroup")]
        DataSet GetBcbsmaProviderGroup(string providerId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetBcbsmaProviderInfo")]
        ProviderResponse GetBcbsmaProviderInfo(string providerId);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/CheckBcbsmaMember")]
        MemberResponse CheckBcbsmaMember(string providerId, string subscriberId, string memberSeq, DateTime dateOfBirth, DateTime startDate);

        [OperationContract(Action = "http://corpcamapp.amhc.amhealthways.net/RRSWebHost/GetBcbsmaProviderReminder")]
        string GetBcbsmaProviderReminder(string providerId, string subscriberId, string memberSeq, DateTime startDate);
    }
}
