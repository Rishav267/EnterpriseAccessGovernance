import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  ApplicationsService,
  Application
} from './applications.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [
    CommonModule, FormsModule, RouterLink
  ],
  templateUrl: './applications.component.html',
  styleUrl: './applications.component.scss'
})
export class ApplicationsComponent implements OnInit {

  private readonly applicationsService =
    inject(ApplicationsService);

  applications: Application[] = [];

  filteredApplications: Application[] = [];

  searchTerm = '';

  loading = true;

  errorMessage = '';

  ngOnInit(): void {
    this.loadApplications();
  }

  private loadApplications(): void {

    this.loading = true;

    this.applicationsService
      .getApplications()
      .subscribe({

        next: applications => {
          this.applications = applications;
          this.filteredApplications = applications;
          this.loading = false;
        },

        error: error => {

          console.error(
            'Failed to load applications',
            error);

          this.errorMessage =
            'Failed to load applications.';

          this.loading = false;
        }

      });
  }

  onSearch(): void {

    const term =
      this.searchTerm
        .trim()
        .toLowerCase();

    if (!term) {
      this.filteredApplications =
        this.applications;

      return;
    }

    this.filteredApplications =
      this.applications.filter(
        application =>
          application.name
            .toLowerCase()
            .includes(term) ||
          application.code
            .toLowerCase()
            .includes(term)
      );
  }
}