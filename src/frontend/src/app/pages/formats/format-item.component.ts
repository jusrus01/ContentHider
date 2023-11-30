import { Component } from '@angular/core';
import { RoutingService } from '../../services/routing.service';
import { OrgResource } from 'src/app/resources/org.resource';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { tap } from 'rxjs';

@Component({
  selector: 'app-format-item',
  templateUrl: './format-item.component.html',
  styleUrls: ['./format-item.component.scss'],
})
export class FormatItemComponent {
  public format: any;

  public _rulesConfig: any = null;

  constructor(
    private route: ActivatedRoute,
    private routing: RoutingService,
    private orgResource: OrgResource,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.orgResource.setText(
      'You can create new rules for format in this page'
    );

    this.route.paramMap.subscribe((paramMap) => {
      const orgId = paramMap.get('orgId');
      const formatId = paramMap.get('formatId');
      if (orgId === null || formatId == null) {
        this.routing.goToNotFound();
        return;
      }

      this.orgResource
        .getFormat(orgId, formatId)
        .subscribe((format) => (this.format = format));
    });
  }

  public getTypeName(format: any) {
    if (format === null) {
      throw new Error('Format cannot be null');
    }

    return format.type === 0 ? 'JSON' : 'XML';
  }

  public isConfiguring() {
    return this._rulesConfig !== null;
  }

  public configureRules(): void {
    if (this.isConfiguring()) {
      this._rulesConfig = null;
      return;
    }

    this._rulesConfig = this.formBuilder.group({
      title: ['', Validators.required],
      anonymizedField: ['', Validators.required],
    });
  }

  public confirmRuleConfiguration() {
    if (!this._rulesConfig.valid) {
      return;
    }

    this.orgResource
      .createRule(
        this.format.organizationId,
        this.format.id,
        this._rulesConfig.value
      )
      .subscribe((rule) => {
        if (!this.format.rules) {
          this.format.rules = [];
        }

        this.format.rules.push(rule);
        this._rulesConfig = null;
      });
  }

  private _selectedRule: any = null;

  public markAsSelected(rule: any) {
    this._selectedRule = rule;
  }

  public goToLatestSelectedRule() {
    this.routing.goToRule(
      this.format.organizationId,
      this.format.id,
      this._selectedRule
    );
  }

  public getLatestDeleteRuleFunc(): any {
    return () =>
      this.orgResource
        .deleteRule(
          this.format.organizationId,
          this.format.id,
          this._selectedRule.id
        )
        .pipe(
          tap({
            next: (deleted: any) => {
              this.format.rules = this.format.rules.filter(
                (rule: any) => rule.id !== deleted.id
              );
            },
          })
        );
  }

  public getDeleteFunc(): any {
    if (this.format === null) {
      throw new Error('Format cannot be null');
    }

    return () =>
      this.orgResource
        .deleteFormat(this.format.organizationId, this.format.id)
        .pipe(
          tap({
            next: () => {
              this.routing.goToOrg(this.format.organizationId);
            },
          })
        );
  }
}
