using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace simulation
{
    public interface IRequestSourceService
    {
        Task<IDictionary<DateTime, IList<ISimulatedRequest>>> GetAsync(DateTime from, DateTime to);
    }
}
