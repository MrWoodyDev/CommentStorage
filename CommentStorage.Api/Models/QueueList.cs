using CommentStorage.Api.Controllers.Domain.Comments.Requests;

namespace CommentStorage.Api.Models;

public static class QueueList
{
    public static List<InternalCreateCommentRequest> InternalCreateCommentRequests { get; set; } = new List<InternalCreateCommentRequest>();

    public static void Add(InternalCreateCommentRequest request)
    {
        InternalCreateCommentRequests.Add(request);
    }

    public static void ClearOutDated()
    {
        for (int i = InternalCreateCommentRequests.Count - 1; i >= 0; i--)
        {
            var timePassed = DateTime.Now - InternalCreateCommentRequests[i].CreatedDate;
            var totalSeconds = timePassed.TotalSeconds;

            if (totalSeconds >= 120)
            {
                InternalCreateCommentRequests.RemoveAt(i);
            }
        }
    }
}