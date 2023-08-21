namespace CommentStorage.Api.Models;

public class Comment
{
    public long Id { get; set; }

    public string Text { get; set; }

    public DateTime CreatedDate { get; set; }
}
