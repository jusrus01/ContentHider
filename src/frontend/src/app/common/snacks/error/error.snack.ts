import { Component } from '@angular/core';
import { MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  template: ` <span class="error">{{ message }}</span> `,
})
export class ErrorSnackComponent {
  public message: string;

  constructor(private _snackbarRef: MatSnackBarRef<ErrorSnackComponent>) {
    this.message = _snackbarRef.containerInstance.snackBarConfig.data;

    if (!this.message) {
      this.message = 'Something went wrong';
    }
  }
}
