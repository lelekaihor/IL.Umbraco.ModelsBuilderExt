using System.Text;
using CustomModelsBuilder.Extensions;
using Umbraco.Cms.Infrastructure.ModelsBuilder.Building;

namespace CustomModelsBuilder.FieldNamesWriter
{
    public class FieldNamesWriter
    {
        private const char NewLine = '\n';
        private const string OpenBracket = "{";
        private const string ClosingBracket = "}";

        public void AddFieldNamesStruct(StringBuilder sb, TypeModel type, int level = 1)
        {
            sb.Append(NewLine);
            sb.AppendWithIndentation($"public static struct {type.ClrName}FieldNames", level);
            sb.AppendWithIndentation(OpenBracket, level);
            WriteContentTypeProperties(sb, type, level + 1);
            sb.AppendWithIndentation(ClosingBracket, level);
        }

        private void WriteContentTypeProperties(StringBuilder sb, TypeModel type, int level)
        {
            var commonListOfProperties = new List<PropertyModel>();
            // properties
            commonListOfProperties.AddRange(type.Properties);
            // mixins properties
            commonListOfProperties.AddRange(type.ImplementingInterfaces.SelectMany(x => x.Properties));

            // write the properties
            foreach (var prop in commonListOfProperties.DistinctBy(x => x.ClrName).OrderBy(x => x.ClrName))
            {
                WriteProperty(sb, prop, level);
            }
        }

        private void WriteProperty(StringBuilder sb, PropertyModel property, int level)
        {
            sb.AppendWithIndentation($"public const string {property.ClrName} = \"{property.Alias}\";", level);
        }
    }
}
