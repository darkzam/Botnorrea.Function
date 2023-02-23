namespace Botnorrea.Functions.Models
{
    public abstract class Message
    {
        protected string Action { get; set; }
        protected string Url { get; set; }
        protected string Username { get; set; }
        protected string Title { get; set; }

        protected Message(string action, string url, string username, string title)
        {
            Action = action;
            Url = url;
            Username = username;
            Title = title;
        }

        public abstract string Initialize();
    }
}
