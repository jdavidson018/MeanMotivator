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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsRepository repository;
        private readonly ILogger<CommentsController> logger;
        public CommentsController(ICommentsRepository repository, ILogger<CommentsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<CommentDto>> GetCommentsAsync()
        {
            var comments = (await repository.GetCommentsAsync())
                        .Select(comment => comment.AsDto());
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: retrieved {comments.Count()} comments");
            return comments;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetCommentAsync(Guid id)
        {
            var comment = await repository.GetCommentAsync(id);

            if(comment is null)
            {
                return NotFound();
            }
            return comment.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateCommentAsync(CreateCommentDto commentDto)
        {
            Comment comment = new()
            {
                Id = Guid.NewGuid(),
                Type = commentDto.Type,
                Text = commentDto.Text,
                Author = commentDto.Author,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateCommentAsync(comment);
            return CreatedAtAction(nameof(GetCommentAsync), new {id = comment.Id}, comment.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCommentAsync(Guid id, UpdateCommentDto commentDto)
        {
            var existingComment = await repository.GetCommentAsync(id);

            if(existingComment is null){
                return NotFound();
            }

            Comment updatedComment = existingComment with
            {
                Type = commentDto.Type,
                Text = commentDto.Text,
                Author = commentDto.Author
            };

            await this.repository.UpdateCommentAsync(updatedComment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCommentAsync(Guid id)
        {
            var existingComment = await repository.GetCommentAsync(id);

            if(existingComment is null){
                return NotFound();
            }

            await repository.DeleteCommentAsync(id);
            return NoContent();
        }
    }
}