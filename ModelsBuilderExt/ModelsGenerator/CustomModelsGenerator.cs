using Microsoft.Extensions.Options;
using System.Text;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Infrastructure.ModelsBuilder.Building;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Extensions;
using CustomModelsBuilder.FieldNamesWriter;

namespace CustomModelsBuilder.ModelsGenerator
{
    public class CustomModelsGenerator : global::Umbraco.Cms.Infrastructure.ModelsBuilder.Building.ModelsGenerator
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly FieldNamesWriter.FieldNamesWriter _fieldNamesWriter;
        private readonly OutOfDateModelsStatus _outOfDateModels;
        private readonly UmbracoServices _umbracoService;
        private ModelsBuilderSettings _config;

        public CustomModelsGenerator(UmbracoServices umbracoService, IOptionsMonitor<ModelsBuilderSettings> config, OutOfDateModelsStatus outOfDateModels, IHostingEnvironment hostingEnvironment, FieldNamesWriter.FieldNamesWriter fieldNamesWriter) : base(umbracoService, config, outOfDateModels, hostingEnvironment)
        {

            _umbracoService = umbracoService;
            _config = config.CurrentValue;
            _outOfDateModels = outOfDateModels;
            _hostingEnvironment = hostingEnvironment;
            _fieldNamesWriter = fieldNamesWriter;
            config.OnChange(x => _config = x);
        }

        public new void GenerateModels()
        {
            var modelsDirectory = _config.ModelsDirectoryAbsolute(_hostingEnvironment);
            if (!Directory.Exists(modelsDirectory))
            {
                Directory.CreateDirectory(modelsDirectory);
            }

            foreach (var file in Directory.GetFiles(modelsDirectory, "*.generated.cs"))
            {
                File.Delete(file);
            }

            IList<TypeModel> typeModels = _umbracoService.GetAllTypes();

            var builder = new TextBuilder(_config, typeModels);

            foreach (TypeModel typeModel in builder.GetModelsToGenerate())
            {
                var sb = new StringBuilder();
                builder.Generate(sb, typeModel);

                //The only custom line introduced
                _fieldNamesWriter.AddFieldNamesStruct(sb, typeModel);

                var filename = Path.Combine(modelsDirectory, typeModel.ClrName + ".generated.cs");
                File.WriteAllText(filename, sb.ToString());
            }

            _outOfDateModels.Clear();
        }
    }
}