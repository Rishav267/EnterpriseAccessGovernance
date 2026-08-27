import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  Employee,
  EmployeesService
} from './employees.service';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.scss'
})
export class EmployeesComponent implements OnInit {

  private readonly employeesService =
    inject(EmployeesService);

  employees: Employee[] = [];

  filteredEmployees: Employee[] = [];

  searchTerm = '';

  selectedDepartment = '';
  selectedStatus = '';

  departments: string[] = [];
  statuses: string[] = [];

  loading = false;

  errorMessage = '';

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.loading = true;
    this.errorMessage = '';

    this.employeesService
      .getEmployees()
      .subscribe({
        next: (employees) => {
            this.employees = employees;

            this.departments = [
              ...new Set(
                employees
                  .map(employee => employee.departmentName)
                  .filter(Boolean)
              )
            ];

            this.statuses = [
              ...new Set(
                employees
                  .map(employee => employee.employmentStatus)
                  .filter(Boolean)
              )
            ];

            this.applyFilters();

            this.loading = false;
          },
        error: (error) => {
          console.error(
            'Failed to load employees',
            error
          );

          this.errorMessage =
            'Unable to load employees. Please try again.';

          this.loading = false;
        }
      });
  }

  onSearch(): void {
    this.applyFilters();
  }

  onDepartmentChange(): void {
    this.applyFilters();
  }

  onStatusChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    const search =
      this.searchTerm
        .trim()
        .toLowerCase();

    this.filteredEmployees =
      this.employees.filter(employee => {

        const matchesSearch =
          !search ||
          employee.employeeNumber
            .toLowerCase()
            .includes(search) ||
          employee.fullName
            .toLowerCase()
            .includes(search) ||
          employee.email
            .toLowerCase()
            .includes(search) ||
          employee.departmentName
            .toLowerCase()
            .includes(search);

        const matchesDepartment =
          !this.selectedDepartment ||
          employee.departmentName ===
            this.selectedDepartment;

        const matchesStatus =
          !this.selectedStatus ||
          employee.employmentStatus ===
            this.selectedStatus;

        return (
          matchesSearch &&
          matchesDepartment &&
          matchesStatus
        );
      });
  }
}