using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RequestSimulation.Requests
{
    public static class ApplicationInsightsDependencyBuilder
    {
        public static ApplicationInsightsDependency Create(JArray data)
        {

            var request = new ApplicationInsightsDependency();

            request.timestamp = data[0].Value<DateTime>();
            request.id = data[1].Value<string>();
            request.target = data[2].Value<string>();
            request.type = data[3].Value<string>();
            request.name = data[4].Value<string>();
            request.data = data[5].Value<string>();
            request.success = data[6].Value<string>();
            request.resultCode = data[7].Value<string>();
            request.duration = data[8].Value<double>();
            request.performanceBucket = data[9].Value<string>();
            request.customDimensions = data[10].Value<dynamic>();
            request.customMeasurements = data[11].Value<string>();
            request.operation_Name = data[12].Value<string>();
            request.operation_Id = data[13].Value<string>();
            request.operation_ParentId = data[14].Value<string>();
            request.operation_SyntheticSource = data[15].Value<string>();
            request.session_Id = data[16].Value<string>();
            request.user_Id = data[17].Value<string>();
            request.user_AuthenticatedId = data[18].Value<string>();
            request.user_AccountId = data[19].Value<string>();
            request.application_Version = data[20].Value<string>();
            request.client_Type = data[21].Value<string>();
            request.client_Model = data[22].Value<string>();
            request.client_OS = data[23].Value<string>();
            request.client_IP = data[24].Value<string>();
            request.client_City = data[25].Value<string>();
            request.client_StateOrProvince = data[26].Value<string>();
            request.client_CountryOrRegion = data[27].Value<string>();
            request.client_Browser = data[28].Value<string>();
            request.cloud_RoleName = data[29].Value<string>();
            request.cloud_RoleInstance = data[30].Value<string>();
            request.appId = data[31].Value<string>();
            request.appName = data[32].Value<string>();
            request.iKey = data[33].Value<string>();
            request.sdkVersion = data[34].Value<string>();
            request.itemId = data[35].Value<string>();
            request.itemType = data[36].Value<string>();
            request.itemCount = data[37].Value<int>();

            request.data = request.data.ToLower().Replace("get", string.Empty).Trim();

            if (!request.data.StartsWith("http"))
            {
                request.data = $"http://{request.target}{request.data}";
            }

            return request;
        }
    }
}