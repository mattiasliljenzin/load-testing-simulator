using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using RequestSimulation.Statistics;

namespace RequestSimulation.Storage.ApplicationInsights
{
    public class ApplicationInsightsResultStorage : ISimulationResultStorage
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationInsightsIngestion _ingestion;

        public ApplicationInsightsResultStorage(IConfiguration configuration, IApplicationInsightsIngestion ingestion)
        {
            _configuration = configuration;
            _ingestion = ingestion;
        }

        public async Task Save(IEnumerable<RequestRecording> requests)
        {
            var data = requests.ToList();

            Console.WriteLine($"[ApplicationInsightsResultStorage]: Saving result ({data.Count()} items)");

            try
            {
                var blob = await GetBlobFromStorageAsync();
                var csv = JsonToCsv(data, ",");

                await blob.UploadTextAsync(csv);
                await _ingestion.Trigger(); // dont run parallel, sequency is necessary
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something bad happened when trying to upload results to blog storage. Exception was: {ex.ToString()}");
                throw;
            }
        }

        private async Task<CloudBlockBlob> GetBlobFromStorageAsync()
        {
            var container = _configuration["Packages:ApplicationInsightsResultStorage:ContainerName"];
            var blob = _configuration["Packages:ApplicationInsightsResultStorage:BlobName"];
            var storage = CloudStorageAccount.Parse(_configuration["Packages:ApplicationInsightsResultStorage:StorageConnectionString"]);

            var client = storage.CreateCloudBlobClient();
            var containerReference = client.GetContainerReference(container);

            await containerReference.CreateIfNotExistsAsync();

            return containerReference.GetBlockBlobReference(blob);
        }

        private static string JsonToCsv(IEnumerable<RequestRecording> requests, string delimiter)
        {
            var csvString = new StringWriter();
            using (var csv = new CsvWriter(csvString))
            {
                csv.Configuration.SkipEmptyRecords = true;
                csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.Delimiter = delimiter;

                csv.WriteRecords(requests);
            }
            return csvString.ToString();
        }
    }
}