namespace Bots.Core.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string str)
    {
        string[] tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            tokens[i] = string.Concat(token[..1].ToUpper(), token.AsSpan(1));
        }

        return string.Join(" ", tokens);
    }
}
