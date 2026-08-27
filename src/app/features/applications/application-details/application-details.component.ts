import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import {
  ApplicationsService,
  Application,
  ApplicationRole
} from '../applications.service';

@Component({
  selector: 'app-application-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './application-details.component.html',
  styleUrl: './application-details.component.scss'
})
export class ApplicationDetailsComponent implements OnInit {

  private readonly route =
    inject(ActivatedRoute);

  private readonly applicationsService =
    inject(ApplicationsService);

  application: Application | null = null;

  roles: ApplicationRole[] = [];

  loading = true;

  errorMessage = '';

  applicationId = '';

  ngOnInit(): void {

    this.applicationId =
      this.route.snapshot.paramMap.get(
        'applicationId'
      ) ?? '';

    if (!this.applicationId) {

      this.errorMessage =
        'Application ID is missing.';

      this.loading = false;

      return;
    }

    this.loadApplicationDetails();
  }

  private loadApplicationDetails(): void {

    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      application:
        this.applicationsService.getApplication(
          this.applicationId
        ),

      roles:
        this.applicationsService.getRoles(
          this.applicationId
        )
    })
    .subscribe({

      next: result => {

        this.application =
          result.application;

        this.roles =
          result.roles;

        this.loading = false;
      },

      error: error => {

        console.error(
          'Failed to load application details',
          error
        );

        this.errorMessage =
          'Unable to load application details.';

        this.loading = false;
      }

    });
  }
}