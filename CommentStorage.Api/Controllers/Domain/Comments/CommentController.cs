using System.Net;
using CommentStorage.Api.Common;
using CommentStorage.Api.Controllers.Domain.Comments.Requests;
using CommentStorage.Api.Models;
using CommentStorage.Api.Persistence.CommentStorageDb;
using Microsoft.AspNetCore.Mvc;
using System.Timers;
using Microsoft.EntityFrameworkCore;

namespace CommentStorage.Api.Controllers.Domain.Comments;

[ApiController]
[Route(Routs.Comments)]
public class CommentController : ControllerBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static readonly System.Timers.Timer _timer;
    private readonly CommentStorageDbContext _dbContext;

    static CommentController()
    {
        _timer = new System.Timers.Timer();
        _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        _timer.Interval = 1000;
        _timer.Enabled = true;
    }

    public CommentController(IServiceScopeFactory serviceScopeFactory, CommentStorageDbContext dbContext)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateRequestId), 202)]
    public async Task<IActionResult> CreateComment(CreateCommentRequest request)
    {
        var requestId = Guid.NewGuid().ToString();
        var createDate = DateTime.Now;
        var internalRequest = new InternalCreateCommentRequest
        {
            RequestId = requestId,
            Text = request.Comment,
            CreatedDate = createDate
        };
        QueueList.Add(internalRequest);
        Task.Run(async () =>
        {
            await Task.Delay(12000);
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CommentStorageDbContext>();
            var comment = await context.Comments.AddAsync(new Comment
            {
                Text = internalRequest.Text,
                CreatedDate = internalRequest.CreatedDate
            });
            await context.SaveChangesAsync();
            internalRequest.Id = comment.Entity.Id;
        });
        return Accepted(new CreateRequestId(requestId));
    }

    [ProducesResponseType(typeof(Comment), 200)]
    [ProducesResponseType(404)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentStatus(string id)
    {
        var comment = QueueList.InternalCreateCommentRequests.FirstOrDefault(c=> c.RequestId == id);

        if (comment.Id == 0)
        {
            return NotFound();
        }
        if (comment is not null)
        {
            return Ok(new Comment
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate
            });
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<Comment[]> GetComment(CancellationToken cancellationToken)
    {
        return await _dbContext.Comments
            .OrderBy(x => x.Id)
            .Select(x => new Comment
            {
                Id = x.Id,
                Text = x.Text,
                CreatedDate = x.CreatedDate
            }).ToArrayAsync(cancellationToken);
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        QueueList.ClearOutDated();
    }
}