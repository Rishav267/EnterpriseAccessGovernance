using ClosedXML.Excel;
using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Readers;

public sealed class ExcelImportFileReader : IImportFileReader
{
    private const string SupportedExtension = ".xlsx";

    private readonly IHeaderNormalizer _headerNormalizer;

    public ExcelImportFileReader(
        IHeaderNormalizer headerNormalizer)
    {
        _headerNormalizer = headerNormalizer;
    }

    public bool CanRead(string fileExtension)
    {
        return string.Equals(
            fileExtension?.Trim(),
            SupportedExtension,
            StringComparison.OrdinalIgnoreCase);
    }

    public Task<ImportFileData> ReadAsync(
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

        cancellationToken.ThrowIfCancellationRequested();

        using var workbook = new XLWorkbook(fileStream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet is null)
        {
            return Task.FromResult(new ImportFileData());
        }

        var firstRow = worksheet.FirstRowUsed();

        if (firstRow is null)
        {
            return Task.FromResult(new ImportFileData());
        }

        var lastRow = worksheet.LastRowUsed();

        if (lastRow is null)
        {
            return Task.FromResult(new ImportFileData());
        }

        var firstCell = firstRow.FirstCellUsed();
        var lastCell = firstRow.LastCellUsed();

        if (firstCell is null || lastCell is null)
        {
            return Task.FromResult(new ImportFileData());
        }

        var firstColumnNumber =
            firstCell.Address.ColumnNumber;

        var lastColumnNumber =
            lastCell.Address.ColumnNumber;

        /*
         * Detect whether the worksheet contains normal
         * Excel columns or CSV-like content inside one cell.
         *
         * Example of CSV-like Excel content:
         *
         * A1:
         * employeeid,employeename,email,department
         *
         * A2:
         * E001,John Doe,john@company.com,IT
         */
        var usedColumnCount =
            lastColumnNumber - firstColumnNumber + 1;

        var firstCellValue =
            firstCell.GetString();

        var isCsvLikeExcel =
            usedColumnCount == 1 &&
            firstCellValue.Contains(',');

        if (isCsvLikeExcel)
        {
            return Task.FromResult(
                ReadCsvLikeWorksheet(
                    worksheet,
                    firstRow.RowNumber(),
                    lastRow.RowNumber(),
                    firstColumnNumber,
                    cancellationToken));
        }

        return Task.FromResult(
            ReadNormalExcelWorksheet(
                worksheet,
                firstRow.RowNumber(),
                lastRow.RowNumber(),
                firstColumnNumber,
                lastColumnNumber,
                cancellationToken));
    }

    private ImportFileData ReadNormalExcelWorksheet(
        IXLWorksheet worksheet,
        int firstRowNumber,
        int lastRowNumber,
        int firstColumnNumber,
        int lastColumnNumber,
        CancellationToken cancellationToken)
    {
        var headers = new List<string>();

        /*
         * Read headers.
         */
        for (
            var columnNumber = firstColumnNumber;
            columnNumber <= lastColumnNumber;
            columnNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var header =
                worksheet
                    .Cell(firstRowNumber, columnNumber)
                    .GetString();

            var normalizedHeader =
                _headerNormalizer.Normalize(header);

            if (!string.IsNullOrWhiteSpace(normalizedHeader))
            {
                headers.Add(normalizedHeader);
            }
        }

        var rows =
            new List<IReadOnlyDictionary<string, string?>>();

        /*
         * Read data rows.
         */
        for (
            var rowNumber = firstRowNumber + 1;
            rowNumber <= lastRowNumber;
            rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row =
                new Dictionary<string, string?>(
                    StringComparer.OrdinalIgnoreCase);

            var hasValue = false;

            var headerIndex = 0;

            for (
                var columnNumber = firstColumnNumber;
                columnNumber <= lastColumnNumber;
                columnNumber++)
            {
                var originalHeader =
                    worksheet
                        .Cell(firstRowNumber, columnNumber)
                        .GetString();

                var normalizedHeader =
                    _headerNormalizer.Normalize(originalHeader);

                if (string.IsNullOrWhiteSpace(normalizedHeader))
                {
                    continue;
                }

                var value =
                    worksheet
                        .Cell(rowNumber, columnNumber)
                        .GetString();

                var trimmedValue =
                    string.IsNullOrWhiteSpace(value)
                        ? null
                        : value.Trim();

                row[normalizedHeader] =
                    trimmedValue;

                if (!string.IsNullOrWhiteSpace(trimmedValue))
                {
                    hasValue = true;
                }

                headerIndex++;
            }

            if (!hasValue)
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

    private ImportFileData ReadCsvLikeWorksheet(
        IXLWorksheet worksheet,
        int firstRowNumber,
        int lastRowNumber,
        int firstColumnNumber,
        CancellationToken cancellationToken)
    {
        var headerCell =
            worksheet
                .Cell(firstRowNumber, firstColumnNumber)
                .GetString();

        var rawHeaders =
            SplitCsvLine(headerCell);

        var headers =
            rawHeaders
                .Select(_headerNormalizer.Normalize)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

        var rows =
            new List<IReadOnlyDictionary<string, string?>>();

        for (
            var rowNumber = firstRowNumber + 1;
            rowNumber <= lastRowNumber;
            rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rowCell =
                worksheet
                    .Cell(rowNumber, firstColumnNumber)
                    .GetString();

            if (string.IsNullOrWhiteSpace(rowCell))
            {
                continue;
            }

            var values =
                SplitCsvLine(rowCell);

            var row =
                new Dictionary<string, string?>(
                    StringComparer.OrdinalIgnoreCase);

            for (
                var index = 0;
                index < headers.Count;
                index++)
            {
                var value =
                    index < values.Count
                        ? values[index]
                        : null;

                row[headers[index]] =
                    string.IsNullOrWhiteSpace(value)
                        ? null
                        : value.Trim();
            }

            rows.Add(row);
        }

        return new ImportFileData
        {
            Headers = headers,
            Rows = rows
        };
    }

    private static List<string> SplitCsvLine(
        string line)
    {
        var values = new List<string>();

        if (string.IsNullOrEmpty(line))
        {
            return values;
        }

        var current = new System.Text.StringBuilder();

        var insideQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];

            if (character == '"')
            {
                /*
                 * Handle escaped quotes:
                 *
                 * ""
                 */
                if (
                    insideQuotes &&
                    index + 1 < line.Length &&
                    line[index + 1] == '"')
                {
                    current.Append('"');
                    index++;
                    continue;
                }

                insideQuotes = !insideQuotes;
                continue;
            }

            if (character == ',' && !insideQuotes)
            {
                values.Add(current.ToString());
                current.Clear();

                continue;
            }

            current.Append(character);
        }

        values.Add(current.ToString());

        return values;
    }
}