using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RequestSimulation.Storage.ApplicationInsights
{
    public class AnalyticsDataSourceClient
    {
        private readonly Uri endpoint = new Uri("https://dc.services.visualstudio.com/v2/track");
        private const string RequestContentType = "application/json; charset=UTF-8";
        private const string RequestAccess = "application/json";

        public async Task<bool> RequestBlobIngestion(AnalyticsDataSourceIngestionRequest ingestionRequest)
        {
            var request = WebRequest.CreateHttp(endpoint);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = RequestContentType;
            request.Accept = RequestAccess;

            string notificationJson = Serialize(ingestionRequest);
            byte[] notificationBytes = GetContentBytes(notificationJson);
            request.ContentLength = notificationBytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(notificationBytes, 0, notificationBytes.Length);
            requestStream.Close();

            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException e)
            {
                HttpWebResponse httpResponse = e.Response as HttpWebResponse;
                if (httpResponse != null)
                {
                    Console.WriteLine(
                        "Ingestion request failed with status code: {0}. Error: {1}",
                        httpResponse.StatusCode,
                        httpResponse.StatusDescription);
                }
                throw;
            }
        }
        private byte[] GetContentBytes(string content)
        {
            return Encoding.UTF8.GetBytes(content);
        }


        private string Serialize(AnalyticsDataSourceIngestionRequest ingestionRequest)
        {
            return JsonConvert.SerializeObject(ingestionRequest);
        }
    }
}