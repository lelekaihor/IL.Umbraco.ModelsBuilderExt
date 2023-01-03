using CustomModelsBuilder.ModelsGenerator;
using CustomModelsBuilder.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Infrastructure.ModelsBuilder.Building;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.ModelsBuilder;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Extensions;

namespace CustomModelsBuilder.Controllers;

/// <summary>
///     API controller for use in the Umbraco back office with Angular resources
/// </summary>
/// <remarks>
///     We've created a different controller for the backoffice/angular specifically this is to ensure that the
///     correct CSRF security is adhered to for angular and it also ensures that this controller is not subseptipal to
///     global WebApi formatters being changed since this is always forced to only return Angular JSON Specific formats.
/// </remarks>
[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class CustomModelsBuilderDashboardController : UmbracoAuthorizedJsonController
{
    private readonly ModelsBuilderSettings _config;
    private readonly CustomDashboardReport _dashboardReport;
    private readonly ModelsGenerationError _mbErrors;
    //Switched to use CustomModelsGenerator
    private readonly CustomModelsGenerator _modelGenerator;
    private readonly OutOfDateModelsStatus _outOfDateModels;

    public CustomModelsBuilderDashboardController(IOptions<ModelsBuilderSettings> config, CustomModelsGenerator modelsGenerator,
        OutOfDateModelsStatus outOfDateModels, ModelsGenerationError mbErrors)
    {
        _config = config.Value;
        _modelGenerator = modelsGenerator;
        _outOfDateModels = outOfDateModels;
        _mbErrors = mbErrors;
        _dashboardReport = new CustomDashboardReport(config, outOfDateModels, mbErrors);
    }

    // invoked by the dashboard
    // requires that the user is logged into the backoffice and has access to the settings section
    // beware! the name of the method appears in modelsbuilder.controller.js
    [HttpPost] // use the http one, not mvc, with api controllers!
    public IActionResult BuildModels()
    {
        try
        {
            if (!_config.ModelsMode.SupportsExplicitGeneration())
            {
                var result2 = new ModelsBuilderDashboardController.BuildResult { Success = false, Message = "Models generation is not enabled." };

                return Ok(result2);
            }

            _modelGenerator.GenerateModels();
            _mbErrors.Clear();
        }
        catch (Exception e)
        {
            _mbErrors.Report("Failed to build models.", e);
        }

        return Ok(GetDashboardResult());
    }

    // invoked by the back-office
    // requires that the user is logged into the backoffice and has access to the settings section
    [HttpGet] // use the http one, not mvc, with api controllers!
    public ActionResult<ModelsBuilderDashboardController.OutOfDateStatus> GetModelsOutOfDateStatus()
    {
        ModelsBuilderDashboardController.OutOfDateStatus status = _outOfDateModels.IsEnabled
            ? _outOfDateModels.IsOutOfDate
                ? new ModelsBuilderDashboardController.OutOfDateStatus { Status = ModelsBuilderDashboardController.OutOfDateType.OutOfDate }
                : new ModelsBuilderDashboardController.OutOfDateStatus { Status = ModelsBuilderDashboardController.OutOfDateType.Current }
            : new ModelsBuilderDashboardController.OutOfDateStatus { Status = ModelsBuilderDashboardController.OutOfDateType.Unknown };

        return status;
    }

    // invoked by the back-office
    // requires that the user is logged into the backoffice and has access to the settings section
    // beware! the name of the method appears in modelsbuilder.controller.js
    [HttpGet] // use the http one, not mvc, with api controllers!
    public ActionResult<ModelsBuilderDashboardController.Dashboard> GetDashboard() => GetDashboardResult();

    private ModelsBuilderDashboardController.Dashboard GetDashboardResult() => new()
    {
        Mode = _config.ModelsMode,
        Text = _dashboardReport.Text(),
        CanGenerate = _dashboardReport.CanGenerate(),
        OutOfDateModels = _dashboardReport.AreModelsOutOfDate(),
        LastError = _dashboardReport.LastError()
    };
}