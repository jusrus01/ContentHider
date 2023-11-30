import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ResourceDeleteConfirmDialog } from './resource-delete-confirm.dialog';

@Component({
  selector: 'app-resource-delete-common',
  templateUrl: './resource-delete.component.html',
})
export class ResourceDeleteComponent {
  @Input('resourceDeleteFunc') resourceDeleteFunc: any | null;

  constructor(private dialog: MatDialog) {}

  public openAreYouSureDialog(): void {
    this.dialog.open(ResourceDeleteConfirmDialog, {
      data: {
        resourceDeleteFunc: () => this.resourceDeleteFunc(),
      },
    });
  }
}
