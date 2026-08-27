using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.AccessAssignments;
using EnterpriseAccessGovernance.Application.Features.ApplicationRoles;
using EnterpriseAccessGovernance.Application.Features.Applications;
using EnterpriseAccessGovernance.Application.Features.Dashboard;
using EnterpriseAccessGovernance.Application.Features.Employees;
using EnterpriseAccessGovernance.Application.Features.Imports.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Imports.Services;
using EnterpriseAccessGovernance.Application.Features.RiskFindings;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseAccessGovernance.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<IEmployeeService, EmployeeService>();

        services.AddScoped<IImportService, ImportService>();

        services.AddScoped<IAccessAssignmentService, AccessAssignmentService>();

        services.AddScoped<IApplicationService, ApplicationService>();

        services.AddScoped<IApplicationRoleService, ApplicationRoleService>();

        services.AddScoped<IRiskFindingService, RiskFindingService>();

        return services;
    }
}