using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeanMotivator.Dtos;
using MeanMotivator.Models;
using MeanMotivator.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MeanMotivator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeightsController : ControllerBase
    {
        private readonly IWeightsRepository repository;
        private readonly ILogger<WeightsController> logger;
        public WeightsController(IWeightsRepository repository, ILogger<WeightsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeightDto>> GetWeightsAsync()
        {
            var weights = (await repository.GetWeightsAsync())
                        .Select(weight => weight.AsDto());
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: retrieved {weights.Count()} weights");
            return weights;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeightDto>> GetWeightAsync(Guid id)
        {
            var weight = await repository.GetWeightAsync(id);

            if(weight is null)
            {
                return NotFound();
            }
            return weight.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<WeightDto>> CreateWeightAsync(CreateWeightDto weightDto)
        {
            Weight weight = new()
            {
                Id = Guid.NewGuid(),
                Value = weightDto.value,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateWeightAsync(weight);
            return CreatedAtAction(nameof(GetWeightAsync), new {id = weight.Id}, weight.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWeightAsync(Guid id, UpdateWeightDto weightDto)
        {
            var existingWeight = await repository.GetWeightAsync(id);

            if(existingWeight is null){
                return NotFound();
            }

            Weight updatedWeight = existingWeight with
            {
                Value = weightDto.value,
                CreatedDate = weightDto.CreatedDate,
            };

            await this.repository.UpdateWeightAsync(updatedWeight);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWeightAsync(Guid id)
        {
            var existingWeight = await repository.GetWeightAsync(id);

            if(existingWeight is null){
                return NotFound();
            }

            await repository.DeleteWeightAsync(id);
            return NoContent();
        }
    }
}