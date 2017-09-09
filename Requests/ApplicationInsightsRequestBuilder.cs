using System;
using Newtonsoft.Json.Linq;

namespace RequestSimulation.Requests
{
    public static class ApplicationInsightsRequestBuilder
    {
        public static ApplicationInsightsRequest Create(JArray data)
        {
            var request = new ApplicationInsightsRequest();
            request.timestamp = data[0].Value<DateTime>();
            request.id = data[1].Value<string>();
            request.source = data[2].Value<string>();
            request.name = data[3].Value<string>();
            request.url = data[4].Value<string>();
            request.success = data[5].Value<string>();
            request.resultCode = data[6].Value<string>();
            request.duration = data[7].Value<int>();
            request.performanceBucket = data[8].Value<string>();
            request.customDimensions = data[9].Value<dynamic>();
            request.customMeasurements = data[10].Value<string>();
            request.operation_Name = data[11].Value<string>();
            request.operation_Id = data[12].Value<string>();
            request.operation_ParentId = data[13].Value<string>();
            request.operation_SyntheticSource = data[14].Value<string>();
            request.session_Id = data[15].Value<string>();
            request.user_Id = data[16].Value<string>();
            request.user_AuthenticatedId = data[17].Value<string>();
            request.user_AccountId = data[18].Value<string>();
            request.application_Version = data[19].Value<string>();
            request.client_Type = data[20].Value<string>();
            request.client_Model = data[21].Value<string>();
            request.client_OS = data[22].Value<string>();
            request.client_IP = data[23].Value<string>();
            request.client_City = data[24].Value<string>();
            request.client_StateOrProvince = data[25].Value<string>();
            request.client_CountryOrRegion = data[26].Value<string>();
            request.client_Browser = data[27].Value<string>();
            request.cloud_RoleName = data[28].Value<string>();
            request.cloud_RoleInstance = data[29].Value<string>();
            request.appId = data[30].Value<string>();
            request.appName = data[31].Value<string>();
            request.iKey = data[32].Value<string>();
            request.sdkVersion = data[33].Value<string>();
            request.itemId = data[34].Value<string>();
            request.itemType = data[35].Value<string>();
            request.itemCount = data[36].Value<int>();
            return request;
        }
    }
}


