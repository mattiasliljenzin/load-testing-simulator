using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using RequestSimulation.Statistics;
using RequestSimulation.Storage;
using simulation.core;

namespace simulation.packages.Storage.ApplicationInsights
{
    public class ApplicationInsightsResultStorage : ISimulationResultStorage
    {
        private readonly IConfiguration _configuration;

        public ApplicationInsightsResultStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Save(IEnumerable<RequestRecording> requests)
        {
            try
            {
                var blob = await GetBlobFromStorageAsync();
                var csv = JsonToCsv(requests, ",");

                await blob.UploadTextAsync(csv);
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