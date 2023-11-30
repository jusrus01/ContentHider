import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RoutingService } from '../../services/routing.service';
import { LoginResource } from '../../resources/login.resource';
import { OrgResource } from 'src/app/resources/org.resource';

@Component({
  selector: 'app-organizations',
  templateUrl: './organization.component.html',
  styleUrls: ['./organization.component.scss'],
})
export class OrganizationComponent {
  public hidePassword: boolean;
  public loginForm: FormGroup;

  public organizations: any;

  constructor(
    private formBuilder: FormBuilder,
    private routing: RoutingService,
    private orgResource: OrgResource
  ) {
    this.hidePassword = true;
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
    this.organizations = [];
  }

  ngOnInit() {
    this.orgResource.setText(
      'You can browse through created organizations in this page'
    );

    this.orgResource.getOrgs().subscribe((orgs) => (this.organizations = orgs));
  }

  public createResourceDeleteFunc(organization: any) {
    return () => {
      this.organizations = this.organizations.filter(
        (org: any) => org.id !== organization.id
      );
      return this.orgResource.deleteOrg(organization.id);
    };
  }

  images = [
    'https://media.istockphoto.com/id/1357365823/vector/default-image-icon-vector-missing-picture-page-for-website-design-or-mobile-app-no-photo.jpg?s=612x612&w=0&k=20&c=PM_optEhHBTZkuJQLlCjLz-v3zzxp-1mpNQZsdjrbns=',
    'https://www.shiftbase.com/hs-fs/hubfs/Double%20exposure%20of%20businessman%20working%20with%20new%20modern%20computer%20show%20social%20network%20structure.jpeg?width=725&name=Double%20exposure%20of%20businessman%20working%20with%20new%20modern%20computer%20show%20social%20network%20structure.jpeg',
    'https://cdn-cashy-static-assets.lucidchart.com/marketing/blog/2017Q1/7-types-organizational-structure/types-organizational-structures.png',
    'https://images.ctfassets.net/im32vxdgp4tj/87W1hOJ3PhZIZhvf39OYi/6b7c141aa096f4cc54d121d4db7aa333/FRIDAY_-Hero-Article-AUG-12-2020-scaled.jpg',
    'https://whatfix.com/blog/wp-content/uploads/2021/10/refresh-images-8.png',
    'https://helpfulprofessor.com/wp-content/uploads/2023/08/social-organization-examples-and-definition-1024x724.jpg',
    'https://teambuilding.com/wp-content/uploads/2023/02/organizational-culture.jpg',
  ];

  getRandomImageUrl() {
    const randomIndex = Math.floor(Math.random() * this.images.length);
    return this.images[randomIndex];
  }
}
