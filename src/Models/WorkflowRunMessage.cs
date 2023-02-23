using Botnorrea.Functions.Extensions;

namespace Botnorrea.Functions.Models
{
    public class WorkflowRunMessage : Message
    {
        public string Status { get; set; }
        public string DebtManagerUrl { get; set; }

        public WorkflowRunMessage(string action, string url, string username, string title, string status, string debtManagerUrl) : base(action, url, username, title)
        {
            Status = status;
            DebtManagerUrl = debtManagerUrl;
        }

        public override string Initialize()
        {
            return $"Workflow {Title} started by {Username.UppercaseFirstChar()} has been {Action} with {Status.UppercaseFirstChar()} status: {DebtManagerUrl}";
        }
    }
}
