namespace CommentStorage.Api.Controllers.Domain.Comments.Requests;

public class CreateRequestId
{
    public CreateRequestId(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}