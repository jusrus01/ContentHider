import { Component } from '@angular/core';
import { OrgResource } from 'src/app/resources/org.resource';
import { ActivatedRoute } from '@angular/router';
import { RoutingService } from 'src/app/services/routing.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-organization-configuration',
  templateUrl: './organization-configuration.component.html',
  styleUrls: ['./organization-configuration.component.scss'],
})
export class OrganizationConfiguratiomComponent {
  public org: any;
  public form!: FormGroup;

  public _forEdit = true;

  constructor(
    private route: ActivatedRoute,
    private routing: RoutingService,
    private orgResource: OrgResource,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.orgResource.setText(
      'You can change organization specific settings in this page'
    );

    this.route.paramMap.subscribe((paramMap) => {
      const id = paramMap.get('id');
      if (id === null) {
        this._forEdit = false;
        this.org = {
          title: '',
          description: '',
        };
        this.form = this.formBuilder.group({
          title: [this.org.title, Validators.required],
          description: [this.org.description, Validators.required],
        });
        return;
      }

      this.orgResource.getOrg(id).subscribe((org) => {
        this.org = org;

        this.form = this.formBuilder.group({
          title: [this.org.title, Validators.required],
          description: [this.org.description, Validators.required],
        });
      });
    });
  }

  public confirm() {
    if (!this.form.valid) {
      return;
    }

    if (this._forEdit) {
      this.orgResource
        .updateOrg(this.org.id, this.form.value)
        .subscribe(() => this.routing.goToOrg(this.org.id));
    } else {
      this.orgResource
        .createOrg(this.form.value)
        .subscribe((createdOrg: any) => this.routing.goToOrg(createdOrg.id));
    }
  }
}
