import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-resource-delete-common-dialog',
  template: `<h1 mat-dialog-title>
      Are you sure you want to delete this resource?
    </h1>
    <div mat-dialog-content>
      <div class="pb-2">This action cannot be undone.</div>
      <button
        mat-raised-button
        color="primary"
        class="mr-2 bg-default-red"
        (click)="applyDeleteResourceFunc()"
      >
        Confirm
      </button>
      <button mat-raised-button color="accent" (click)="cancel()">
        Cancel
      </button>
    </div>`,
})
export class ResourceDeleteConfirmDialog {
  public output: any;

  constructor(
    private dialogRef: MatDialogRef<ResourceDeleteConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  public applyDeleteResourceFunc() {
    this.data
      .resourceDeleteFunc()
      .subscribe((value: any) => this.dialogRef.close(value));
  }

  public cancel(): void {
    this.dialogRef.close(null);
  }
}
