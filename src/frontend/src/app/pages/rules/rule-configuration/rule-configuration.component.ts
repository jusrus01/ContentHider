import { Component } from '@angular/core';
import { RoutingService } from '../../../services/routing.service';
import { OrgResource } from 'src/app/resources/org.resource';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-rule-item-configuration',
  templateUrl: './rule-configuration.component.html',
  styleUrls: ['./rule-configuration.component.scss'],
})
export class RuleConfigurationComponent {
  public rule: any;
  public form!: FormGroup;

  public _orgId: any;
  public _formatId: any;

  public sample: any;

  constructor(
    private route: ActivatedRoute,
    private routing: RoutingService,
    private orgResource: OrgResource,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.orgResource.setText(
      'You can edit specific rule settings here. Make sure that new field is part of format definition'
    );

    this.route.paramMap.subscribe((paramMap) => {
      const orgId = paramMap.get('orgId');
      const formatId = paramMap.get('formatId');
      const ruleId = paramMap.get('ruleId');
      if (orgId === null || formatId == null || ruleId == null) {
        this.routing.goToNotFound();
        return;
      }

      this._orgId = orgId;
      this._formatId = formatId;

      this.orgResource.getRule(orgId, formatId, ruleId).subscribe((rule) => {
        this.rule = rule;
        this.form = this.formBuilder.group({
          title: [this.rule.title, Validators.required],
          anonymizedField: [this.rule.anonymizedField, Validators.required],
        });
      });

      this.orgResource.getFormat(orgId, formatId).subscribe((format) => {
        this.sample = format.formatDefinition;
      });
    });
  }

  public updateRule() {
    if (!this.form.valid) {
      return;
    }

    this.orgResource
      .updateRule(this._orgId, this._formatId, this.rule.id, this.form.value)
      .subscribe((_) => this.routing.goToFormat(this._orgId, this._formatId));
  }
}
