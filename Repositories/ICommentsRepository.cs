using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeanMotivator.Models;

namespace MeanMotivator.Repositories
{
        public interface ICommentsRepository
    {
        Task<Comment> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task CreateCommentAsync(Comment Comment);
        Task UpdateCommentAsync(Comment Comment);
        Task DeleteCommentAsync(Guid id);
        Task<Comment> GetRandomCommentAsync();
    }
}