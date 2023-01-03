using System.Text;

namespace CustomModelsBuilder.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendWithIndentation(this StringBuilder sb, string text, int level)
        {
            sb.Append($"{string.Concat(Enumerable.Repeat("\t", level))}{text}\n");
        }
    }
}