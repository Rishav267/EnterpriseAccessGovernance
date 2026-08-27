import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },

  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component')
        .then(m => m.DashboardComponent)
  },

  {
    path: 'employees',
    loadComponent: () =>
      import('./features/employees/employees.component')
        .then(m => m.EmployeesComponent)
  },

  {
    path: 'employees/:id',
    loadComponent: () =>
      import('./features/employees/employee-details/employee-details.component')
        .then(m => m.EmployeeDetailsComponent)
  },

  {
    path: 'employees/:employeeId/access-assignments',
    loadComponent: () =>
      import('./features/employees/access-assignments/access-assignments.component')
        .then(m => m.AccessAssignmentsComponent)
  },
  {
    path: 'applications',
    loadComponent: () =>
      import('./features/applications/applications.component')
        .then(m => m.ApplicationsComponent)
  },
  {
    path: 'applications/:applicationId',
    loadComponent: () =>
      import(
        './features/applications/application-details/application-details.component'
      ).then(
        m => m.ApplicationDetailsComponent
      )
  },
  {
    path: 'applications/:applicationId/roles/:roleId',
    loadComponent: () =>
      import(
        './features/applications/role-details/role-details.component'
      ).then(
        m => m.RoleDetailsComponent
      )
  },
  {
    path: 'employees/:employeeId/risk-findings',
    loadComponent: () =>
      import(
        './features/risk-findings/risk-findings.component'
      ).then(m => m.RiskFindingsComponent)
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];