using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Common.Models;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Infrastructure.Import.Processors;

public sealed class EmployeeImportProcessor
    : IImportDatasetProcessor
{
    private readonly IImportRepository _importRepository;
    private readonly IImportRowValidator _rowValidator;

    public EmployeeImportProcessor(
        IImportRepository importRepository,
        IImportRowValidator rowValidator)
    {
        _importRepository = importRepository;
        _rowValidator = rowValidator;
    }

    public DatasetType DatasetType =>
        DatasetType.Employees;

    public async Task<ImportProcessingResult> ProcessAsync(
        IReadOnlyCollection<CanonicalImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var successfulRecords = 0;

        var errors =
            new List<ImportProcessingError>();

        var rowNumber = 1;

        // Track values within the current file as well.
        var employeeNumbersInFile =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        var emailsInFile =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            rowNumber++;

            try
            {
                var validationResult =
                    _rowValidator.Validate(
                        row,
                        DatasetType.Employees);

                if (!validationResult.IsValid)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage = string.Join(
                                " ",
                                validationResult.Errors)
                        });

                    continue;
                }

                var employeeNumber =
                    row.Get(ImportField.EmployeeId)!;

                var employeeName =
                    row.Get(ImportField.EmployeeName)!;

                var email =
                    row.Get(ImportField.Email)!;

                var departmentName =
                    row.Get(ImportField.Department)!;

                employeeNumber =
                    employeeNumber.Trim();

                email =
                    email.Trim()
                        .ToLowerInvariant();

                // =====================================================
                // Duplicate Employee Number within uploaded file
                // =====================================================

                if (!employeeNumbersInFile.Add(employeeNumber))
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                $"Employee '{employeeNumber}' appears more than once in the uploaded file."
                        });

                    continue;
                }

                // =====================================================
                // Duplicate Email within uploaded file
                // =====================================================

                if (!emailsInFile.Add(email))
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                $"Email '{email}' appears more than once in the uploaded file."
                        });

                    continue;
                }

                // =====================================================
                // Existing Employee Number
                // =====================================================

                var existingEmployeeByNumber =
                    await _importRepository
                        .GetEmployeeByEmployeeNumberAsync(
                            employeeNumber,
                            cancellationToken);

                if (existingEmployeeByNumber is not null)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                $"Employee '{employeeNumber}' already exists. Existing employee records are not updated by import."
                        });

                    continue;
                }

                // =====================================================
                // Existing Email
                // =====================================================

                var existingEmployeeByEmail =
                    await _importRepository
                        .GetEmployeeByEmailAsync(
                            email,
                            cancellationToken);

                if (existingEmployeeByEmail is not null)
                {
                    errors.Add(
                        new ImportProcessingError
                        {
                            RowNumber = rowNumber,
                            ErrorMessage =
                                $"Email '{email}' is already associated with an existing employee. Existing employee records are not updated by import."
                        });

                    continue;
                }

                // =====================================================
                // Department
                // =====================================================

                var department =
                    await _importRepository
                        .GetDepartmentByNameAsync(
                            departmentName,
                            cancellationToken);

                if (department is null)
                {
                    var departmentCode =
                        await GenerateUniqueDepartmentCodeAsync(
                            departmentName,
                            cancellationToken);

                    department =
                        Department.Create(
                            departmentName,
                            departmentCode);

                    await _importRepository
                        .AddDepartmentAsync(
                            department,
                            cancellationToken);
                }

                // =====================================================
                // Employee
                // =====================================================

                var (firstName, lastName) =
                    SplitEmployeeName(employeeName);

                var employee =
                    Employee.Create(
                        employeeNumber,
                        firstName,
                        lastName,
                        email,
                        EmploymentStatus.Active,
                        department.Id);

                await _importRepository
                    .AddEmployeeAsync(
                        employee,
                        cancellationToken);

                successfulRecords++;
            }
            catch (Exception exception)
            {
                errors.Add(
                    new ImportProcessingError
                    {
                        RowNumber = rowNumber,
                        ErrorMessage =
                            exception.Message
                    });
            }
        }

        await _importRepository
            .SaveChangesAsync(
                cancellationToken);

        if (errors.Count == 0)
        {
            return ImportProcessingResult.Success();
        }

        return ImportProcessingResult.PartialFailure(errors);
    }

    private static (
        string FirstName,
        string LastName)
        SplitEmployeeName(
            string employeeName)
    {
        var nameParts =
            employeeName
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

        if (nameParts.Length == 1)
        {
            return (
                nameParts[0],
                nameParts[0]);
        }

        var firstName =
            nameParts[0];

        var lastName =
            string.Join(
                " ",
                nameParts.Skip(1));

        return (
            firstName,
            lastName);
    }

    private async Task<string> GenerateUniqueDepartmentCodeAsync(
        string departmentName,
        CancellationToken cancellationToken)
    {
        var baseCode =
            CreateDepartmentCode(
                departmentName);

        var departmentCode =
            baseCode;

        var suffix = 1;

        while (await _importRepository
            .GetDepartmentByCodeAsync(
                departmentCode,
                cancellationToken) is not null)
        {
            suffix++;

            departmentCode =
                $"{baseCode}{suffix}";
        }

        return departmentCode;
    }

    private static string CreateDepartmentCode(
        string departmentName)
    {
        var words =
            departmentName
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

        var code =
            string.Concat(
                words.Select(
                    word =>
                        char.ToUpperInvariant(
                            word[0])));

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException(
                "Unable to generate department code.");
        }

        return code;
    }
}