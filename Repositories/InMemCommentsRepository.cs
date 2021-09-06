using System;
using System.Collections.Generic;
using MeanMotivator.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MeanMotivator.Repositories{
    public class InMemCommentsRepository : ICommentsRepository
    {
        private readonly List<Comment> comments = new()
        {
            new Comment { Id = Guid.NewGuid(), Type=CommentTypeEnum.INSULT,Text = "You are a fat sack of shit.", Author = "Jar Badson", CreatedDate = DateTimeOffset.UtcNow },
            new Comment {Id = Guid.NewGuid(), Type=CommentTypeEnum.COMPLIMENT, Text="Damn, look at you! Keep this up and you are going to be one polished turd!", Author="Jar Badson", CreatedDate = DateTimeOffset.UtcNow},
            new Comment {Id = Guid.NewGuid(), Type=CommentTypeEnum.MOTIVATIONAL_QUOTE, Text="Nature has given us all the pieces required to achieve exceptional wellness and health, but has left it to us to put these pieces together.", Author="Diane McLaren", CreatedDate= DateTimeOffset.UtcNow}
        };

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            return await Task.FromResult(comments);
        }

        public async Task<Comment> GetCommentAsync(Guid id)
        {
            var comment = comments.SingleOrDefault(x => x.Id == id);
            return await Task.FromResult(comment);
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            comments.Add(comment);
            await Task.CompletedTask;
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            var index = comments.FindIndex(x => x.Id == comment.Id);
            comments[index] = comment;
            await Task.CompletedTask;
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var index = comments.FindIndex(x => x.Id == id);
            comments.RemoveAt(index);
            await Task.CompletedTask;
        }
    }
}