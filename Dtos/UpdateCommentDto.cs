using System;
using System.ComponentModel.DataAnnotations;
using MeanMotivator.Models;

namespace MeanMotivator.Dtos
{
    public record UpdateCommentDto
    {
        [Required]
        public CommentTypeEnum Type { get; set; }
        [Required]
        public string Text { get; set; }
        public string Author { get; set; }
    }
}