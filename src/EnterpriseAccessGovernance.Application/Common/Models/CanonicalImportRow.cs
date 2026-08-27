namespace EnterpriseAccessGovernance.Application.Common.Models;

public sealed class CanonicalImportRow
{
    private readonly Dictionary<ImportField, string?> _values = [];

    public void Set(
        ImportField field,
        string? value)
    {
        _values[field] = string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public string? Get(
        ImportField field)
    {
        return _values.TryGetValue(
            field,
            out var value)
            ? value
            : null;
    }

    public bool Has(
        ImportField field)
    {
        return _values.ContainsKey(field);
    }

    public IReadOnlyDictionary<ImportField, string?> Values =>
        _values;
}