using CustomModelsBuilder.Controllers;
using CustomModelsBuilder.DashboardProvider;
using CustomModelsBuilder.ModelsGenerator;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.BackOffice.ModelsBuilder;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.ModelsBuilder;
using Umbraco.Extensions;

namespace CustomModelsBuilder.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder EnrichEmbeddedModelsBuilderWithModelPropsAliases(this IUmbracoBuilder builder)
        {
            builder.WithCollectionBuilder<UmbracoApiControllerTypeCollectionBuilder>().Remove<ModelsBuilderDashboardController>();
            builder.Services.AddUnique<IModelsBuilderDashboardProvider, CustomModelsBuilderDashboardProvider>();
            builder.WithCollectionBuilder<UmbracoApiControllerTypeCollectionBuilder>().Add<CustomModelsBuilderDashboardController>();
            builder.Services.AddSingleton<CustomModelsGenerator>();
            builder.Services.AddSingleton<FieldNamesWriter.FieldNamesWriter>();

            return builder;
        }
    }
}