using CustomModelsBuilder.Controllers;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Web.Common.ModelsBuilder;
using Umbraco.Extensions;

namespace CustomModelsBuilder.DashboardProvider
{
    public class CustomModelsBuilderDashboardProvider : IModelsBuilderDashboardProvider
    {
        private readonly LinkGenerator _linkGenerator;

        public CustomModelsBuilderDashboardProvider(LinkGenerator linkGenerator) => _linkGenerator = linkGenerator;

        public string? GetUrl() => _linkGenerator.GetUmbracoApiServiceBaseUrl<CustomModelsBuilderDashboardController>(controller => controller.BuildModels());
    }
}