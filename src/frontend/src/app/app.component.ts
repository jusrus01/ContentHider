import { Component } from '@angular/core';
import { OrgResource } from './resources/org.resource';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'ContentHider';

  public footerText: any;

  constructor(private orgResource: OrgResource) {}

  ngOnInit() {
    this.orgResource.text$.subscribe((newText) => {
      setTimeout(() => (this.footerText = newText));
    });
  }
}
