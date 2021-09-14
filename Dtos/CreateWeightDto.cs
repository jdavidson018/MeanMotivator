using System;
using System.ComponentModel.DataAnnotations;
using MeanMotivator.Models;

namespace MeanMotivator.Dtos
{
    public record CreateWeightDto
    {
        [Required]
        public Double value { get; set; }
        [Required]
        public DateTimeOffset CreatedDate { get; init; }
    }
}