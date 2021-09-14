using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeanMotivator.Models;

namespace MeanMotivator.Repositories
{
        public interface IWeightsRepository
    {
        Task<Weight> GetWeightAsync(Guid id);
        Task<IEnumerable<Weight>> GetWeightsAsync();
        Task CreateWeightAsync(Weight weight);
        Task UpdateWeightAsync(Weight weight);
        Task DeleteWeightAsync(Guid id);
    }
}