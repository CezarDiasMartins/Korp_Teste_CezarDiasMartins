import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class FeedbackService {
  private readonly snackBar = inject(MatSnackBar);

  success(message: string) {
    this.snackBar.open(message, 'OK', { duration: 3500, panelClass: ['snackbar-success'] });
  }

  error(errors: string[] | string) {
    const message = Array.isArray(errors) ? errors.join(' ') : errors;
    this.snackBar.open(message, 'OK', { duration: 6500, panelClass: ['snackbar-error'] });
  }
}