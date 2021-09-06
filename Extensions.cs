using MeanMotivator.Dtos;
using MeanMotivator.Models;

namespace MeanMotivator
{
    public static class Extensions
    {
        public static CommentDto AsDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Type = comment.Type,
                Text = comment.Text,
                Author = comment.Author,
                CreatedDate = comment.CreatedDate
            };
        }
    }
}