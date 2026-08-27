import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Application {
  id: string;
  name: string;
  code: string;
  description: string | null;
  roleCount: number;
}

export interface ApplicationRole {
  id: string;
  enterpriseApplicationId: string;
  name: string;
  code: string;
  isHighPrivilege: boolean;
  permissionCount: number;
}

export interface Permission {
  id: string;
  name: string;
  code: string;
  description: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ApplicationsService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5249/api/applications';

  getApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(
      this.apiUrl
    );
  }

  getApplication(id: string): Observable<Application> {
    return this.http.get<Application>(
        `${this.apiUrl}/${id}`
    );
  }

    getRoles(applicationId: string): Observable<ApplicationRole[]> {
    return this.http.get<ApplicationRole[]>(
        `${this.apiUrl}/${applicationId}/roles`
    );
    }
   getPermissions(
    applicationId: string,
    roleId: string
    ): Observable<Permission[]> {
    return this.http.get<Permission[]>(
        `${this.apiUrl}/${applicationId}/roles/${roleId}/permissions`
    );
    }
}
