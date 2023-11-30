import { Component } from '@angular/core';
import { OrgResource } from 'src/app/resources/org.resource';
import { RoutingService } from 'src/app/services/routing.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  constructor(
    private routing: RoutingService,
    private orgResource: OrgResource
  ) {}

  ngOnInit() {
    this.orgResource.setText('Resource not found. Did you use correct url?');
  }

  public goToOrganizations() {
    this.routing.goToOrganizations();
  }
}
