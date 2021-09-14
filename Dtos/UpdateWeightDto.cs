using System;
using System.ComponentModel.DataAnnotations;
using MeanMotivator.Models;

namespace MeanMotivator.Dtos
{
    public record UpdateWeightDto
    {
        public Double value { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}