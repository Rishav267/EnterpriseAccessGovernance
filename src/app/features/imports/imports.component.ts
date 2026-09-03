import {
  Component,
  inject
} from '@angular/core';

import {
  CommonModule
} from '@angular/common';

import {
  ImportsService,
  ImportResponse
} from './imports.service';


@Component({
  selector: 'app-imports',
  standalone: true,

  imports: [
    CommonModule
  ],

  templateUrl:
    './imports.component.html',

  styleUrl:
    './imports.component.scss'
})
export class ImportsComponent {

  private readonly importsService =
    inject(ImportsService);


  // =========================================================
  // Configuration
  // =========================================================

  readonly maxFileSizeMb = 10;

  readonly maxFileSizeBytes =
    this.maxFileSizeMb * 1024 * 1024;

  readonly allowedExtensions = [
    '.csv',
    '.xlsx'
  ];


  // =========================================================
  // State
  // =========================================================

  selectedFile: File | null = null;

  uploading = false;

  uploadResult: ImportResponse | null = null;

  errorMessage = '';

  isDragOver = false;


  // =========================================================
  // File Selection
  // =========================================================

  onFileSelected(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    if (!input.files ||
        input.files.length === 0) {

      return;
    }

    this.selectFile(
      input.files[0]
    );

    input.value = '';
  }


  // =========================================================
  // Select File
  // =========================================================

  private selectFile(
    file: File
  ): void {

    this.clearMessages();

    const validationError =
      this.validateFile(file);

    if (validationError) {

      this.errorMessage =
        validationError;

      this.selectedFile = null;

      return;
    }

    this.selectedFile = file;

    this.uploadResult = null;
  }


  // =========================================================
  // Validation
  // =========================================================

  private validateFile(
    file: File
  ): string | null {

    if (!file) {

      return 'Please select a file.';
    }

    if (file.size === 0) {

      return 'The selected file is empty.';
    }

    if (file.size >
        this.maxFileSizeBytes) {

      return `File size must not exceed ${this.maxFileSizeMb} MB.`;
    }

    const fileName =
      file.name.toLowerCase();

    const isAllowed =
      this.allowedExtensions.some(
        extension =>
          fileName.endsWith(extension)
      );

    if (!isAllowed) {

      return 'Only CSV and XLSX files are supported.';
    }

    return null;
  }


  // =========================================================
  // Drag & Drop
  // =========================================================

  onDragOver(
    event: DragEvent
  ): void {

    event.preventDefault();

    event.stopPropagation();

    this.isDragOver = true;
  }


  onDragLeave(
    event: DragEvent
  ): void {

    event.preventDefault();

    event.stopPropagation();

    this.isDragOver = false;
  }


  onDrop(
    event: DragEvent
  ): void {

    event.preventDefault();

    event.stopPropagation();

    this.isDragOver = false;

    const files =
      event.dataTransfer?.files;

    if (!files ||
        files.length === 0) {

      return;
    }

    this.selectFile(
      files[0]
    );
  }


  // =========================================================
  // Upload
  // =========================================================

  upload(): void {

    if (!this.selectedFile ||
        this.uploading) {

      return;
    }

    this.clearMessages();

    this.uploading = true;

    this.importsService
      .uploadFile(this.selectedFile)
      .subscribe({

        next: result => {

          this.uploadResult =
            result;

          this.uploading = false;
        },

        error: error => {

          console.error(
            'Import failed',
            error
          );

          this.uploading = false;

          this.errorMessage =
            this.getApiErrorMessage(
              error
            );
        }

      });
  }


  get isImportSuccessful(): boolean {
  return !!this.uploadResult &&
    this.uploadResult.failedRecords === 0;
  }

  get isPartialImport(): boolean {
    return !!this.uploadResult &&
      this.uploadResult.successfulRecords > 0 &&
      this.uploadResult.failedRecords > 0;
  }

  get isImportFailed(): boolean {
    return !!this.uploadResult &&
      this.uploadResult.successfulRecords === 0 &&
      this.uploadResult.failedRecords > 0;
  }

  // =========================================================
  // Reset
  // =========================================================

  reset(): void {

    this.selectedFile = null;

    this.uploadResult = null;

    this.errorMessage = '';

    this.uploading = false;

    this.isDragOver = false;
  }


  // =========================================================
  // Helpers
  // =========================================================

  private clearMessages(): void {

    this.errorMessage = '';
  }


  private getApiErrorMessage(
    error: any
  ): string {

    if (typeof error?.error === 'string') {

      return error.error;
    }

    return error?.error?.message ||
      error?.message ||
      'Unable to import the selected file.';
  }


  getFileSize(
    bytes: number
  ): string {

    if (bytes < 1024) {

      return `${bytes} B`;
    }

    if (bytes < 1024 * 1024) {

      return `${(
        bytes / 1024
      ).toFixed(1)} KB`;
    }

    return `${(
      bytes / (1024 * 1024)
    ).toFixed(2)} MB`;
  }


  getFileExtension(
    fileName: string
  ): string {

    const index =
      fileName.lastIndexOf('.');

    if (index === -1) {

      return '';
    }

    return fileName
      .substring(index + 1)
      .toUpperCase();
  }


  trackByError(
    index: number
  ): number {

    return index;
  }
}