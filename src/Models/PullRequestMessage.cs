using Botnorrea.Functions.Extensions;

namespace Botnorrea.Functions.Models
{
    public class PullRequestMessage : Message
    {
        public string Merged { get; set; }

        public PullRequestMessage(string action,
                                  string url,
                                  string username,
                                  string title,
                                  string merged) : base(action, url, username, title)
        {
            Merged = merged;
        }

        public override string Initialize()
        {
            return $"{Username.UppercaseFirstChar()} has {Action} a pull request: {Url}";
        }
    }
}
