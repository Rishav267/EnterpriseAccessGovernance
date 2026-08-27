using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Infrastructure.Import.Detection;
using EnterpriseAccessGovernance.Infrastructure.Import.HeaderNormalization;
using EnterpriseAccessGovernance.Infrastructure.Import.Mapping;
using EnterpriseAccessGovernance.Infrastructure.Import.Processors;
using EnterpriseAccessGovernance.Infrastructure.Import.Readers;
using EnterpriseAccessGovernance.Infrastructure.Import.Validation;
using EnterpriseAccessGovernance.Infrastructure.Persistence;
using EnterpriseAccessGovernance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseAccessGovernance.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");
        }

        // =========================================================
        // Database
        // =========================================================

        services.AddDbContext<EnterpriseAccessGovernanceDbContext>(
            options =>
                options.UseSqlServer(connectionString));

        // =========================================================
        // Import infrastructure
        // =========================================================

        services.AddSingleton<
            IHeaderNormalizer,
            HeaderNormalizer>();

        services.AddSingleton<
            IImportFieldMappingProvider,
            ImportFieldMappingProvider>();

        services.AddSingleton<
            IImportHeaderMapper,
            ImportHeaderMapper>();

        services.AddSingleton<
            IDatasetDetector,
            DatasetDetector>();

        services.AddSingleton<
            IImportRowMapper,
            ImportRowMapper>();

        services.AddSingleton<
            IImportRowValidator,
            ImportRowValidator>();

        // =========================================================
        // File readers
        // =========================================================

        services.AddScoped<
            IImportFileReader,
            CsvImportFileReader>();

        services.AddScoped<
            IImportFileReader,
            ExcelImportFileReader>();

        // =========================================================
        // Dataset processors
        // =========================================================

        services.AddScoped<
            IImportDatasetProcessor,
            DepartmentImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            EmployeeImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            ApplicationImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            RoleImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            PermissionImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            RolePermissionImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            AccessAssignmentImportProcessor>();

        services.AddScoped<
            IImportDatasetProcessor,
            LoginActivityImportProcessor>();

        services.AddScoped<IImportDatasetProcessor, RiskFindingImportProcessor>();

        // =========================================================
        // Repositories
        // =========================================================

        services.AddScoped<
            IDashboardRepository,
            DashboardRepository>();

        services.AddScoped<
            IImportRepository,
            ImportRepository>();

        services.AddScoped<
            IImportDataRepository,
            ImportDataRepository>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IAccessAssignmentRepository, AccessAssignmentRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IApplicationRoleRepository, ApplicationRoleRepository>();
        services.AddScoped<IRiskFindingRepository, RiskFindingRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IRiskDetectionRepository, RiskDetectionRepository>();

        return services;
    }
}