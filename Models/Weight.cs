using System;

namespace MeanMotivator.Models
{
    public record Weight
    {
        public Guid Id { get; set; }
        public Double Value { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}