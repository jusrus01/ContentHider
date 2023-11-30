import { Component } from '@angular/core';
import { RoutingService } from '../../services/routing.service';
import { OrgResource } from 'src/app/resources/org.resource';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { UseFormatDialog } from './use-format-dialog/use-format.dialog';
import { FormBuilder, Validators } from '@angular/forms';
import { Observable, tap } from 'rxjs';

@Component({
  selector: 'app-organization-item',
  templateUrl: './organization-item.component.html',
  styleUrls: ['./organization-item.component.scss'],
})
export class OrganizationItemComponent {
  public org: any;

  public formatConfiguration: any = null;

  constructor(
    private route: ActivatedRoute,
    private routing: RoutingService,
    private dialog: MatDialog,
    private orgResource: OrgResource,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.orgResource.setText(
      'You can see detailed information about an organization in this page'
    );

    this.route.paramMap.subscribe((paramMap) => {
      const id = paramMap.get('id');
      if (id === null) {
        this.routing.goToNotFound();
        return;
      }

      this.orgResource.getOrg(id).subscribe((org) => (this.org = org));
    });
  }

  public hasPresetConfiguration(organization: any): boolean {
    return organization?.formats?.length > 0;
  }

  public deleteOrg(): any {
    return () =>
      this.orgResource.deleteOrg(this.org.id).pipe(
        tap({
          next: () => {
            this.routing.goToOrganizations();
          },
        })
      );
  }

  public createDeleteFormatFunc(orgId: any, format: any) {
    return () =>
      this.orgResource.deleteFormat(orgId, format.id).pipe(
        tap({
          next: () => {
            this.org.formats = this.org.formats.filter(
              (f: any) => f.id !== format.id
            );
          },
        })
      );
  }

  public isConfiguring(): boolean {
    return this.formatConfiguration !== null;
  }

  public configureFormat() {
    if (this.isConfiguring()) {
      this.stopConfigureFormat();
      return;
    }

    this.formatConfiguration = this.formBuilder.group({
      title: ['', Validators.required],
      formatDefinition: ['', Validators.required],
      type: [0, Validators.required],
      description: ['', Validators.required],
    });
  }

  public createFormat() {
    if (!this.formatConfiguration.valid) {
      return;
    }

    this.orgResource
      .createFormat(this.org.id, this.formatConfiguration.value)
      .subscribe((format) => {
        if (this.org.formats === null) {
          this.org.formats = [];
        }

        this.org.formats.push(format);

        this.stopConfigureFormat();
      });
  }

  public stopConfigureFormat() {
    this.formatConfiguration = null;
  }

  public openUseFormatDialog(format: any) {
    this.dialog.open(UseFormatDialog, {
      // width: '600px',
      data: {
        orgId: this.org.id,
        formatId: format.id,
        preview: format.formatDefinition,
      },
      // disableClose: true,
    });
  }
}
