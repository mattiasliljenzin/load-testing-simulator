using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Statistics;

namespace RequestSimulation.Storage
{
    public class SqlSimulationResultStorage : ISimulationResultStorage
    {
        private readonly IConfiguration _configuration;

        public SqlSimulationResultStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Save(ICollection<RequestRecording> requests)
        {
            Console.WriteLine($"[SqlSimulationResultStorage]: Saving result ({requests.Count()} items)");
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:SQL"]))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    await connection.InsertAsync(requests, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}