using Botnorrea.Functions.Models;

namespace Botnorrea.Functions.Strategies
{
    public class PullRequestGetMessageStrategy : GetMessageStrategy
    {
        public override string GetMessage(dynamic payload)
        {
            var message = new PullRequestMessage(payload?.action,
                                                 payload?.pull_request?.html_url,
                                                 payload?.pull_request?.user?.login,
                                                 payload?.pull_request?.title,
                                                 payload?.pull_request?.merged);

            return message.Initialize();
        }
    }
}
