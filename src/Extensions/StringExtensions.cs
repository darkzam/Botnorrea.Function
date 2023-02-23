namespace Botnorrea.Functions.Extensions
{
    public static class StringExtensions
    {
        public static string UppercaseFirstChar(this string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
