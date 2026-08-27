using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;

namespace EnterpriseAccessGovernance.Application.Features.RiskFindings;

public interface IRiskDetectionService
{
    Task<int> RunAsync(
        int dormantDays = 90,
        int excessiveApplicationThreshold = 5,
        CancellationToken cancellationToken = default);

    //Task<RiskDetectionResultDto> DetectAsync(
    //    int dormantDays = 90,
    //    int excessiveApplicationThreshold = 5,
    //    CancellationToken cancellationToken = default);
}
