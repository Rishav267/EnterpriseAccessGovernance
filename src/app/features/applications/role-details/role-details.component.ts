import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import {
  ApplicationsService,
  ApplicationRole,
  Permission
} from '../applications.service';

@Component({
  selector: 'app-role-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './role-details.component.html',
  styleUrl: './role-details.component.scss'
})
export class RoleDetailsComponent implements OnInit {

  private readonly route =
    inject(ActivatedRoute);

  private readonly applicationsService =
    inject(ApplicationsService);

  role: ApplicationRole | null = null;

  permissions: Permission[] = [];

  loading = true;

  errorMessage = '';

  applicationId = '';

  roleId = '';

  ngOnInit(): void {

    this.applicationId =
      this.route.snapshot.paramMap.get(
        'applicationId'
      ) ?? '';

    this.roleId =
      this.route.snapshot.paramMap.get(
        'roleId'
      ) ?? '';

    if (!this.applicationId) {

      this.errorMessage =
        'Application ID is missing.';

      this.loading = false;

      return;
    }

    if (!this.roleId) {

      this.errorMessage =
        'Role ID is missing.';

      this.loading = false;

      return;
    }

    this.loadRoleDetails();
  }

  private loadRoleDetails(): void {

    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      roles:
        this.applicationsService.getRoles(
          this.applicationId
        ),

      permissions:
        this.applicationsService.getPermissions(
          this.applicationId,
          this.roleId
        )
    })
    .subscribe({

      next: result => {

        this.role =
          result.roles.find(
            x => x.id === this.roleId
          ) ?? null;

        this.permissions =
          result.permissions;

        if (!this.role) {

          this.errorMessage =
            'Role was not found.';

          this.loading = false;

          return;
        }

        this.loading = false;
      },

      error: error => {

        console.error(
          'Failed to load role details',
          error
        );

        this.errorMessage =
          'Unable to load role details.';

        this.loading = false;
      }

    });
  }
}