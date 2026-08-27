using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.Applications.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.Applications;

public sealed class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;

    public ApplicationService(
        IApplicationRepository applicationRepository)
    {
        _applicationRepository =
            applicationRepository
            ?? throw new ArgumentNullException(
                nameof(applicationRepository));
    }

    public Task<IReadOnlyCollection<ApplicationListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _applicationRepository.GetAllAsync(
            cancellationToken);
    }

    public Task<ApplicationDetailsDto?> GetByIdAsync(
    Guid applicationId,
    CancellationToken cancellationToken = default)
    {
        if (applicationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application ID is required.",
                nameof(applicationId));
        }

        return _applicationRepository.GetByIdAsync(
            applicationId,
            cancellationToken);
    }
}