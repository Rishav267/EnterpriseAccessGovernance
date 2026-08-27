import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  Employee,
  EmployeesService,
  RiskFinding
} from '../employees.service';

@Component({
  selector: 'app-employee-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './employee-details.component.html',
  styleUrl: './employee-details.component.scss'
})
export class EmployeeDetailsComponent
  implements OnInit {

  private readonly route =
    inject(ActivatedRoute);

  private readonly employeesService =
    inject(EmployeesService);


  employee: Employee | null = null;

  riskFindings: RiskFinding[] = [];

  loading = false;

  riskFindingsLoading = false;

  errorMessage = '';

  riskFindingsErrorMessage = '';


  ngOnInit(): void {

    const id =
      this.route.snapshot.paramMap.get('id');

    if (!id) {

      this.errorMessage =
        'Employee ID was not provided.';

      return;
    }

    this.loadEmployee(id);

    this.loadRiskFindings(id);
  }


  private loadEmployee(
    id: string
  ): void {

    this.loading = true;

    this.errorMessage = '';

    this.employeesService
      .getEmployeeById(id)
      .subscribe({

        next: (employee) => {

          this.employee = employee;

          this.loading = false;

        },

        error: (error) => {

          console.error(
            'Failed to load employee',
            error
          );

          this.errorMessage =
            'Unable to load employee details.';

          this.loading = false;

        }

      });

  }


  private loadRiskFindings(
    employeeId: string
  ): void {

    this.riskFindingsLoading = true;

    this.riskFindingsErrorMessage = '';

    this.employeesService
      .getRiskFindings(employeeId)
      .subscribe({

        next: (findings) => {

          this.riskFindings = findings;

          this.riskFindingsLoading = false;

        },

        error: (error) => {

          console.error(
            'Failed to load risk findings',
            error
          );

          this.riskFindingsErrorMessage =
            'Unable to load risk findings.';

          this.riskFindingsLoading = false;

        }

      });

  }


  resolveRiskFinding(
    finding: RiskFinding
  ): void {

    if (!this.employee) {
      return;
    }

    this.employeesService
      .resolveRiskFinding(
        this.employee.id,
        finding.id
      )
      .subscribe({

        next: () => {

          this.loadRiskFindings(
            this.employee!.id
          );

        },

        error: (error) => {

          console.error(
            'Failed to resolve risk finding',
            error
          );

          this.riskFindingsErrorMessage =
            'Unable to resolve risk finding.';

        }

      });

  }


  ignoreRiskFinding(
    finding: RiskFinding
  ): void {

    if (!this.employee) {
      return;
    }

    this.employeesService
      .ignoreRiskFinding(
        this.employee.id,
        finding.id
      )
      .subscribe({

        next: () => {

          this.loadRiskFindings(
            this.employee!.id
          );

        },

        error: (error) => {

          console.error(
            'Failed to ignore risk finding',
            error
          );

          this.riskFindingsErrorMessage =
            'Unable to ignore risk finding.';

        }

      });

  }

}