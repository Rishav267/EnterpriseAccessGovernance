using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Dashboard;
using EnterpriseAccessGovernance.Application.Features.Imports.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Imports.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseAccessGovernance.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IImportService, ImportService>();

        return services;
    }
}