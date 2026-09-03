import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ImportResponse {
  importBatchId: string;
  status: string;
  totalRecords: number;
  successfulRecords: number;
  failedRecords: number;
  errors: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ImportsService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5249/api/import';

  uploadFile(file: File): Observable<ImportResponse> {

    const formData = new FormData();

    formData.append('file', file, file.name);

    return this.http.post<ImportResponse>(
      this.apiUrl,
      formData
    );
  }
}