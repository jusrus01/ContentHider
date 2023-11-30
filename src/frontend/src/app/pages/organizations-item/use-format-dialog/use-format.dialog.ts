import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { OrgResource } from 'src/app/resources/org.resource';

@Component({
  selector: 'app-use-format',
  templateUrl: './use-format.dialog.html',
})
export class UseFormatDialog {
  public output: any;

  constructor(
    private dialogRef: MatDialogRef<UseFormatDialog>,
    private orgResource: OrgResource,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit() {
    this.output = 'No rules applied yet.';
  }

  public apply(text: any, outputArea: any) {
    const clearText = text.trim();
    this.orgResource
      .applyRulesForText(this.data.orgId, this.data.formatId, clearText)
      .subscribe((response) => {
        this.output = response.text;

        outputArea.value = this.output;
        outputArea.style.height = 'auto';
        outputArea.style.height = outputArea.scrollHeight + 'px';
      });
  }

  public choosePrize(prize: any): void {
    this.dialogRef.close(prize);
  }
}
