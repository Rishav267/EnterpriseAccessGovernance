using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Mapping;

public sealed class ImportHeaderMapper : IImportHeaderMapper
{
    private readonly IHeaderNormalizer _headerNormalizer;
    private readonly IImportFieldMappingProvider _mappingProvider;

    public ImportHeaderMapper(
        IHeaderNormalizer headerNormalizer,
        IImportFieldMappingProvider mappingProvider)
    {
        _headerNormalizer = headerNormalizer;
        _mappingProvider = mappingProvider;
    }

    public ImportMappingResult Map(
        IReadOnlyCollection<string> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        var definitions =
            _mappingProvider.GetDefinitions();

        var mappings =
            new List<ImportHeaderMapping>();

        var unknownHeaders =
            new List<string>();

        var ambiguousHeaders =
            new List<string>();

        var mappedFields =
            new HashSet<ImportField>();

        foreach (var sourceHeader in headers)
        {
            if (string.IsNullOrWhiteSpace(sourceHeader))
            {
                continue;
            }

            var normalizedHeader =
                _headerNormalizer.Normalize(sourceHeader);

            var matchingDefinitions =
                definitions
                    .Where(
                        definition =>
                            definition.Aliases.Any(
                                alias =>
                                    string.Equals(
                                        _headerNormalizer.Normalize(alias),
                                        normalizedHeader,
                                        StringComparison.OrdinalIgnoreCase)))
                    .ToList();

            if (matchingDefinitions.Count == 0)
            {
                unknownHeaders.Add(sourceHeader);
                continue;
            }

            if (matchingDefinitions.Count > 1)
            {
                ambiguousHeaders.Add(sourceHeader);
                continue;
            }

            var definition =
                matchingDefinitions[0];

            if (!mappedFields.Add(definition.Field))
            {
                ambiguousHeaders.Add(sourceHeader);
                continue;
            }

            mappings.Add(
                new ImportHeaderMapping
                {
                    Field = definition.Field,
                    SourceHeader = sourceHeader
                });
        }

        return new ImportMappingResult
        {
            Mappings = mappings,
            MissingRequiredFields = Array.Empty<string>(),
            UnknownHeaders = unknownHeaders,
            AmbiguousHeaders = ambiguousHeaders
        };
    }
}