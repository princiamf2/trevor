using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using TrevorDrepaBot.Conversation;

namespace TrevorDrepaBot.Bots
{
    public class DrepaBot : ActivityHandler, IBot
    {
        private readonly DrepaConversationEngine _engine;

        public DrepaBot(DrepaConversationEngine engine)
        {
            _engine = engine;
        }

        protected override async Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var sessionId = turnContext.Activity.Conversation?.Id
                            ?? turnContext.Activity.From?.Id
                            ?? "default";

            var replyText = await _engine.GetReplyAsync(
                turnContext.Activity.Text ?? "",
                sessionId,
                cancellationToken);

            await turnContext.SendActivityAsync(
                MessageFactory.Text(replyText),
                cancellationToken);
        }
    }
}