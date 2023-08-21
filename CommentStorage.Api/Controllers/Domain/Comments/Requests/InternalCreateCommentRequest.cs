namespace CommentStorage.Api.Controllers.Domain.Comments.Requests;

public class InternalCreateCommentRequest
{
    public long Id { get; set; }

    public string RequestId { get; set; }

    public string Text { get; set; }

    public DateTime CreatedDate { get; set; }
}