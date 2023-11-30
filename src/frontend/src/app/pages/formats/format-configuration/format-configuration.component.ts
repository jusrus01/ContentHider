import { Component } from '@angular/core';
import { RoutingService } from '../../../services/routing.service';
import { OrgResource } from 'src/app/resources/org.resource';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, tap } from 'rxjs';

@Component({
  selector: 'app-format-item-configuration',
  templateUrl: './format-configuration.component.html',
  styleUrls: ['./format-configuration.component.scss'],
})
export class FormatConfigurationComponent {
  public format: any;
  public form!: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private routing: RoutingService,
    private orgResource: OrgResource,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.orgResource.setText(
      'You can change format specific settings in this page'
    );

    this.route.paramMap.subscribe((paramMap) => {
      const orgId = paramMap.get('orgId');
      const formatId = paramMap.get('formatId');
      if (orgId === null || formatId == null) {
        this.routing.goToNotFound();
        return;
      }

      this.orgResource.getFormat(orgId, formatId).subscribe((format) => {
        this.format = format;
        this.form = this.formBuilder.group({
          title: [this.format.title, Validators.required],
          type: [this.format.type, Validators.required],
          formatDefinition: [this.format.formatDefinition, Validators.required],
        });
      });
    });
  }

  public updateFormat() {
    if (!this.form.valid) {
      return;
    }

    this.orgResource
      .updateFormat(this.format.organizationId, this.format.id, this.form.value)
      .subscribe((_) =>
        this.routing.goToFormat(this.format.organizationId, this.format.id)
      );
  }

  public getTypeName(format: any) {
    if (format === null) {
      throw new Error('Format cannot be null');
    }

    return format.type === 0 ? 'JSON' : 'XML';
  }
}
