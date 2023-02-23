namespace Botnorrea.Functions.Strategies
{
    public abstract class GetMessageStrategy
    {
        protected GetMessageStrategy()
        { }

        public abstract string GetMessage(dynamic payload);
    }
}
