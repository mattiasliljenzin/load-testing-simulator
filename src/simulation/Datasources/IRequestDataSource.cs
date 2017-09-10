using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestSimulation.Requests;

namespace RequestSimulation.Datasources
{
    public interface IRequestDataSource
    {
        Task<IDictionary<DateTime, IList<ISimulatedRequest>>> GetAsync(DateTime from, DateTime to);
    }
}
