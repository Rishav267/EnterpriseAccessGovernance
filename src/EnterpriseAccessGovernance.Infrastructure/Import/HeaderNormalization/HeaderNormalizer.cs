using EnterpriseAccessGovernance.Application.Common.Interfaces;

namespace EnterpriseAccessGovernance.Infrastructure.Import.HeaderNormalization;

public sealed class HeaderNormalizer : IHeaderNormalizer
{
    public string Normalize(string header)
    {
        if (string.IsNullOrWhiteSpace(header))
        {
            return string.Empty;
        }

        return header
            .Trim()
            .Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }
}