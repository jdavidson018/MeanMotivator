using System;
using MeanMotivator.Models;

namespace MeanMotivator.Dtos
{
    public record CommentDto
    {
        public Guid Id { get; set; }
        public CommentTypeEnum Type { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}