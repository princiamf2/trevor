using Microsoft.AspNetCore.Mvc;
using TrevorDrepaBot.Conversation;

namespace TrevorDrepaBot.Controllers
{
    public class TestRequest
    {
        public string? Text { get; set; }
        public string? SessionId { get; set; }
    }

    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly DrepaConversationEngine _engine;

        public TestController(DrepaConversationEngine engine)
        {
            _engine = engine;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TestRequest request, CancellationToken cancellationToken)
        {
            var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
                ? "default"
                : request.SessionId.Trim();

            var reply = await _engine.GetReplyAsync(
                request.Text ?? "",
                sessionId,
                cancellationToken);

            return Ok(new { reply });
        }
    }
}