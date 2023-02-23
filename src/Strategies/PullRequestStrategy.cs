using Botnorrea.Functions.Models;

namespace Botnorrea.Functions.Strategies
{
    public class PullRequestGetMessageStrategy : GetMessageStrategy
    {
        public override string GetMessage(dynamic payload)
        {
            var message = new PullRequestMessage(payload?.action.ToString(),
                                                 payload?.pull_request?.html_url.ToString(),
                                                 payload?.pull_request?.user?.login.ToString(),
                                                 payload?.pull_request?.title.ToString(),
                                                 payload?.pull_request?.merged.ToString());

            return message.Initialize();
        }
    }
}
