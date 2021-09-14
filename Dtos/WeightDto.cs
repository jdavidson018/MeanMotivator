using System;
using MeanMotivator.Models;

namespace MeanMotivator.Dtos
{
    public record WeightDto
    {
        public Guid Id { get; set; }
        public Double Value { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}