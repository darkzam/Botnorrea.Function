using Botnorrea.Functions.Models;

namespace Botnorrea.Functions.Strategies
{
    public class WorkflowRunGetMessageStrategy : GetMessageStrategy
    {
        private readonly string DebtManagerUrl;
        public WorkflowRunGetMessageStrategy(string debtManagerUrl)
        {
            DebtManagerUrl = debtManagerUrl;
        }

        public override string GetMessage(dynamic payload)
        {
            var message = new WorkflowRunMessage(payload?.action.ToString(),
                                                 payload?.workflow_run?.html_url.ToString(),
                                                 payload?.workflow_run?.actor?.login.ToString(),
                                                 payload?.workflow_run?.name.ToString(),
                                                 payload?.workflow_run?.status.ToString(),
                                                 DebtManagerUrl);

            return message.Initialize();
        }
    }
}
