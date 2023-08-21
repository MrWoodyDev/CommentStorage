using CommentStorage.Console;


var baseUrl = "https://localhost:7045";
var httpClient = new HttpClient();
var client = new Client(baseUrl, httpClient);

var requestIds = new List<string>();
for (int i = 0; i < 10; i++)
{
    var comment = new CreateCommentRequest
    {
        Comment = "Comment" + i
    };

    var response = await client.CreateCommentAsync(comment);
    var requestId = response.Id;
    requestIds.Add(requestId);
    Console.WriteLine($"Created Comment with RequestId: {requestId}");
    await Task.Delay(1000);
}

while (requestIds.Count > 0)
{
    for (int i = 0; i < requestIds.Count; i++)
    {
        string currentRequestId = requestIds[i];

        try
        {
            var commentStatusAsync = await client.GetCommentStatusAsync(currentRequestId);

            if (commentStatusAsync.Id != 0)
            {
                Console.WriteLine($"CommentId for RequestId {currentRequestId}: {commentStatusAsync.Id}, {commentStatusAsync.Text}, {commentStatusAsync.CreatedDate}");
                requestIds.RemoveAt(i);
            }
            else
            {
                Console.WriteLine($"Waiting for Comment with RequestId {currentRequestId} to be written...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while checking Comment with RequestId {currentRequestId}: {ex.Message}");
            await Task.Delay(1000);
        }
    }
}

Console.WriteLine("All comments have been written. Exiting...");
