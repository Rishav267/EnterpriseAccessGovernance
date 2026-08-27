using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Readers;

public sealed class CsvImportFileReader : IImportFileReader
{
    private readonly IHeaderNormalizer _headerNormalizer;
    private static readonly string[] SupportedExtensions =
    {
        ".csv"
    };
    public CsvImportFileReader(IHeaderNormalizer headerNormalizer)
    {
        _headerNormalizer = headerNormalizer;
    }
    public bool CanRead(string fileExtension)
    {
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return false;
        }

        return SupportedExtensions.Contains(
            fileExtension.Trim(),
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task<ImportFileData> ReadAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        if (!fileStream.CanRead)
        {
            throw new ArgumentException(
                "The provided stream cannot be read.",
                nameof(fileStream));
        }

        using var streamReader = new StreamReader(
            fileStream,
            detectEncodingFromByteOrderMarks: true,
            leaveOpen: true);

        var configuration = new CsvConfiguration(
            CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var csv = new CsvReader(
            streamReader,
            configuration);

        if (!await csv.ReadAsync())
        {
            return new ImportFileData();
        }

        csv.ReadHeader();

        var headers = csv.HeaderRecord?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(_headerNormalizer.Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            ?? new List<string>();

        var rows =
            new List<IReadOnlyDictionary<string, string?>>();

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row =
                new Dictionary<string, string?>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var header in csv.HeaderRecord ?? Array.Empty<string>())
            {
                var normalizedHeader =
                    _headerNormalizer.Normalize(header);

                if (string.IsNullOrWhiteSpace(normalizedHeader))
                {
                    continue;
                }

                var value = csv.GetField(header);

                row[normalizedHeader] =
                    string.IsNullOrWhiteSpace(value)
                        ? null
                        : value.Trim();
            }

            if (row.Values.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            rows.Add(row);
        }

        return new ImportFileData
        {
            Headers = headers,
            Rows = rows
        };
    }
}